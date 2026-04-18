using Godot;
using LudumDareApril2026.Core;
using LudumDareApril2026.Data;

namespace LudumDareApril2026.Gameplay;

/// <summary>
/// ProgressBar-backed aura tracker. Owns the running score and animates its
/// own <see cref="Range.Value"/>.
/// <para/>
/// Scoring rule: <c>CurrentAura = sum(response_scores) / responses_given</c>.
/// With scores Bad=0, Neutral=0.5, Good=1.0, this naturally produces a
/// percentage-of-perfect reading throughout the date.
/// <para/>
/// The script looks up <c>PercentLabel</c> and <c>ThresholdMarker</c> as
/// children of itself (by name), so the <c>HUD.tscn</c> layout stays simple
/// and self-contained.
/// </summary>
public partial class AuraMeter : ProgressBar
{
    /// <summary>How long the bar takes to catch up to a new value.</summary>
    [Export] public float TweenDuration { get; set; } = 0.35f;

    [Export(PropertyHint.Range, "0.0,1.0,0.05")]
    public float SuccessThreshold { get; set; } = GameState.DefaultSuccessThreshold;

    private Label? _displayLabel;
    private Control? _thresholdMarker;
    private float _totalScore;
    private int _responseCount;
    private Tween? _tween;

    public int ResponseCount => _responseCount;

    /// <summary>
    /// Current running average in 0..1. Returns 0.5 when no responses given
    /// (so the bar starts in a neutral, undefined position).
    /// </summary>
    public float CurrentAura => _responseCount == 0
        ? 0.5f
        : _totalScore / _responseCount;

    public bool MeetsThreshold => CurrentAura >= SuccessThreshold;

    public override void _Ready()
    {
        MinValue = 0;
        MaxValue = 1;
        Step = 0.001;
        Value = CurrentAura;

        _displayLabel = GetNodeOrNull<Label>("PercentLabel");
        _thresholdMarker = GetNodeOrNull<Control>("ThresholdMarker");

        PositionThresholdMarker();
        UpdateLabel();
    }

    public void Reset()
    {
        _totalScore = 0f;
        _responseCount = 0;
        AnimateTo(CurrentAura);
        UpdateLabel();
        PositionThresholdMarker();
    }

    /// <summary>Applies a reaction's score contribution and animates the bar.</summary>
    public void Apply(ReactionType reaction)
    {
        _totalScore += reaction.GetScore();
        _responseCount++;
        AnimateTo(CurrentAura);
        UpdateLabel();
    }

    private void AnimateTo(float target)
    {
        _tween?.Kill();
        _tween = CreateTween();
        _tween.TweenProperty(this, "value", target, TweenDuration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    private void UpdateLabel()
    {
        if (_displayLabel is null) return;
        _displayLabel.Text = $"{Mathf.RoundToInt(CurrentAura * 100f)}%";
    }

    private void PositionThresholdMarker()
    {
        if (_thresholdMarker is null) return;
        _thresholdMarker.AnchorLeft = SuccessThreshold;
        _thresholdMarker.AnchorRight = SuccessThreshold;
    }
}
