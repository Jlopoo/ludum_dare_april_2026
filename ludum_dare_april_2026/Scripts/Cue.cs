/// <summary>
/// A non-verbal "signal" an alien shows on a dialogue line — what the player observes
/// and must interpret. Cues are intentionally species-specific (one species' cues never
/// overlap with another's) so a player who sees <see cref="BubbleEars"/> immediately
/// knows they're looking at a Floopian.
///
/// Add new cues here as you add new species. Each species defines what its cues *mean*
/// via its <see cref="Species.CueTones"/> mapping.
/// </summary>
public enum Cue
{
    None = 0,

    // Alien 2
    BloodshotEyes,
    WidePupils,
    TongueOut,
}
