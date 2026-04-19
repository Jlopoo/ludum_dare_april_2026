using System.Collections.Generic;
using Godot;

#nullable enable

/// <summary>
/// Per-scene orchestrator. Owns the in-flight <see cref="ConversationManager"/>,
/// translates its signals into game-state effects, and runs the campaign-level flow:
///
/// - Scores choices into Aura.
/// - Swaps alien portraits based on cue images.
/// - Logs dialogue into <see cref="DialogueLog"/>.
/// - Evaluates each finished date as success/failure against that conversation's
///   aura threshold.
/// - Awards +1 point on success.
/// - Waits for an explicit "Next Date" click from UI before progressing.
///
/// Lives as a child of <c>Main</c> (not an autoload). Persistent run state
/// (Aura, points) stays in <see cref="GameState"/>.
/// </summary>
public partial class GameManager : Node
{
    /// <summary>
    /// Set during <c>_Ready</c> so other scene-local nodes (HUD widgets, etc.) can
    /// reach the active GameManager + its <see cref="ConversationManager"/> without
    /// tree-walking.
    /// </summary>
    public static GameManager? Instance { get; private set; }

    /// <summary>
    /// Optional override for quick testing. If set, campaign order is ignored and this
    /// single conversation is played as a one-level run.
    /// </summary>
    [Export] public Conversation? StartingConversation { get; set; }

    /// <summary>Default line SFX when <see cref="DialogueLine.LineSound"/> is unset.</summary>
    [Export] public AudioStream? LineAdvanceSound { get; set; }

    /// <summary>
    /// When true, each new date starts from <see cref="GameState.StartingAura"/>,
    /// so aura reflects performance within that single conversation level.
    /// </summary>
    [Export] public bool ResetAuraOnDateStart { get; set; } = true;

    /// <summary>
    /// Delay before auto-advancing lines that have no response choices.
    /// Prevents progression from getting stuck on narration/outro lines.
    /// </summary>
    [Export(PropertyHint.Range, "0,5,0.1")] public float NoChoiceAutoAdvanceDelay { get; set; } = 0.9f;

    private ConversationManager _conversationManager = null!;
    private GameState _gameState = null!;
    private AudioStreamPlayer _lineSfx = null!;

    private readonly List<Conversation> _campaign = new();
    private int _campaignIndex = -1;
    private Conversation? _activeConversation;
    private Conversation? _pendingNextConversation;
    private bool _runComplete;

    /// <summary>
    /// The in-flight conversation manager. UI scripts (ResponseWindow, etc.) should
    /// subscribe to its signals via <c>GameManager.Instance.Conversations</c>.
    /// </summary>
    public ConversationManager Conversations => _conversationManager;

    /// <summary>
    /// True only after a date ended and there is another conversation queued.
    /// ResponseWindow uses this to decide whether its "Next Date" button is enabled.
    /// </summary>
    public bool HasPendingNextDate => _pendingNextConversation is not null;

    /// <summary>
    /// True once the last campaign conversation has ended and no next date exists.
    /// </summary>
    public bool RunComplete => _runComplete;

