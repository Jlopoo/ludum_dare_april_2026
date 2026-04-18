using Godot;

public partial class main : Node2D
{
	private HUD hud;
	
	public override void _Ready()
	{
        base._Ready();
        hud = GetNode<HUD>("HUD");

        // Hide HUD at start — show it when gameplay begins
        hud.Visible = false;
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

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
