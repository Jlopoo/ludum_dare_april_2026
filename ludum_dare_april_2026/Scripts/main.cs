using Godot;

public partial class main : Node2D
{
	private HUD hud;
	private Button optionsButton;
	private Aurameter aurameter;
	
	public override void _Ready()
	{
        base._Ready();
        
        // Try to get HUD if it exists
        if (HasNode("HUD"))
        {
            hud = GetNode<HUD>("HUD");
            hud.Visible = false;
        }

		// Setup options gear button
		optionsButton = GetNode<Button>("OptionsButton");
		if (optionsButton != null)
		{
			optionsButton.Pressed += OnOptionsPressed;
			GD.Print("Options button connected");
		}

		// Start the game
		StartGame();
	}

	public void StartGame()
    {
		GD.Print("StartGame() called");
		
		if (hud != null)
		{
			hud.Visible = true;
			GD.Print("HUD made visible");
		}
		else
		{
			GD.Print("WARNING: HUD is null");
		}

		// Instantiate and add the Aurameter scene
		try
		{
			PackedScene auraMeterScene = GD.Load<PackedScene>("res://Scenes/Aurameter.tscn");
			if (auraMeterScene == null)
			{
				GD.Print("ERROR: Failed to load Aurameter.tscn");
				return;
			}
			
			aurameter = auraMeterScene.Instantiate<Aurameter>();
			if (aurameter == null)
			{
				GD.Print("ERROR: Failed to instantiate Aurameter");
				return;
			}
			
			AddChild(aurameter);
			GD.Print("Aurameter instantiated and added to scene");
		}
		catch (System.Exception ex)
		{
			GD.Print($"ERROR in StartGame: {ex.Message}");
		}

        // Optionally kick off the first conversation immediately
        // hud.ShowConversation(
        //     "Hello! Nice to meet you.",
        //     "Nice to meet you too!",
        //     "Who are you?",
        //     "..."
        // );
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
