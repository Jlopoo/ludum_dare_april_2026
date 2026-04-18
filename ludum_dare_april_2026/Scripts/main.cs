using Godot;

public partial class main : Node2D
{
	private HUD hud;
	private Button optionsButton;

	public override void _Ready()
	{
        base._Ready();

		// Setup options gear button
		optionsButton = GetNode<Button>("OptionsButton");
		if (optionsButton != null)
		{
			optionsButton.Pressed += OnOptionsPressed;
			GD.Print("Options button connected");
		}

		//Instance HUD
		var hud_Scene = GD.Load<PackedScene>("res://Scenes/HUD.tscn");
        var hud_Instance = hud_Scene.Instantiate<HUD>();
        AddChild(hud_Instance);

		StartGame();
	}

	public void StartGame()
    {
		GD.Print("StartGame() called");

		// Instantiate HUD (includes Aurameter as a child)
		PackedScene hudScene = GD.Load<PackedScene>("res://Scenes/HUD.tscn");
		hud = hudScene.Instantiate<HUD>();
		AddChild(hud);
		GD.Print("HUD instantiated and added to scene");
    }

	private void OnOptionsPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/options_menu.tscn");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
