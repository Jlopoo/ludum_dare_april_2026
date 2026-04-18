using Godot;

public partial class HUD : Control
{
    private float aurameter = 50f;
    private float maxAurameter = 100f;
    private float minAurameter = 0f;
    private float auraChangeRate = 15f; // Aura change per second

    // HUD child nodes
    private Control auraMeterNode;
    private ProgressBar auraMeterBar;
    private Label auraLabel;

    private Control optionsButtons;
    private Button optionButton;
    private Button option2Button;
    private Button option3Button;

    public override void _Ready()
    {
        base._Ready();
        GD.Print("HUD Node Ready");

        // --- AuraMeter ---
        auraMeterNode = GetNode<Control>("AuraMeter");
        auraMeterBar = auraMeterNode.GetNode<ProgressBar>("AuraMeterBar");
        auraLabel = auraMeterNode.GetNode<Label>("AuraLabel");

        auraMeterBar.MinValue = minAurameter;
        auraMeterBar.MaxValue = maxAurameter;
        auraMeterBar.Value = aurameter;
        auraMeterBar.CustomMinimumSize = new Vector2(200, 30);

        // --- OptionsButtons ---
        optionsButtons = GetNode<Control>("OptionsButtons");
        optionButton = optionsButtons.GetNode<Button>("Option1Button");

        optionButton.Pressed += () => OnOptionPressed(0);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        HandleAuraInput(delta);
        UpdateAuraUI();
    }

    private void HandleAuraInput(double delta)
    {
        float auraChange = 0f;

        if (Input.IsActionPressed("ui_up"))
            auraChange += (float)delta * auraChangeRate;

        if (Input.IsActionPressed("ui_down"))
            auraChange -= (float)delta * auraChangeRate;

        if (Input.IsActionPressed("ui_accept"))
            auraChange += (float)delta * auraChangeRate;

        aurameter = Mathf.Clamp(aurameter + auraChange, minAurameter, maxAurameter);
    }

    private void UpdateAuraUI()
    {
        auraMeterBar.Value = aurameter;
        auraLabel.Text = $"Aura: {aurameter:F0}/{maxAurameter:F0}";
    }

     /// Called when a dialogue option button is pressed (override or connect signal as needed).
    private void OnOptionPressed(int index)
    {
        GD.Print($"Option {index + 1} selected");
        // TODO: hook into your dialogue / game logic here
    }

    // -- AuraMeter API ------------------------------------------------------

    /// Increase the Aurameter by the specified amount.
    public void IncreaseAura(float amount)
    {
        aurameter = Mathf.Clamp(aurameter + amount, minAurameter, maxAurameter);
    }

    /// Decrease the Aurameter by the specified amount.
    public void DecreaseAura(float amount)
    {
        aurameter = Mathf.Clamp(aurameter - amount, minAurameter, maxAurameter);
    }

    /// Set the Aurameter to a specific value.
    public void SetAura(float value)
    {
        aurameter = Mathf.Clamp(value, minAurameter, maxAurameter);
    }

    /// Get the current Aurameter value.
    public float GetAura() => aurameter;

    /// Get the Aurameter as a normalized value (0?1).
    public float GetAuraNormalized() => aurameter / maxAurameter;
}
