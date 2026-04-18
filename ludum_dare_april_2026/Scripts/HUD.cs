using Godot;

public partial class HUD : Control
{
	// HUD child nodes
	private Aurameter aurameter;
	private Control optionsButtons;
	private Button optionButton;
	private ConversationManager conversationManager;
	private GameState _gameState;

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

		// --- GameState and Conversation Wiring ---
		_gameState = GetNode<GameState>("/root/GameState");

		// Find ConversationManager in the scene tree (it's a sibling or ancestor)
		var root = GetTree().Root;
		conversationManager = root.FindChild("ConversationManager", true, false) as ConversationManager;
		if (conversationManager != null)
		{
			conversationManager.ChoiceSelected += OnChoiceSelected;
			GD.Print("HUD connected to ConversationManager");
		}
		else
		{
			GD.PrintErr("HUD could not find ConversationManager in scene tree");
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (conversationManager != null)
			conversationManager.ChoiceSelected -= OnChoiceSelected;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	private void OnChoiceSelected(DialogueChoice choice)
	{
		if (choice.AffectionDelta != 0)
		{
			_gameState.ApplyAffectionDelta(choice.AffectionDelta);
			GD.Print($"Applied affection delta: {choice.AffectionDelta}, aura now: {_gameState.Aura}");
		}
	}

	/// Called when a dialogue option button is pressed (override or connect signal as needed).
	private void OnOptionPressed(int index)
	{
		GD.Print($"Option {index + 1} selected");
		// TODO: hook into your dialogue / game logic here
	}
}
