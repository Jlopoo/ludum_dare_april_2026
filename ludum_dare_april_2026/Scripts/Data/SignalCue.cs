using Godot;

namespace LudumDareApril2026.Data;

/// <summary>
/// A signal an alien emits when reacting to a response. This is the thing the
/// player is learning to read: a color flash, a short phrase, an icon, a sound.
/// <para/>
/// Species intentionally assign different cues to each <see cref="ReactionType"/>,
/// so the player has to observe and infer which cue corresponds to which
/// reaction for each species.
/// </summary>
[GlobalClass]
public partial class SignalCue : Resource
{
    /// <summary>Short description shown in the signal journal (e.g. "eyes glow blue").</summary>
    [Export(PropertyHint.MultilineText)] public string Description { get; set; } = "";

    /// <summary>Accent color flashed over the alien portrait when the cue fires.</summary>
    [Export] public Color DisplayColor { get; set; } = new(1f, 1f, 1f, 1f);

    /// <summary>Single glyph or short label to display on the reaction overlay.</summary>
    [Export] public string Glyph { get; set; } = "";

    /// <summary>Optional sprite shown alongside or instead of the glyph.</summary>
    [Export] public Texture2D? Icon { get; set; }

    /// <summary>Optional SFX played when the cue fires.</summary>
    [Export] public AudioStream? SoundEffect { get; set; }
}
