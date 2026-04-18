using Godot;

public partial class main : Node2D
{
	private HUD hud;
	private Button optionsButton;
	
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
	}

	public void StartGame()
    {
        hud.Visible = true;

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
