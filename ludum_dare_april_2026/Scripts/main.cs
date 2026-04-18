using Godot;
using LudumDareApril2026.Core;

namespace LudumDareApril2026;

/// <summary>
/// Boot scene. Autoloads finish initializing during <c>_EnterTree</c>, so we
/// simply hand off to the main menu on the next frame.
/// </summary>
public partial class Main : Node2D
{
	public override void _Ready()
	{
		base._Ready();
		CallDeferred(nameof(GoToMenu));
	}

	private void GoToMenu()
	{
		SceneManager.Instance.GoToMainMenu();
	}
}
