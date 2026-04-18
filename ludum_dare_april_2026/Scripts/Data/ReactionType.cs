namespace LudumDareApril2026.Data;

/// <summary>
/// How the alien feels about a given response. Ordered intentionally so that a
/// higher integer value corresponds to a "better" reaction, which keeps score
/// aggregation and comparisons straightforward.
/// </summary>
public enum ReactionType
{
    Bad = 0,
    Neutral = 1,
    Good = 2,
}

public static class ReactionTypeExtensions
{
    /// <summary>
    /// Normalized score contribution in the 0..1 range. Used by <c>AuraMeter</c>
    /// to compute the running average that determines date success.
    /// </summary>
    public static float GetScore(this ReactionType reaction) => reaction switch
    {
        ReactionType.Bad => 0.0f,
        ReactionType.Neutral => 0.5f,
        ReactionType.Good => 1.0f,
        _ => 0.0f,
    };

    public static string GetDisplayName(this ReactionType reaction) => reaction switch
    {
        ReactionType.Bad => "Bad",
        ReactionType.Neutral => "Neutral",
        ReactionType.Good => "Good",
        _ => "Unknown",
    };
}
