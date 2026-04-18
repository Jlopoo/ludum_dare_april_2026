using Godot;
using LudumDareApril2026.Core;
using LudumDareApril2026.Data;
using LudumDareApril2026.UI;

namespace LudumDareApril2026.Gameplay;

/// <summary>
/// Orchestrator attached to the root of <c>date.tscn</c>. Pulls the current
/// scenario from <see cref="GameManager"/>, walks the prompt list, mediates
/// button presses, updates the <see cref="AuraMeter"/>, and transitions to the
/// summary scene when finished.
/// <para/>
/// This script holds NO dialogue content - all content lives in
/// <see cref="DateScenario"/> resources. The designer edits data, not code.
/// </summary>
public partial class DateController : Control
{
    private const float ReactionHoldSeconds = 1.4f;

    private Hud? _hud;
    private TextureRect? _alienPortrait;
    private Label? _alienNameLabel;
    private Label? _alienSpeciesLabel;
    private ColorRect? _alienBackdrop;
    private RichTextLabel? _alienDialogueText;
    private Button? _responseAButton;
    private Button? _responseBButton;
    private Button? _responseCButton;
    private Control? _reactionOverlay;
    private Label? _reactionGlyphLabel;
    private Label? _reactionDescriptionLabel;
    private ColorRect? _reactionFlash;

    private DateScenario? _scenario;
    private int _promptIndex;
    private bool _isTransitioning;

    public override void _Ready()
    {
        LookupNodes();

        _scenario = GameManager.Instance.GetCurrentDate();

        if (_scenario is null)
        {
            // Library is either missing or exhausted. If the run was actually
            // in progress, fall through to the completion screen; otherwise
            // bail to the menu so the user can set up content first.
            if (GameManager.Instance.State.CurrentDateIndex > 0)
            {
                SceneManager.Instance.GoToGameComplete();
            }
            else
            {
                GD.PushError("DateController: no DateScenario available. Author Data/date_library.tres before pressing Start Dating.");
                SceneManager.Instance.GoToMainMenu();
            }
            return;
        }

        var threshold = _scenario.SuccessThresholdOverride >= 0f
            ? _scenario.SuccessThresholdOverride
            : GameState.DefaultSuccessThreshold;

        _hud?.Configure(GameManager.Instance.State, threshold);

        WireButtons();
        RenderAlien(_scenario);
        HideReactionOverlay();
        ShowPrompt(0);
    }

    private void LookupNodes()
    {
        _hud = GetNodeOrNull<Hud>("Hud");
        _alienPortrait = GetNodeOrNull<TextureRect>("AlienArea/AlienPortrait");
        _alienNameLabel = GetNodeOrNull<Label>("AlienArea/AlienNameLabel");
        _alienSpeciesLabel = GetNodeOrNull<Label>("AlienArea/AlienSpeciesLabel");
        _alienBackdrop = GetNodeOrNull<ColorRect>("AlienBackdrop");
        _alienDialogueText = GetNodeOrNull<RichTextLabel>("DialogueBox/AlienDialogueText");
        _responseAButton = GetNodeOrNull<Button>("ResponseButtons/ResponseAButton");
        _responseBButton = GetNodeOrNull<Button>("ResponseButtons/ResponseBButton");
        _responseCButton = GetNodeOrNull<Button>("ResponseButtons/ResponseCButton");
        _reactionOverlay = GetNodeOrNull<Control>("ReactionOverlay");
        _reactionGlyphLabel = GetNodeOrNull<Label>("ReactionOverlay/ReactionGlyphLabel");
        _reactionDescriptionLabel = GetNodeOrNull<Label>("ReactionOverlay/ReactionDescriptionLabel");
        _reactionFlash = GetNodeOrNull<ColorRect>("ReactionOverlay/ReactionFlash");
    }

