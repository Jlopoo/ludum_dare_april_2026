using Godot;

public partial class main : Node2D
{
	private HUD hud;

	public override void _Ready()
	{
        base._Ready();

		StartGame();
	}

	public void StartGame()
    {
		GD.Print("StartGame() called");

		PackedScene hudScene = GD.Load<PackedScene>("res://Scenes/HUD.tscn");
		hud = hudScene.Instantiate<HUD>();
		AddChild(hud);
		GD.Print("HUD instantiated and added to scene");
    }

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
