using Godot;

#nullable enable

[GlobalClass]
public partial class DialogueChoice : Resource
{
    [Export] public string Text { get; set; } = "";

    [Export] public int AffectionDelta { get; set; }

    [Export] public string ResponseText { get; set; } = "";

    /// <summary>
    /// Categorises this response as Affectionate / Aggressive / Intellectual.
    /// Compared against the resolved expected <see cref="global::Tone"/> for the
    /// line's <see cref="Cue"/> by <see cref="ToneScoring"/>. Leave as
    /// <see cref="global::Tone.None"/> for choices that aren't part of the
    /// cue-reading puzzle (e.g. always-available "leave" / "tell me more" options).
    /// </summary>
    [Export] public Tone Tone { get; set; } = Tone.None;

    /// <summary>
    /// Optional follow-up conversation that replaces the current one when this choice is selected.
    /// If null, the current conversation continues with the next line.
    /// </summary>
    [Export] public Conversation? NextConversation { get; set; }
}
