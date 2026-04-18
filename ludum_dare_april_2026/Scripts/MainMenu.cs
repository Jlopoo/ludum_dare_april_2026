using Godot;
using LudumDareApril2026.Core;

namespace LudumDareApril2026.UI;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        base._Ready();
        GD.Print("[MainMenu] _Ready");

        WireButton("CenterContainer/ButtonColumn/StartButton", OnStartPressed);
        WireButton("CenterContainer/ButtonColumn/SignalJournalButton", OnSignalJournalPressed);
        WireButton("CenterContainer/ButtonColumn/OptionsButton", OnOptionsPressed);
        WireButton("CenterContainer/ButtonColumn/QuitButton", OnQuitPressed);
    }

    private void WireButton(string path, System.Action handler)
    {
        var button = GetNodeOrNull<Button>(path);
        if (button is null)
        {
            GD.PushError($"[MainMenu] Missing Button at '{path}'. Check the scene.");
            return;
        }
        button.Pressed += handler;
    }

    private void OnStartPressed()
    {
        GD.Print("[MainMenu] Start pressed");
        GameManager.Instance.StartNewGame();
        SceneManager.Instance.GoToDate();
    }

    private void OnSignalJournalPressed()
    {
        GD.Print("[MainMenu] Signal Journal pressed");
        SceneManager.Instance.GoToSignalJournal();
    }

    private void OnOptionsPressed()
    {
        GD.Print("[MainMenu] Options pressed");
        SceneManager.Instance.GoToOptions();
    }

    private void OnQuitPressed()
    {
        GD.Print("[MainMenu] Quit pressed");
        SceneManager.Instance.QuitGame();
    }
}
