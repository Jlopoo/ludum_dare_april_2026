using Godot;
using Godot.Collections;

/// <summary>
/// Three-button response panel. Subscribes to <see cref="ConversationManager.ChoicesPresented"/>
/// (via <see cref="GameManager.Instance"/>) to relabel its buttons whenever a new set of
/// player choices appears, and routes button presses to <see cref="ConversationManager.SelectChoice"/>.
///
/// Owns no game state — just UI plumbing. Hidden buttons mean "this slot has no choice".
/// </summary>
public partial class ResponseWindow : Control
{
    private Button response1;
    private Button response2;
    private Button response3;
    private TextureRect icon1;
    private TextureRect icon2;
    private TextureRect icon3;
    private ConversationManager _conversationManager;

    private readonly Dictionary<Tone, string> _toneIconPaths = new()
    {
        { Tone.Affectionate, "res://Assets/Art/toneAffectionate.png" },
        { Tone.Aggressive, "res://Assets/Art/toneAggression.png" },
        { Tone.Intellectual, "res://Assets/Art/toneIntellectual.png" },
    };

    public override void _Ready()
    {
        base._Ready();

        // Buttons live inside a VBoxContainer so they auto-distribute vertical space
        // and resize cleanly with the parent ResponseWindow.
        response1 = GetNode<Button>("VBoxContainer/Response_1");
        response2 = GetNode<Button>("VBoxContainer/Response_2");
        response3 = GetNode<Button>("VBoxContainer/Response_3");

        icon1 = GetNode<TextureRect>("VBoxContainer/Response_1/Icon_1");
        icon2 = GetNode<TextureRect>("VBoxContainer/Response_2/Icon_2");
        icon3 = GetNode<TextureRect>("VBoxContainer/Response_3/Icon_3");

        // Disable keyboard focus so spacebar / enter doesn't accidentally re-trigger a button.
        response1.FocusMode = FocusModeEnum.Click;
        response2.FocusMode = FocusModeEnum.Click;
        response3.FocusMode = FocusModeEnum.Click;

        // Resolve the ConversationManager via the GameManager singleton — much more
        // reliable than walking the scene tree by node name.
        _conversationManager = GameManager.Instance?.Conversations;

        if (_conversationManager == null)
        {
            GD.PrintErr("ResponseWindow: GameManager.Instance is null. Is the GameManager node in the scene tree?");
            return;
        }

        response1.Pressed += () => _conversationManager.SelectChoice(0);
        response2.Pressed += () => _conversationManager.SelectChoice(1);
        response3.Pressed += () => _conversationManager.SelectChoice(2);

        _conversationManager.ChoicesPresented += OnChoicesPresented;
        GD.Print("ResponseWindow connected to ConversationManager (via GameManager.Instance)");
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_conversationManager != null)
        {
            _conversationManager.ChoicesPresented -= OnChoicesPresented;
        }
    }

    private void OnChoicesPresented(Array<DialogueChoice> choices)
    {
        GD.Print($"ResponseWindow received {choices.Count} choices");

        response1.Visible = choices.Count > 0;
        response1.Text = choices.Count > 0 ? choices[0].Text : "";
        icon1.Visible = choices.Count > 0;
        SetToneIcon(icon1, choices.Count > 0 ? choices[0].Tone : Tone.None);

        response2.Visible = choices.Count > 1;
        response2.Text = choices.Count > 1 ? choices[1].Text : "";
        icon2.Visible = choices.Count > 1;
        SetToneIcon(icon2, choices.Count > 1 ? choices[1].Tone : Tone.None);

        response3.Visible = choices.Count > 2;
        response3.Text = choices.Count > 2 ? choices[2].Text : "";
        icon3.Visible = choices.Count > 2;
        SetToneIcon(icon3, choices.Count > 2 ? choices[2].Tone : Tone.None);
    }

    private void SetToneIcon(TextureRect textureRect, Tone tone)
    {
        if (tone == Tone.None || !_toneIconPaths.ContainsKey(tone))
        {
            textureRect.Texture = null;
            return;
        }

        textureRect.Texture = GD.Load<Texture2D>(_toneIconPaths[tone]);
    }
}
