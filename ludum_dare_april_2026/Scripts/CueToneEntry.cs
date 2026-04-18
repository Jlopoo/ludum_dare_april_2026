using Godot;

#nullable enable

/// <summary>
/// One row of a <see cref="Species"/>' lookup table: "when this species shows
/// <see cref="Cue"/>, the polite response is in this <see cref="Tone"/>."
///
/// Modelled as a Resource (rather than a Godot typed Dictionary export) so the inspector
/// renders each entry as a clean two-field row designers can edit comfortably.
///
/// Future-proofing: when planetary customs land, this row's <c>Tone</c> field will
/// become a <c>Mood</c> field (the species-only inner state) and a separate
/// <c>Planet</c> resource will map <c>Mood → Tone</c>. The split preserves authoring
/// volume — N species + M planets, not N×M.
/// </summary>
[GlobalClass]
public partial class CueToneEntry : Resource
{
    [Export] public Cue Cue { get; set; } = Cue.None;
    [Export] public Tone Tone { get; set; } = Tone.None;
}
