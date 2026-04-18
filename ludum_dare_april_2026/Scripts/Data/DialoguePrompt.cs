using Godot;

namespace LudumDareApril2026.Data;

/// <summary>
/// A single beat of the conversation: the alien says something, and the player
/// picks one of three responses.
/// </summary>
[GlobalClass]
public partial class DialoguePrompt : Resource
{
    [Export(PropertyHint.MultilineText)] public string AlienLine { get; set; } = "";

    [Export] public ResponseOption? ResponseA { get; set; }
    [Export] public ResponseOption? ResponseB { get; set; }
    [Export] public ResponseOption? ResponseC { get; set; }

    /// <summary>Returns the response at index 0, 1, or 2, or null if not configured.</summary>
    public ResponseOption? GetResponse(int index) => index switch
    {
        0 => ResponseA,
        1 => ResponseB,
        2 => ResponseC,
        _ => null,
    };

    public int ResponseCount
    {
        get
        {
            var count = 0;
            if (ResponseA is not null) count++;
            if (ResponseB is not null) count++;
            if (ResponseC is not null) count++;
            return count;
        }
    }
}
