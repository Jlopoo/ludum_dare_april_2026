using Godot;
using Godot.Collections;

public partial class ResponseWindow : Control
{
    private Button response1;
    private Button response2;
    private Button response3;
    private ConversationManager _conversationManager;

    public override void _Ready()
    {
        base._Ready();

        // Cache button references through VBoxContainer
        response1 = GetNode<Button>("VBoxContainer/Response_1");
        response2 = GetNode<Button>("VBoxContainer/Response_2");
        response3 = GetNode<Button>("VBoxContainer/Response_3");

        // Disable keyboard focus so space bar doesn't trigger buttons
        response1.FocusMode = Control.FocusModeEnum.Click;
        response2.FocusMode = Control.FocusModeEnum.Click;
        response3.FocusMode = Control.FocusModeEnum.Click;

        // Find ConversationManager in the scene tree
        var root = GetTree().Root;
        _conversationManager = root.FindChild("ConversationManager", true, false) as ConversationManager;

        if (_conversationManager != null)
        {
            // Connect button pressed signals to SelectChoice
            response1.Pressed += () => _conversationManager.SelectChoice(0);
            response2.Pressed += () => _conversationManager.SelectChoice(1);
            response3.Pressed += () => _conversationManager.SelectChoice(2);

            // Listen for choices to update button labels
            _conversationManager.ChoicesPresented += OnChoicesPresented;
            GD.Print("ResponseWindow connected to ConversationManager");
        }
        else
        {
            GD.PrintErr("ResponseWindow could not find ConversationManager in scene tree");
        }
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
        // Update button visibility and text based on available choices
        response1.Visible = choices.Count > 0;
        response1.Text = choices.Count > 0 ? choices[0].Text : "";

        response2.Visible = choices.Count > 1;
        response2.Text = choices.Count > 1 ? choices[1].Text : "";

        response3.Visible = choices.Count > 2;
        response3.Text = choices.Count > 2 ? choices[2].Text : "";
    }
}
