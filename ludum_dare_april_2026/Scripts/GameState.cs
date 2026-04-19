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
    public float StartingAura { get; private set; } = 67f;
    public float Aura { get; private set; } = 67f;
    public float MaxAura { get; } = 100f;
    public float MinAura { get; } = 0f;

    [Signal] public delegate void AuraChangedEventHandler(float newValue);

    // ── Progress / Score ───────────────────────────────────────────────────
    public int Points { get; private set; }

    [Signal] public delegate void PointsChangedEventHandler(int newValue);

    // ── Character Portrait ────────────────────────────────────────────────
    /// <summary>
    /// Emitted when gameplay code wants to swap the character portrait. The texture
    /// is passed by reference (already loaded) — callers don't need to think about
    /// res:// paths, and the listening UI doesn't have to GD.Load anything.
    /// </summary>
    [Signal] public delegate void CharacterChangeRequestedEventHandler(Texture2D texture);

    /// <summary>
    /// Call this from GameManager (or any other system) to swap the character
    /// portrait shown in the main scene.
    /// </summary>
    public void RequestCharacterChange(Texture2D texture)
    {
        EmitSignal(SignalName.CharacterChangeRequested, texture);
    }

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }

    /// <summary>
    /// Resets run-scoped state before starting a new campaign.
    /// </summary>
    public void ResetRunState()
    {
        SetAura(StartingAura);
        SetPoints(0);
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

    public void AddPoint(int amount = 1)
    {
        if (amount <= 0) return;
        SetPoints(Points + amount);
    }

    private void SetPoints(int value)
    {
        if (value == Points) return;
        Points = value;
        EmitSignal(SignalName.PointsChanged, Points);
    }
}
