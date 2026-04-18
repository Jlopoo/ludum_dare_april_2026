using Godot;

namespace LudumDareApril2026.Data;

/// <summary>
/// One of the three things the player can say in response to a <see cref="DialoguePrompt"/>.
/// </summary>
[GlobalClass]
public partial class ResponseOption : Resource
{
    [Export(PropertyHint.MultilineText)] public string Text { get; set; } = "";

    /// <summary>Reaction this response elicits from the current alien.</summary>
    [Export] public ReactionType Reaction { get; set; } = ReactionType.Neutral;
}
