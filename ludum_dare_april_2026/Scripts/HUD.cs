using Godot;

/// <summary>
/// HUD — pure UI shell. Hosts the Aurameter, DialogueWindow (DialogueLog),
/// ResponseWindow, and the Options gear button. All gameplay orchestration
/// (scoring, portrait swaps, conversation starts) lives in <see cref="GameManager"/>;
/// HUD just hands its children to the engine and keeps the gear button wired.
/// </summary>
public partial class HUD : Control
{
	private Aurameter aurameter;
	private Button optionsButton;
	private Label scoreLabel;
	private GameState gameState;

	public override void _Ready()
	{
		base._Ready();
		GD.Print("HUD Node Ready");

		aurameter = GetNode<Aurameter>("Aurameter");

		optionsButton = GetNode<Button>("OptionsButton");
		optionsButton.Pressed += OnOptionsPressed;

		scoreLabel = GetNode<Label>("ScoreLabel");
		gameState = GetNode<GameState>("/root/GameState");
		gameState.PointsChanged += OnPointsChanged;
		UpdateScoreLabel(gameState.Points);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (optionsButton != null)
			optionsButton.Pressed -= OnOptionsPressed;
		if (gameState != null)
			gameState.PointsChanged -= OnPointsChanged;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		// Eat the spacebar so it doesn't accidentally re-trigger whichever response
		// button last had keyboard focus.
		if (@event is InputEventKey keyEvent && keyEvent.Keycode == Key.Space)
		{
			GetTree().Root.SetInputAsHandled();
		}
	}

	private void OnOptionsPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/options_menu.tscn");
	}

	private void OnPointsChanged(int newValue)
	{
		UpdateScoreLabel(newValue);
	}

	private void UpdateScoreLabel(int successfulDates)
	{
		scoreLabel.Text = $"Successful Dates: {successfulDates}";
	}
}
