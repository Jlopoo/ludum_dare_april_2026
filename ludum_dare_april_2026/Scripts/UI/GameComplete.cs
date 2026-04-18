using Godot;
using LudumDareApril2026.Core;

namespace LudumDareApril2026.UI;

/// <summary>
/// Shown after all dates have been played. Summarizes the player's run.
/// </summary>
public partial class GameComplete : Control
{
    public override void _Ready()
    {
        base._Ready();

        var headlineLabel = GetNodeOrNull<Label>("Center/Column/HeadlineLabel");
        var babiesLabel = GetNodeOrNull<Label>("Center/Column/BabiesLabel");
        var signalJournalButton = GetNodeOrNull<Button>("Center/Column/SignalJournalButton");
        var mainMenuButton = GetNodeOrNull<Button>("Center/Column/MainMenuButton");

        var state = GameManager.Instance.State;

        if (headlineLabel is not null)
        {
            headlineLabel.Text = state.BabiesEarned switch
            {
                0 => "Forever Alone",
                1 => "You found love... once.",
                GameState.TotalDates => "Cosmic Casanova!",
                _ => "Not bad, space romantic.",
            };
        }

        if (babiesLabel is not null)
        {
            babiesLabel.Text = $"Babies: {state.BabiesEarned} / {GameState.TotalDates}";
        }

        if (signalJournalButton is not null)
        {
            signalJournalButton.Pressed += () => SceneManager.Instance.GoToSignalJournal();
        }

        if (mainMenuButton is not null)
        {
            mainMenuButton.Pressed += () => SceneManager.Instance.GoToMainMenu();
        }
    }
}
