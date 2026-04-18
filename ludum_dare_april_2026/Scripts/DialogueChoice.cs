using Godot;

#nullable enable

[GlobalClass]
public partial class DialogueChoice : Resource
{
    [Export] public string Text { get; set; } = "";

    [Export] public int AffectionDelta { get; set; }

    [Export] public string ResponseText { get; set; } = "";

    /// <summary>
    /// Optional follow-up conversation that replaces the current one when this choice is selected.
    /// If null, the current conversation continues with the next line.
    /// </summary>
    [Export] public Conversation? NextConversation { get; set; }
}
