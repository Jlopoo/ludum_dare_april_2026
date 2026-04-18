#nullable enable

/// <summary>
/// Looks up the expected response <see cref="Tone"/> for a given alien + cue.
///
/// Today: one-step lookup via <see cref="Species.ToneFor"/>.
/// When planetary customs land: this becomes a two-step lookup
/// (Cue → Mood via Species, then Mood → Tone via Planet) but the public API
/// stays exactly this same — callers will not need to change.
///
/// Returns <c>null</c> at any step where data is missing — callers should treat
/// that as "no expected tone for this moment" and not score it.
/// </summary>
public static class CueResolver
{
    public static Tone? Resolve(Alien? alien, Cue cue)
    {
        if (alien is null || cue == Cue.None) return null;
        return alien.Species?.ToneFor(cue);
    }

    /// <summary>Convenience overload that pulls cue + alien straight from the conversation.</summary>
    public static Tone? Resolve(Conversation? conversation, DialogueLine? line)
    {
        if (conversation is null || line is null) return null;
        return Resolve(conversation.Alien, line.Cue);
    }
}
