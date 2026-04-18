using Godot;
using LudumDareApril2026.Core;
using LudumDareApril2026.Gameplay;

namespace LudumDareApril2026.UI;

/// <summary>
/// Reusable HUD component used during dates. Contains the aura meter and the
/// date/baby counters. Embedded in <c>date.tscn</c> (and optionally other
/// scenes) rather than used as a top-level scene.
/// </summary>
public partial class Hud : Control
{
    public AuraMeter? AuraMeter { get; private set; }

    private Label? _dateCounterLabel;
    private Label? _babyCounterLabel;

    public override void _Ready()
    {
        base._Ready();

        AuraMeter = GetNodeOrNull<AuraMeter>("TopRow/AuraMeter");
        _dateCounterLabel = GetNodeOrNull<Label>("TopRow/DateCounter");
        _babyCounterLabel = GetNodeOrNull<Label>("TopRow/BabyCounter");

        if (AuraMeter is null) GD.PushError("[Hud] AuraMeter node missing.");
        if (_dateCounterLabel is null) GD.PushError("[Hud] DateCounter node missing.");
        if (_babyCounterLabel is null) GD.PushError("[Hud] BabyCounter node missing.");
    }

    /// <summary>
    /// Initializes the HUD for the start of a date. Sets the labels once and
    /// configures the aura meter with the date's success threshold.
    /// </summary>
    public void Configure(GameState state, float successThreshold)
    {
        if (_dateCounterLabel is not null)
        {
            _dateCounterLabel.Text = $"Date {state.CurrentDateIndex + 1} / {GameState.TotalDates}";
        }
        if (_babyCounterLabel is not null)
        {
            _babyCounterLabel.Text = $"Babies: {state.BabiesEarned}";
        }
        if (AuraMeter is not null)
        {
            AuraMeter.SuccessThreshold = successThreshold;
            AuraMeter.Reset();
        }
    }
}
