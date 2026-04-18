using Godot;
using Godot.Collections;

#nullable enable

[GlobalClass]
public partial class DialogueLine : Resource
{
    [Export] public string SpeakerId { get; set; } = "";

    [Export(PropertyHint.MultilineText)] public string Text { get; set; } = "";

    [Export] public Array<DialogueChoice> Choices { get; set; } = new();
}
