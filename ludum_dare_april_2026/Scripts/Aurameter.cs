using Godot;

/// <summary>
/// UI display for the aura meter. Owns no game state itself — all values
/// live in <see cref="GameState"/> so they survive scene changes.
/// </summary>
public partial class Aurameter : Control
{
	private float auraChangeRate = 15f; // units per second (debug input)

	private ProgressBar auraMeterBar;
	private Label auraLabel;

	private GameState _gameState;

	public override void _Ready()
	{
		base._Ready();
		GD.Print("Aurameter Node Ready");

		_gameState = GetNode<GameState>("/root/GameState");

		auraMeterBar = GetNode<ProgressBar>("AuraMeterBar");
		auraLabel = GetNode<Label>("AuraLabel");

		auraMeterBar.MinValue = _gameState.MinAura;
		auraMeterBar.MaxValue = _gameState.MaxAura;
		auraMeterBar.CustomMinimumSize = new Vector2(200, 30);

		// Keep UI in sync whenever aura changes (even from other systems)
		_gameState.AuraChanged += OnAuraChanged;

		UpdateAuraUI(_gameState.Aura);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (_gameState != null)
			_gameState.AuraChanged -= OnAuraChanged;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		// debug // HandleAuraInput(delta);
	}

	//---DEBUG---
	// 
	// private void HandleAuraInput(double delta)
	// {
	// 	float auraChange = 0f;

	// 	if (Input.IsActionPressed("ui_up"))
	// 		auraChange += (float)delta * auraChangeRate;

	// 	if (Input.IsActionPressed("ui_down"))
	// 		auraChange -= (float)delta * auraChangeRate;

	// 	if (auraChange != 0f)
	// 		_gameState.IncreaseAura(auraChange);
	// }
	//---DEBUG---

	private void OnAuraChanged(float newValue) => UpdateAuraUI(newValue);

	private void UpdateAuraUI(float value)
	{
		auraMeterBar.Value = value;
		auraLabel.Text = $"Aura: {value:F0}/{_gameState.MaxAura:F0}";
	}

	// -- Pass-through API (so existing callers don't break) -----------------

	public void IncreaseAura(float amount) => _gameState.IncreaseAura(amount);
	public void DecreaseAura(float amount) => _gameState.DecreaseAura(amount);
	public void SetAura(float value) => _gameState.SetAura(value);
	public float GetAura() => _gameState.Aura;
	public float GetAuraNormalized() => _gameState.GetAuraNormalized();
}