    private void WireButtons()
    {
        if (_responseAButton is not null) _responseAButton.Pressed += () => OnResponsePicked(0);
        if (_responseBButton is not null) _responseBButton.Pressed += () => OnResponsePicked(1);
        if (_responseCButton is not null) _responseCButton.Pressed += () => OnResponsePicked(2);
    }

    private void RenderAlien(DateScenario scenario)
    {
        if (_alienNameLabel is not null)
        {
            _alienNameLabel.Text = scenario.AlienName;
        }

        if (_alienSpeciesLabel is not null)
        {
            var species = scenario.Species;
            _alienSpeciesLabel.Text = species is null
                ? ""
                : $"{species.SpeciesName} of {species.HomePlanet}";
        }

        if (_alienPortrait is not null)
        {
            _alienPortrait.Texture = scenario.Species?.Portrait;
        }

        if (_alienBackdrop is not null && scenario.Species is not null)
        {
            _alienBackdrop.Color = scenario.Species.ThemeColor;
        }
    }

    private void ShowPrompt(int index)
    {
        if (_scenario is null) return;
        if (index < 0 || index >= _scenario.Prompts.Count)
        {
            FinishDate();
            return;
        }

        _promptIndex = index;
        var prompt = _scenario.Prompts[index];

        if (_alienDialogueText is not null)
        {
            _alienDialogueText.Text = prompt.AlienLine;
        }

        SetButton(_responseAButton, prompt.ResponseA);
        SetButton(_responseBButton, prompt.ResponseB);
        SetButton(_responseCButton, prompt.ResponseC);
        SetButtonsDisabled(false);
    }

    private static void SetButton(Button? button, ResponseOption? option)
    {
        if (button is null) return;
        if (option is null)
        {
            button.Visible = false;
            button.Text = "";
            return;
        }
        button.Visible = true;
        button.Text = option.Text;
    }

    private void SetButtonsDisabled(bool disabled)
    {
        if (_responseAButton is not null) _responseAButton.Disabled = disabled;
        if (_responseBButton is not null) _responseBButton.Disabled = disabled;
        if (_responseCButton is not null) _responseCButton.Disabled = disabled;
    }

    private async void OnResponsePicked(int optionIndex)
    {
        if (_isTransitioning || _scenario is null) return;
        var prompt = _scenario.Prompts[_promptIndex];
        var option = prompt.GetResponse(optionIndex);
        if (option is null) return;

        _isTransitioning = true;
        SetButtonsDisabled(true);

        ApplyReaction(option.Reaction);

        await ToSignal(GetTree().CreateTimer(ReactionHoldSeconds), SceneTreeTimer.SignalName.Timeout);

        HideReactionOverlay();
        _isTransitioning = false;
        ShowPrompt(_promptIndex + 1);
    }

    private void ApplyReaction(ReactionType reaction)
    {
        if (_scenario?.Species is null) return;

        _hud?.AuraMeter?.Apply(reaction);
        GameManager.Instance.RecordObservation(_scenario.Species, reaction);
        ShowReactionCue(_scenario.Species.GetCue(reaction));
    }

    private void ShowReactionCue(SignalCue? cue)
    {
        if (_reactionOverlay is not null) _reactionOverlay.Visible = true;

        if (_reactionGlyphLabel is not null)
        {
            _reactionGlyphLabel.Text = cue?.Glyph ?? "?";
        }
        if (_reactionDescriptionLabel is not null)
        {
            _reactionDescriptionLabel.Text = cue?.Description ?? "";
        }
        if (_reactionFlash is not null && cue is not null)
        {
            var color = cue.DisplayColor;
            color.A = 0.35f;
            _reactionFlash.Color = color;
        }
    }

    private void HideReactionOverlay()
    {
        if (_reactionOverlay is not null) _reactionOverlay.Visible = false;
    }

    private void FinishDate()
    {
        var finalAura = _hud?.AuraMeter?.CurrentAura ?? 0f;
        GameManager.Instance.CompleteCurrentDate(finalAura);
        SceneManager.Instance.GoToDateSummary();
    }
}
