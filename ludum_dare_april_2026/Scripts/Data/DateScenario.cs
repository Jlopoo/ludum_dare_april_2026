using Godot;
using Godot.Collections;

namespace LudumDareApril2026.Data;

/// <summary>
/// A single speed-dating encounter: one alien plus the sequence of prompts
/// they'll say to the player.
/// </summary>
[GlobalClass]
public partial class DateScenario : Resource
{
    /// <summary>Individual name of this alien (e.g. "Vorbax"), separate from species name.</summary>
    [Export] public string AlienName { get; set; } = "";

    [Export] public AlienSpecies? Species { get; set; }

    [Export] public Array<DialoguePrompt> Prompts { get; set; } = new();

    /// <summary>Text shown before the date starts (optional).</summary>
    [Export(PropertyHint.MultilineText)] public string IntroLine { get; set; } = "";

    /// <summary>Text shown when the date succeeds (optional).</summary>
    [Export(PropertyHint.MultilineText)] public string SuccessLine { get; set; } = "";

    /// <summary>Text shown when the date fails (optional).</summary>
    [Export(PropertyHint.MultilineText)] public string FailureLine { get; set; } = "";

    /// <summary>
    /// Per-date override for success threshold. Negative means "use the game-wide
    /// default" (see <c>GameState.DefaultSuccessThreshold</c>).
    /// </summary>
    [Export(PropertyHint.Range, "-1.0,1.0,0.05")]
    public float SuccessThresholdOverride { get; set; } = -1f;
}
