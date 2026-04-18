using Godot;
using Godot.Collections;

namespace LudumDareApril2026.Data;

/// <summary>
/// Ordered list of all dates in the game. The game plays them in order.
/// Stored as a single <c>.tres</c> file so designers can drag-and-drop dates
/// into position inside the Godot editor without touching code.
/// </summary>
[GlobalClass]
public partial class DateLibrary : Resource
{
    [Export] public Array<DateScenario> Dates { get; set; } = new();

    public int Count => Dates.Count;

    public DateScenario? GetDate(int index)
    {
        if (index < 0 || index >= Dates.Count) return null;
        return Dates[index];
    }
}
