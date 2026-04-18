/// <summary>
/// The three response categories the player can choose between in the cue-reading puzzle.
/// Each <see cref="DialogueChoice"/> is tagged with one (or <see cref="None"/> for choices
/// that aren't part of the puzzle, e.g. always-available "leave" / "tell me more").
///
/// <see cref="Affectionate"/> and <see cref="Aggressive"/> are direct opposites
/// (see <see cref="ToneScoring"/>); <see cref="Intellectual"/> is the neutral middle.
/// </summary>
public enum Tone
{
    None = 0,
    Affectionate,
    Aggressive,
    Intellectual,
}
