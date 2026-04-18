#nullable enable

/// <summary>
/// Pure-function scoring rule for cue reading. Lives in its own file so the rule is
/// trivial to find and tune as the design evolves. <see cref="GameState"/> calls this
/// when a <see cref="DialogueChoice"/> is selected:
///
/// <code>int delta = ToneScoring.AuraDelta(choice.Tone, expected);</code>
///
/// Rule (RPS-ish):
/// <list type="bullet">
///   <item><description>Picked tone matches expected → <see cref="CorrectMatch"/> (+5)</description></item>
///   <item><description>Picked tone is the *opposite* of expected (Affectionate ↔ Aggressive) → <see cref="OppositeMismatch"/> (−10)</description></item>
///   <item><description>Any other mismatch (typically anything involving Intellectual) → <see cref="NeutralMismatch"/> (−5)</description></item>
///   <item><description>Either side is <see cref="Tone.None"/> (choice or line opted out of the puzzle) → 0</description></item>
/// </list>
/// </summary>
public static class ToneScoring
{
    public const int CorrectMatch = +5;
    public const int OppositeMismatch = -10;
    public const int NeutralMismatch = -5;

    public static int AuraDelta(Tone picked, Tone expected)
    {
        if (picked == Tone.None || expected == Tone.None) return 0;
        if (picked == expected) return CorrectMatch;
        return IsOpposite(picked, expected) ? OppositeMismatch : NeutralMismatch;
    }

    private static bool IsOpposite(Tone a, Tone b) =>
        (a == Tone.Affectionate && b == Tone.Aggressive) ||
        (a == Tone.Aggressive && b == Tone.Affectionate);
}
