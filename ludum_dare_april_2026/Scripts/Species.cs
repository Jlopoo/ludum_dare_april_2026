using Godot;
using Godot.Collections;
using SysDict = System.Collections.Generic.Dictionary<Cue, Tone>;

#nullable enable

/// <summary>
/// A biological species — defines what response <see cref="Tone"/> each of its cues
/// is asking for. Today this is a direct <c>Cue → Tone</c> mapping; when planetary
/// customs are added it'll split into <c>Cue → Mood</c> (here) plus <c>Mood → Tone</c>
/// (a new Planet resource), and <see cref="CueResolver"/> will chain them. Callers
/// don't need to change.
/// </summary>
[GlobalClass]
public partial class Species : Resource
{
    [Export] public string Name { get; set; } = "";

    [Export(PropertyHint.MultilineText)] public string Description { get; set; } = "";

    /// <summary>
    /// Portrait shown on the character display when no cue is active (conversation
    /// start, lines without a cue, etc.). Optional — when null, GameManager simply
    /// leaves whatever portrait is currently shown.
    /// </summary>
    [Export] public Texture2D? DefaultPortrait { get; set; }

    [Export] public Array<CueToneEntry> CueTones { get; set; } = new();

    private SysDict? _cache;

    /// <summary>What response tone does <paramref name="cue"/> demand for this species?</summary>
    public Tone? ToneFor(Cue cue)
    {
        if (cue == Cue.None) return null;
        _cache ??= BuildCache();
        return _cache.TryGetValue(cue, out Tone tone) ? tone : null;
    }

    /// <summary>
    /// Portrait the alien should show while expressing <paramref name="cue"/>, if any.
    /// Returns null if the cue has no image attached or if the cue isn't in this species' table.
    /// </summary>
    public Texture2D? ImageFor(Cue cue)
    {
        if (cue == Cue.None) return null;
        foreach (CueToneEntry entry in CueTones)
        {
            if (entry is null) continue;
            if (entry.Cue == cue) return entry.CueImage;
        }
        return null;
    }

    private SysDict BuildCache()
    {
        SysDict dict = new(CueTones.Count);
        foreach (CueToneEntry entry in CueTones)
        {
            if (entry is null || entry.Cue == Cue.None) continue;
            dict[entry.Cue] = entry.Tone;
        }
        return dict;
    }
}
