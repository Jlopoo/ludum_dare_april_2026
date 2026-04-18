using Godot;

public partial class HUD : Control
{
	// HUD child nodes
	private Aurameter aurameter;
	private Control optionsButtons;
	private Button optionButton;

	public override void _Ready()
	{
		base._Ready();
		GD.Print("HUD Node Ready");

		// --- Aurameter ---
		aurameter = GetNode<Aurameter>("Aurameter");

		// --- OptionsButtons ---
		optionsButtons = GetNode<Control>("OptionsButtons");
		optionButton = optionsButtons.GetNode<Button>("OpenOptions");

		optionButton.Pressed += () => OnOptionPressed(0);

		// --- Dialogue Window ---
		var dw_Scene = GD.Load<PackedScene>("res://Scenes/DialogueWindow.tscn");
		var dw_Instance = dw_Scene.Instantiate<Control>();
		AddChild(dw_Instance);

		DialogueLog.Instance.AddEntry("Alice", "We meed again.");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	/// Called when a dialogue option button is pressed (override or connect signal as needed).
	private void OnOptionPressed(int index)
	{
		GD.Print($"Option {index + 1} selected");
		// TODO: hook into your dialogue / game logic here
	}
}
