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
    private ConversationManager _conversationManager;

    public override void _Ready()
    {
        base._Ready();

        // The buttons are direct children of ResponseWindow in the .tscn (no VBoxContainer wrapper).
        response1 = GetNode<Button>("Response_1");
        response2 = GetNode<Button>("Response_2");
        response3 = GetNode<Button>("Response_3");

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

        response2.Visible = choices.Count > 1;
        response2.Text = choices.Count > 1 ? choices[1].Text : "";

        response3.Visible = choices.Count > 2;
        response3.Text = choices.Count > 2 ? choices[2].Text : "";
    }
}
