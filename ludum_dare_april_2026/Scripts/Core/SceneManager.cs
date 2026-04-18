using System;
using Godot;

namespace LudumDareApril2026.Core;

/// <summary>
/// Centralized scene transitions. Keeps scene path constants in one place so
/// renames and reorganizations don't ripple across the codebase.
/// </summary>
public partial class SceneManager : Node
{
    public const string MainMenuScene = "res://Scenes/main_menu.tscn";
    public const string OptionsMenuScene = "res://Scenes/options_menu.tscn";
    public const string DateScene = "res://Scenes/date.tscn";
    public const string DateSummaryScene = "res://Scenes/date_summary.tscn";
    public const string GameCompleteScene = "res://Scenes/game_complete.tscn";
    public const string SignalJournalScene = "res://Scenes/signal_journal.tscn";

    private static SceneManager? _instance;
    public static SceneManager Instance =>
        _instance ?? throw new InvalidOperationException(
            "SceneManager autoload not initialized. Check project.godot autoload config.");

    public override void _EnterTree()
    {
        if (_instance is not null && _instance != this)
        {
            QueueFree();
            return;
        }
        _instance = this;
    }

    public override void _ExitTree()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void GoToMainMenu() => ChangeToScene(MainMenuScene);
    public void GoToOptions() => ChangeToScene(OptionsMenuScene);
    public void GoToDate() => ChangeToScene(DateScene);
    public void GoToDateSummary() => ChangeToScene(DateSummaryScene);
    public void GoToGameComplete() => ChangeToScene(GameCompleteScene);
    public void GoToSignalJournal() => ChangeToScene(SignalJournalScene);

    public void ChangeToScene(string path)
    {
        if (!ResourceLoader.Exists(path))
        {
            GD.PushError($"SceneManager: scene '{path}' does not exist.");
            return;
        }

        var error = GetTree().ChangeSceneToFile(path);
        if (error != Error.Ok)
        {
            GD.PushError($"SceneManager: failed to change to '{path}' ({error}).");
        }
    }

    public void QuitGame()
    {
        GetTree().Quit();
    }
}
