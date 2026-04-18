using Godot;
using Godot.Collections;

#nullable enable

[GlobalClass]
public partial class Conversation : Resource
{
    /// <summary>
    /// Stable identifier for the conversation. Used by listeners (analytics, save data,
    /// quest tracking) and emitted with <see cref="ConversationManager.ConversationEnded"/>.
    /// </summary>
    [Export] public string ConversationId { get; set; } = "";

    [Export] public Array<DialogueLine> Lines { get; set; } = new();
}
