using Godot;
using Godot.Collections;

#nullable enable

[GlobalClass]
public partial class DialogueLine : Resource
{
    [Export] public string SpeakerId { get; set; } = "";

    [Export(PropertyHint.MultilineText)] public string Text { get; set; } = "";

    [Export] public Array<DialogueChoice> Choices { get; set; } = new();

    /// <summary>
    /// Optional non-verbal "signal" the alien shows on this line. Combined with the
    /// owning <see cref="Conversation.Alien"/> via <see cref="CueResolver"/> to derive
    /// the response <see cref="Tone"/> the player is "supposed to" pick. Leave as
    /// <see cref="global::Cue.None"/> for narration / player lines / unscored lines.
    /// </summary>
    [Export] public Cue Cue { get; set; } = Cue.None;

    /// <summary>
    /// Optional clip when this line is shown; if unset, the scene's GameManager default applies.
    /// </summary>
    [Export] public AudioStream? LineSound { get; set; }
}
