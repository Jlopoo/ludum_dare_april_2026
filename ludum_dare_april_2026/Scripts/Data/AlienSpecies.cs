using Godot;

namespace LudumDareApril2026.Data;

/// <summary>
/// Reusable definition for an alien species. Individual dates reference a
/// species via <see cref="DateScenario.Species"/>; the species owns the signal
/// cues that the player learns to read across multiple dates.
/// </summary>
[GlobalClass]
public partial class AlienSpecies : Resource
{
    /// <summary>
    /// Stable identifier. Used as the key in <c>GameState.KnownSignals</c> so
    /// signal knowledge persists across dates with the same species.
    /// </summary>
    [Export] public string SpeciesId { get; set; } = "";

    [Export] public string SpeciesName { get; set; } = "";
    [Export] public string HomePlanet { get; set; } = "";

    [Export] public Texture2D? Portrait { get; set; }

    /// <summary>UI accent color used for speech bubbles, name plates, etc.</summary>
    [Export] public Color ThemeColor { get; set; } = new(0.6f, 0.7f, 1.0f, 1f);

    [Export(PropertyHint.MultilineText)] public string Description { get; set; } = "";

    [Export] public SignalCue? GoodSignal { get; set; }
    [Export] public SignalCue? NeutralSignal { get; set; }
    [Export] public SignalCue? BadSignal { get; set; }

    /// <summary>Returns the cue this species emits for the given reaction, or null if not configured.</summary>
    public SignalCue? GetCue(ReactionType reaction) => reaction switch
    {
        ReactionType.Good => GoodSignal,
        ReactionType.Neutral => NeutralSignal,
        ReactionType.Bad => BadSignal,
        _ => null,
    };
}
