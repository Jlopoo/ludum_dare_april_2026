using Godot;

/// <summary>
/// Autoload singleton — persists for the entire lifetime of the program.
/// Holds game-wide state (aura, which aliens have been dated, etc.) so
/// individual scenes can come and go without losing progress.
///
/// Access from any node: GetNode&lt;GameState&gt;("/root/GameState")
/// </summary>
public partial class GameState : Node
{
    public static GameState Instance { get; private set; } = null!;

    // ── Aura ──────────────────────────────────────────────────────────────
    public float Aura { get; private set; } = 67f;
    public float MaxAura { get; } = 100f;
    public float MinAura { get; } = 0f;

    [Signal] public delegate void AuraChangedEventHandler(float newValue);

    // ── Character Portrait ────────────────────────────────────────────────
    /// <summary>
    /// Emitted when gameplay code wants to swap the character portrait.
    /// The <paramref name="texturePath"/> is a res:// path to the new texture.
    /// </summary>
    [Signal] public delegate void CharacterChangeRequestedEventHandler(string texturePath);

    /// <summary>
    /// Call this from ConversationManager (or any other system) to swap the
    /// character portrait shown in the main scene.
    /// </summary>
    public void RequestCharacterChange(string texturePath)
    {
        EmitSignal(SignalName.CharacterChangeRequested, texturePath);
    }

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }

    public void IncreaseAura(float amount)
    {
        SetAura(Aura + amount);
    }

    public void DecreaseAura(float amount)
    {
        SetAura(Aura - amount);
    }

    public void SetAura(float value)
    {
        float clamped = Mathf.Clamp(value, MinAura, MaxAura);
        if (Mathf.IsEqualApprox(clamped, Aura)) return;
        Aura = clamped;
        EmitSignal(SignalName.AuraChanged, Aura);
    }

    public float GetAuraNormalized() => Aura / MaxAura;

    public void ApplyAffectionDelta(int delta)
    {
        if (delta != 0)
            IncreaseAura(delta);
    }
}
