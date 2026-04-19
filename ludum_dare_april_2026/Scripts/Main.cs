using Godot;

public partial class Main : Node2D
{
	private HUD hud;
	private TextureRect characterDisplay;

	public override void _Ready()
	{
        base._Ready();

		StartGame();
	}

	public void StartGame()
    {
		GD.Print("StartGame() called");

		characterDisplay = GetNode<TextureRect>("CharacterDisplay");

		// Listen for portrait swap requests from GameState
		var gameState = GetNode<GameState>("/root/GameState");
		gameState.CharacterChangeRequested += SetCharacter;

		PackedScene hudScene = GD.Load<PackedScene>("res://Scenes/HUD.tscn");
		hud = hudScene.Instantiate<HUD>();
		AddChild(hud);
		GD.Print("HUD instantiated and added to scene");
    }

	public override void _ExitTree()
	{
		base._ExitTree();
		var gameState = GetNode<GameState>("/root/GameState");
		if (gameState != null)
			gameState.CharacterChangeRequested -= SetCharacter;
	}

	/// Swaps the character portrait. Texture is already loaded — caller provides the resource.
	public void SetCharacter(Texture2D texture)
	{
		if (characterDisplay != null)
			characterDisplay.Texture = texture;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
