using Godot;
using System;

public partial class options_menu : Control
{
    private Button backButton;
	private Button mainMenuButton;

    public override void _Ready()
    {
        base._Ready();
        GD.Print("OptionsMenu Node Ready");

        backButton = GetNode<Button>("CenterContainer/BackButton");
        backButton.Pressed += OnBackPressed;

		mainMenuButton = GetNode<Button>("CenterContainer/MainMenuButton");
		mainMenuButton.Pressed += OnMainMenuPressed;
    }

    private void OnBackPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
    }

	private void OnMainMenuPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
	}
}
