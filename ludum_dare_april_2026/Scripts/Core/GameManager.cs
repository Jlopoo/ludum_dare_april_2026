using System;
using Godot;
using LudumDareApril2026.Data;

namespace LudumDareApril2026.Core;

/// <summary>
/// Autoload singleton that owns the run-level <see cref="GameState"/> and the
/// <see cref="DateLibrary"/>. Other scenes ask this for "what date am I on?"
/// and push results back when a date finishes.
/// </summary>
public partial class GameManager : Node
{
    public const string DateLibraryPath = "res://Data/date_library.tres";

    private static GameManager? _instance;
    public static GameManager Instance =>
        _instance ?? throw new InvalidOperationException(
            "GameManager autoload not initialized. Check project.godot autoload config.");

    public GameState State { get; private set; } = new();

    public DateLibrary? Library { get; private set; }

    /// <summary>True when another scenario is available in the library.</summary>
    public bool HasMoreDates =>
        Library is not null && State.CurrentDateIndex < Library.Count;

    /// <summary>
    /// True when the run is over, either because the player reached the
    /// design-target date count or because the library ran out of scenarios.
    /// </summary>
    public bool IsRunComplete =>
        State.CurrentDateIndex >= GameState.TotalDates || !HasMoreDates;

    /// <summary>Fired when <see cref="StartNewGame"/> is called, so UIs can refresh.</summary>
    [Signal] public delegate void NewGameStartedEventHandler();

    /// <summary>Fired after <see cref="CompleteCurrentDate"/> with the latest <see cref="DateResult"/>.</summary>
    [Signal] public delegate void DateCompletedEventHandler();

    public override void _EnterTree()
    {
        if (_instance is not null && _instance != this)
        {
            QueueFree();
            return;
        }
        _instance = this;
    }

    public override void _Ready()
    {
        LoadLibrary();
    }

    public override void _ExitTree()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void LoadLibrary()
    {
        if (!ResourceLoader.Exists(DateLibraryPath))
        {
            GD.PushWarning($"GameManager: {DateLibraryPath} not found. Dates will not load until the library is authored.");
            return;
        }

        Library = ResourceLoader.Load<DateLibrary>(DateLibraryPath);
        if (Library is null)
        {
            GD.PushError($"GameManager: failed to load {DateLibraryPath} as DateLibrary.");
        }
    }

    public void StartNewGame()
    {
        State.Reset();
        EmitSignal(SignalName.NewGameStarted);
    }

    /// <summary>Returns the <see cref="DateScenario"/> for the current index, or null if none remains.</summary>
    public DateScenario? GetCurrentDate()
    {
        if (Library is null) return null;
        return Library.GetDate(State.CurrentDateIndex);
    }

    public void CompleteCurrentDate(float finalAura)
    {
        var scenario = GetCurrentDate();
        if (scenario is null)
        {
            GD.PushError("CompleteCurrentDate called but no current scenario is available.");
            return;
        }

        var threshold = scenario.SuccessThresholdOverride >= 0f
            ? scenario.SuccessThresholdOverride
            : GameState.DefaultSuccessThreshold;

        var result = new DateResult(
            Scenario: scenario,
            FinalAura: finalAura,
            SuccessThreshold: threshold,
            DateIndex: State.CurrentDateIndex);

        State.CompleteDate(result);
        EmitSignal(SignalName.DateCompleted);
    }

    public void RecordObservation(AlienSpecies species, ReactionType reaction)
    {
        State.RecordObservation(species, reaction);
    }
}
