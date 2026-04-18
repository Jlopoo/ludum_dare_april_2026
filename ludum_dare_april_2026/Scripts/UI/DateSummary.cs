using Godot;
using LudumDareApril2026.Core;

namespace LudumDareApril2026.UI;

/// <summary>
/// Shown immediately after a date finishes. Reads <see cref="GameState.LastDateResult"/>
/// and routes the player onward: next date, game complete, or back to menu.
/// </summary>
public partial class DateSummary : Control
{
    private Label? _headlineLabel;
    private Label? _scoreLabel;
    private Label? _thresholdLabel;
    private Label? _babiesLabel;
    private RichTextLabel? _flavorTextLabel;
    private Button? _continueButton;

    public override void _Ready()
    {
        base._Ready();

        _headlineLabel = GetNodeOrNull<Label>("Center/Column/HeadlineLabel");
        _scoreLabel = GetNodeOrNull<Label>("Center/Column/ScoreLabel");
        _thresholdLabel = GetNodeOrNull<Label>("Center/Column/ThresholdLabel");
        _babiesLabel = GetNodeOrNull<Label>("Center/Column/BabiesLabel");
        _flavorTextLabel = GetNodeOrNull<RichTextLabel>("Center/Column/FlavorTextLabel");
        _continueButton = GetNodeOrNull<Button>("Center/Column/ContinueButton");

        var state = GameManager.Instance.State;
        var result = state.LastDateResult;

        if (result is null)
        {
            GD.PushError("DateSummary: no LastDateResult set on GameState.");
            SceneManager.Instance.GoToMainMenu();
            return;
        }

        var success = result.Success;

        if (_headlineLabel is not null)
        {
            _headlineLabel.Text = success ? "A Match!" : "No Spark...";
        }

        if (_scoreLabel is not null)
        {
            _scoreLabel.Text = $"Final Aura: {Mathf.RoundToInt(result.FinalAura * 100f)}%";
        }

        if (_thresholdLabel is not null)
        {
            _thresholdLabel.Text = $"Needed: {Mathf.RoundToInt(result.SuccessThreshold * 100f)}%";
        }

        if (_babiesLabel is not null)
        {
            _babiesLabel.Text = $"Babies: {state.BabiesEarned}";
        }

        if (_flavorTextLabel is not null)
        {
            _flavorTextLabel.Text = success
                ? result.Scenario.SuccessLine
                : result.Scenario.FailureLine;
        }

        if (_continueButton is not null)
        {
            _continueButton.Text = GameManager.Instance.IsRunComplete ? "See Results" : "Next Date";
            _continueButton.Pressed += OnContinuePressed;
        }
    }

    private void OnContinuePressed()
    {
        if (GameManager.Instance.IsRunComplete)
        {
            SceneManager.Instance.GoToGameComplete();
        }
        else
        {
            SceneManager.Instance.GoToDate();
        }
    }
}
