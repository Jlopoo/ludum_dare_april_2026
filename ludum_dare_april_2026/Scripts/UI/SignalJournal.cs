using Godot;
using LudumDareApril2026.Core;
using LudumDareApril2026.Data;

namespace LudumDareApril2026.UI;

/// <summary>
/// Shows every species the player has encountered in the current run plus
/// which of their signal cues the player has observed. This is the main
/// learning UI - it builds up as the player plays more dates.
/// </summary>
public partial class SignalJournal : Control
{
    private VBoxContainer? _speciesListContainer;
    private Label? _emptyStateLabel;
    private Button? _backButton;

    public override void _Ready()
    {
        base._Ready();

        _speciesListContainer = GetNodeOrNull<VBoxContainer>("Margin/Layout/ScrollContainer/SpeciesListContainer");
        _emptyStateLabel = GetNodeOrNull<Label>("Margin/Layout/EmptyStateLabel");
        _backButton = GetNodeOrNull<Button>("Margin/Layout/BackButton");

        if (_backButton is not null)
        {
            _backButton.Pressed += OnBackPressed;
        }

        Populate();
    }

    private void Populate()
    {
        if (_speciesListContainer is null) return;

        foreach (var child in _speciesListContainer.GetChildren())
        {
            child.QueueFree();
        }

        var state = GameManager.Instance.State;
        var hasAny = state.KnownSignals.Count > 0;

        if (_emptyStateLabel is not null)
        {
            _emptyStateLabel.Visible = !hasAny;
        }

        foreach (var (_, knowledge) in state.KnownSignals)
        {
            _speciesListContainer.AddChild(BuildSpeciesCard(knowledge));
        }
    }

    private static Control BuildSpeciesCard(SpeciesKnowledge knowledge)
    {
        var species = knowledge.Species;

        var card = new VBoxContainer
        {
            Name = $"Species_{species.SpeciesId}",
        };

        var title = new Label
        {
            Text = $"{species.SpeciesName} - {species.HomePlanet}",
        };
        card.AddChild(title);

        card.AddChild(BuildCueRow(ReactionType.Good, species, knowledge));
        card.AddChild(BuildCueRow(ReactionType.Neutral, species, knowledge));
        card.AddChild(BuildCueRow(ReactionType.Bad, species, knowledge));

        return card;
    }

    private static Control BuildCueRow(ReactionType reaction, AlienSpecies species, SpeciesKnowledge knowledge)
    {
        var observed = knowledge.HasObserved(reaction);
        var cue = species.GetCue(reaction);

        var row = new HBoxContainer();

        var label = new Label
        {
            Text = observed
                ? $"{reaction.GetDisplayName()}: {cue?.Glyph} — {cue?.Description}  (seen {knowledge.GetObservations(reaction)}x)"
                : $"{reaction.GetDisplayName()}: ???",
        };
        row.AddChild(label);
        return row;
    }

    private void OnBackPressed()
    {
        SceneManager.Instance.GoToMainMenu();
    }
}
