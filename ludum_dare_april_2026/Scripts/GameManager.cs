using Godot;

#nullable enable

/// <summary>
/// Per-scene orchestrator. Owns the in-flight <see cref="ConversationManager"/>,
/// translates its signals into game-state effects:
/// <list type="bullet">
///   <item><description><b>Aura scoring</b> — combines tone match (<see cref="ToneScoring"/>) with the choice's manual <see cref="DialogueChoice.AffectionDelta"/> and pushes to <see cref="GameState"/>.</description></item>
///   <item><description><b>Portrait swaps</b> — looks up the alien's <see cref="Species"/> for a cue-specific image, falls back to <see cref="Species.DefaultPortrait"/>, and routes through <see cref="GameState.RequestCharacterChange"/>.</description></item>
///   <item><description><b>Dialogue log</b> — writes spoken lines and chosen player responses to <see cref="DialogueLog"/>.</description></item>
/// </list>
///
/// Lives as a child of the <c>Main</c> node, NOT as an autoload. Persistent
/// cross-scene state stays in <see cref="GameState"/>.
///
/// To start a different conversation, drag a <c>.tres</c> onto the
/// <see cref="StartingConversation"/> slot in the inspector. Otherwise it falls
/// back to the hardcoded <see cref="SampleConversations.SquilliamIntro"/>.
/// </summary>
public partial class GameManager : Node
{
    /// <summary>
    /// Set during <c>_Ready</c> so other scene-local nodes (HUD widgets, etc.) can
    /// reach the active GameManager + its <see cref="ConversationManager"/> without
    /// tree-walking. Mirrors the <see cref="GameState"/> singleton pattern.
    /// </summary>
    public static GameManager? Instance { get; private set; }

    /// <summary>
    /// First conversation to start when the scene loads. Drag a Conversation .tres
    /// here to override the default sample.
    /// </summary>
    [Export] public Conversation? StartingConversation { get; set; }

    private ConversationManager _conversationManager = null!;
    private GameState _gameState = null!;

    /// <summary>
    /// The in-flight conversation manager. UI scripts (ResponseWindow, etc.) should
    /// subscribe to its signals via <c>GameManager.Instance.Conversations</c>
    /// instead of tree-walking for a node by name.
    /// </summary>
    public ConversationManager Conversations => _conversationManager;

    public override void _Ready()
    {
        base._Ready();

        Instance = this;
        _gameState = GetNode<GameState>("/root/GameState");

        _conversationManager = new ConversationManager { Name = "ConversationManager" };
        AddChild(_conversationManager);

        _conversationManager.ConversationStarted += OnConversationStarted;
        _conversationManager.LineAdvanced += OnLineAdvanced;
        _conversationManager.ChoiceSelected += OnChoiceSelected;
        _conversationManager.ConversationEnded += OnConversationEnded;

        // Defer so HUD/ResponseWindow have entered the tree and connected their
        // own signal handlers before the first ChoicesPresented fires.
        Conversation conversation = StartingConversation ?? SampleConversations.SquilliamIntro();
        CallDeferred(nameof(BeginConversation), conversation);
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        if (Instance == this) Instance = null;

        if (_conversationManager != null)
        {
            _conversationManager.ConversationStarted -= OnConversationStarted;
            _conversationManager.LineAdvanced -= OnLineAdvanced;
            _conversationManager.ChoiceSelected -= OnChoiceSelected;
            _conversationManager.ConversationEnded -= OnConversationEnded;
        }
    }

    /// <summary>
    /// Public entry-point for starting (or chaining into) a conversation. External
    /// callers should prefer this over poking <see cref="ConversationManager"/> directly.
    /// </summary>
    public void BeginConversation(Conversation conversation)
    {
        _conversationManager.Start(conversation);
    }

    private void OnConversationStarted(Conversation conversation)
    {
        // Set the alien's default portrait at the start of a date.
        Texture2D? portrait = conversation.Alien?.Species?.DefaultPortrait;
        if (portrait != null)
        {
            _gameState.RequestCharacterChange(portrait);
        }

        DialogueLog.Instance?.AddEntry("System", $"— {conversation.Alien?.Name ?? "???"} sits down across from you —");
    }

    private void OnLineAdvanced(DialogueLine line)
    {
        Conversation? current = _conversationManager.Current;
        Species? species = current?.Alien?.Species;

        // Portrait: prefer the cue-specific image; fall back to the species default
        // so a no-cue line doesn't leave the previous cue's expression frozen on screen.
        Texture2D? portrait = species?.ImageFor(line.Cue) ?? species?.DefaultPortrait;
        if (portrait != null)
        {
            _gameState.RequestCharacterChange(portrait);
        }

        // Log the spoken line. Use the alien's display name when available so the
        // log shows "Squilliam:" instead of the raw SpeakerId on alien lines.
        string speaker = !string.IsNullOrEmpty(current?.Alien?.Name) && line.SpeakerId == current!.Alien!.Name
            ? current.Alien.Name
            : line.SpeakerId;
        DialogueLog.Instance?.AddEntry(speaker, line.Text);
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        Conversation? current = _conversationManager.Current;
        DialogueLine? currentLine = _conversationManager.CurrentLine;

        // 1. Log the player's chosen response in their own voice.
        DialogueLog.Instance?.AddEntry("You", choice.Text);

        // 2. Score it. Tone-match (cue-driven) + the choice's manual AffectionDelta.
        Tone? expected = CueResolver.Resolve(current, currentLine);
        int toneDelta = expected is { } e ? ToneScoring.AuraDelta(choice.Tone, e) : 0;
        int totalDelta = toneDelta + choice.AffectionDelta;

        if (totalDelta != 0)
        {
            _gameState.ApplyAffectionDelta(totalDelta);
            GD.Print($"[GameManager] aura {(totalDelta >= 0 ? "+" : "")}{totalDelta} (tone={toneDelta}, manual={choice.AffectionDelta}) → {_gameState.Aura:F0}/{_gameState.MaxAura:F0}");
        }

        // 3. Log the alien's in-line reaction (if the choice had one).
        if (!string.IsNullOrEmpty(choice.ResponseText))
        {
            string speaker = current?.Alien?.Name ?? "Alien";
            DialogueLog.Instance?.AddEntry(speaker, choice.ResponseText);
        }
    }

    private void OnConversationEnded(string conversationId)
    {
        DialogueLog.Instance?.AddEntry("System", "— end of date —");
        // TODO: queue next alien, transition to results screen, etc.
    }
}
