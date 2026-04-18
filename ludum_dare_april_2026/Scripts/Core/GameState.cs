using System.Collections.Generic;
using LudumDareApril2026.Data;

namespace LudumDareApril2026.Core;

/// <summary>
/// Pure-data snapshot of the player's run. Lives inside <see cref="GameManager"/>
/// and is intentionally independent of Godot's Node system so it's easy to
/// reason about, serialize later, and unit test.
/// </summary>
public sealed class GameState
{
    public const int TotalDates = 9;
    public const float DefaultSuccessThreshold = 0.80f;

    /// <summary>Index of the next date to play. Ranges from 0 to <see cref="TotalDates"/>.</summary>
    public int CurrentDateIndex { get; private set; }

    public int BabiesEarned { get; private set; }

    /// <summary>
    /// What the player has observed, keyed by <see cref="AlienSpecies.SpeciesId"/>.
    /// Persists across dates so the signal journal accumulates knowledge.
    /// </summary>
    public IReadOnlyDictionary<string, SpeciesKnowledge> KnownSignals => _knownSignals;

    /// <summary>Most recent completed date, used by the summary screen.</summary>
    public DateResult? LastDateResult { get; private set; }

    private readonly Dictionary<string, SpeciesKnowledge> _knownSignals = new();

    public bool IsGameComplete => CurrentDateIndex >= TotalDates;

    public void Reset()
    {
        CurrentDateIndex = 0;
        BabiesEarned = 0;
        LastDateResult = null;
        _knownSignals.Clear();
    }

    /// <summary>
    /// Records that the player saw <paramref name="species"/> react with
    /// <paramref name="reaction"/>. The cue shown is implicit in the species
    /// definition; we only need to track which reactions the player has now
    /// seen for which species.
    /// </summary>
    public void RecordObservation(AlienSpecies species, ReactionType reaction)
    {
        var id = string.IsNullOrEmpty(species.SpeciesId) ? species.SpeciesName : species.SpeciesId;
        if (string.IsNullOrEmpty(id))
        {
            return;
        }

        if (!_knownSignals.TryGetValue(id, out var knowledge))
        {
            knowledge = new SpeciesKnowledge(species);
            _knownSignals[id] = knowledge;
        }
        knowledge.MarkObserved(reaction);
    }

    public void CompleteDate(DateResult result)
    {
        LastDateResult = result;
        if (result.Success)
        {
            BabiesEarned++;
        }
        CurrentDateIndex++;
    }
}

/// <summary>Outcome of a single completed date, surfaced to the summary screen.</summary>
public sealed record DateResult(
    DateScenario Scenario,
    float FinalAura,
    float SuccessThreshold,
    int DateIndex
)
{
    public bool Success => FinalAura >= SuccessThreshold;
}

/// <summary>
/// Running tally of what the player has witnessed from a given species across
/// the run. The UI uses this to populate the signal journal.
/// </summary>
public sealed class SpeciesKnowledge
{
    public AlienSpecies Species { get; }

    public int GoodObservations { get; private set; }
    public int NeutralObservations { get; private set; }
    public int BadObservations { get; private set; }

    public SpeciesKnowledge(AlienSpecies species)
    {
        Species = species;
    }

    public int TotalObservations => GoodObservations + NeutralObservations + BadObservations;

    public bool HasObserved(ReactionType reaction) => reaction switch
    {
        ReactionType.Good => GoodObservations > 0,
        ReactionType.Neutral => NeutralObservations > 0,
        ReactionType.Bad => BadObservations > 0,
        _ => false,
    };

    public int GetObservations(ReactionType reaction) => reaction switch
    {
        ReactionType.Good => GoodObservations,
        ReactionType.Neutral => NeutralObservations,
        ReactionType.Bad => BadObservations,
        _ => 0,
    };

    public void MarkObserved(ReactionType reaction)
    {
        switch (reaction)
        {
            case ReactionType.Good: GoodObservations++; break;
            case ReactionType.Neutral: NeutralObservations++; break;
            case ReactionType.Bad: BadObservations++; break;
        }
    }
}