    public override void _Ready()
    {
        base._Ready();

        Instance = this;
        _gameState = GetNode<GameState>("/root/GameState");

        _conversationManager = new ConversationManager { Name = "ConversationManager" };
        AddChild(_conversationManager);

        _lineSfx = new AudioStreamPlayer { Name = "DialogueLineSfx" };
        AddChild(_lineSfx);

        _conversationManager.ConversationStarted += OnConversationStarted;
        _conversationManager.LineAdvanced += OnLineAdvanced;
        _conversationManager.ChoiceSelected += OnChoiceSelected;
        _conversationManager.ConversationEnded += OnConversationEnded;

        BuildCampaign();
        if (_campaign.Count == 0)
        {
            GD.PushError("GameManager: campaign is empty. Add entries in SampleConversations.CampaignOrder().");
            return;
        }

        // Fresh run state every time gameplay scene boots.
        _gameState.ResetRunState();
        _pendingNextConversation = null;
        _runComplete = false;

        _campaignIndex = 0;
        // Defer so HUD/ResponseWindow have entered the tree and connected before
        // the first ChoicesPresented signal fires.
        CallDeferred(nameof(BeginConversation), _campaign[_campaignIndex]);
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

    private void BuildCampaign()
    {
        _campaign.Clear();

        if (StartingConversation is not null)
        {
            _campaign.Add(StartingConversation);
            return;
        }

        _campaign.AddRange(SampleConversations.CampaignOrder());
    }

    private static void PlayIfSet(AudioStreamPlayer player, AudioStream? stream)
    {
        if (stream is null) return;
        player.Stream = stream;
        player.Play();
    }

    /// <summary>
    /// Public entry-point for starting a conversation. External callers should prefer
    /// this over poking <see cref="ConversationManager"/> directly.
    /// </summary>
    public void BeginConversation(Conversation conversation)
    {
        _runComplete = false;
        _activeConversation = conversation;
        _conversationManager.Start(conversation);
    }

    /// <summary>
    /// Called by UI (ResponseWindow "Next Date" button) after a date ends.
    /// Starts the queued next conversation if one exists.
    /// </summary>
    public void ContinueToNextDate()
    {
        if (_pendingNextConversation is null)
        {
            GD.Print("[GameManager] ContinueToNextDate called, but no pending conversation exists.");
            return;
        }

        _campaignIndex++;
        Conversation next = _pendingNextConversation;
        _pendingNextConversation = null;
        GD.Print($"[GameManager] Advancing to next date {_campaignIndex + 1}/{_campaign.Count}: {next.ConversationId}");
        BeginConversation(next);
    }

    private void OnConversationStarted(Conversation conversation)
    {
        _activeConversation = conversation;

        if (ResetAuraOnDateStart)
        {
            _gameState.SetAura(_gameState.StartingAura);
            GD.Print($"[GameManager] Aura reset for new date: {_gameState.Aura:F0}/{_gameState.MaxAura:F0}");
        }

        // Set the alien's default portrait at the start of a date.
        Texture2D? portrait = conversation.Alien?.Species?.DefaultPortrait;
        if (portrait != null)
        {
            _gameState.RequestCharacterChange(portrait);
        }

        int dateNumber = _campaignIndex + 1;
        string alienName = conversation.Alien?.Name ?? "your next date";
        DialogueLog.Instance?.AddEntry("System", $"Date {dateNumber} of {_campaign.Count}. {alienName} takes the seat across from you.");
        DialogueLog.Instance?.AddEntry("System", $"Goal: finish this date above {conversation.SuccessAuraThreshold:F0} aura.");
    }

    private void OnLineAdvanced(DialogueLine line)
    {
        Conversation? current = _conversationManager.Current;
        Species? species = current?.Alien?.Species;

        // Portrait: prefer cue-specific image; fall back to species default so a
        // no-cue line doesn't leave the prior expression frozen.
        Texture2D? portrait = species?.ImageFor(line.Cue) ?? species?.DefaultPortrait;
        if (portrait != null)
        {
            _gameState.RequestCharacterChange(portrait);
        }

        // Use alien display name when SpeakerId matches it, otherwise keep the literal SpeakerId.
        string speaker = !string.IsNullOrEmpty(current?.Alien?.Name) && line.SpeakerId == current!.Alien!.Name
            ? current.Alien.Name
            : line.SpeakerId;
        DialogueLog.Instance?.AddEntry(speaker, line.Text);

        PlayIfSet(_lineSfx, line.LineSound ?? LineAdvanceSound);

        // If a line has no choices, advance automatically after a short delay.
        // Without this, the flow can stall on terminal/outro lines.
        if (line.Choices.Count == 0 && current is not null)
        {
            GD.Print($"[GameManager] Auto-advance queued for no-choice line in '{current.ConversationId}'.");
            QueueAutoAdvanceForNoChoiceLine(current, line);
        }
    }

    private async void QueueAutoAdvanceForNoChoiceLine(Conversation scheduledConversation, DialogueLine scheduledLine)
    {
        if (NoChoiceAutoAdvanceDelay > 0f)
        {
            await ToSignal(GetTree().CreateTimer(NoChoiceAutoAdvanceDelay), SceneTreeTimer.SignalName.Timeout);
            if (!IsInsideTree()) return;
        }

        // Guard against stale timers after the player already progressed.
        if (_conversationManager.Current != scheduledConversation) return;
        if (_conversationManager.CurrentLine != scheduledLine) return;
        if (_conversationManager.State != ConversationState.Presenting) return;

        _conversationManager.Advance();
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        Conversation? current = _conversationManager.Current;
        DialogueLine? currentLine = _conversationManager.CurrentLine;

        // 1) Log the player's selected response.
        DialogueLog.Instance?.AddEntry("You", choice.Text);

        // 2) Score it: tone match + manual delta authored on the choice.
        Tone? expected = CueResolver.Resolve(current, currentLine);
        int toneDelta = expected is { } e ? ToneScoring.AuraDelta(choice.Tone, e) : 0;
        int totalDelta = toneDelta + choice.AffectionDelta;

        if (totalDelta != 0)
        {
            _gameState.ApplyAffectionDelta(totalDelta);
            GD.Print($"[GameManager] aura {(totalDelta >= 0 ? "+" : "")}{totalDelta} (tone={toneDelta}, manual={choice.AffectionDelta}) -> {_gameState.Aura:F0}/{_gameState.MaxAura:F0}");
        }

        // 3) Log the alien's reaction line if authored.
        if (!string.IsNullOrEmpty(choice.ResponseText))
        {
            string speaker = current?.Alien?.Name ?? "Alien";
            DialogueLog.Instance?.AddEntry(speaker, choice.ResponseText);
        }
    }

    private void OnConversationEnded(string conversationId)
    {
        GD.Print($"[GameManager] Conversation ended: {conversationId}");
        Conversation? finished = _activeConversation;
        _activeConversation = null;

        if (finished is null)
        {
            DialogueLog.Instance?.AddEntry("System", "The date wraps up.");
            return;
        }

        bool success = _gameState.Aura > finished.SuccessAuraThreshold;
        if (success)
        {
            _gameState.AddPoint(1);
            DialogueLog.Instance?.AddEntry("System", $"Success. You ended at {_gameState.Aura:F0} aura (target was above {finished.SuccessAuraThreshold:F0}). +1 successful date.");
        }
        else
        {
            DialogueLog.Instance?.AddEntry("System", $"No spark this round. You ended at {_gameState.Aura:F0} aura (needed above {finished.SuccessAuraThreshold:F0}).");
        }

        string outro = success ? finished.SuccessOutro : finished.FailureOutro;
        if (!string.IsNullOrWhiteSpace(outro))
        {
            DialogueLog.Instance?.AddEntry(finished.Alien?.Name ?? "System", outro);
        }

        if (_campaignIndex + 1 < _campaign.Count)
        {
            _pendingNextConversation = _campaign[_campaignIndex + 1];
            DialogueLog.Instance?.AddEntry("System", $"Up next: {_pendingNextConversation.Alien?.Name ?? _pendingNextConversation.ConversationId}.");
            DialogueLog.Instance?.AddEntry("System", "Click 'Next Date' when you're ready.");
            GD.Print($"[GameManager] Next conversation queued: {_pendingNextConversation.ConversationId}");
        }
        else
        {
            _pendingNextConversation = null;
            _runComplete = true;
            DialogueLog.Instance?.AddEntry("System", $"All dates complete. Final score: {_gameState.Points}/{_campaign.Count} successful dates.");
            GD.Print("[GameManager] Campaign complete.");
        }
    }
}
