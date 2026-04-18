using Godot;
using System;

public partial class main_menu : Control
{
    private Button startButton;
    private Button optionsButton;
    private Button exitButton;

    public override void _Ready()
    {
        base._Ready();
        GD.Print("MainMenu Node Ready");

        startButton   = GetNode<Button>("CenterContainer/StartButton");
        optionsButton = GetNode<Button>("CenterContainer/OptionsButton");
        exitButton    = GetNode<Button>("CenterContainer/ExitButton");

        startButton.Pressed   += OnStartPressed;
        optionsButton.Pressed += OnOptionsPressed;
        exitButton.Pressed    += OnExitPressed;
    }

    private void OnStartPressed()
    {
        GetNode<GameState>("/root/GameState").SetAura(67f);
        GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
    }

    private void OnOptionsPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/options_menu.tscn");
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
