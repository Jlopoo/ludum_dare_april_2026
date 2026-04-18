using Godot;
using System;

public partial class Main : Node2D
{
	public override void _Ready()
	{
		base._Ready();
		GD.Print("Main Node Ready");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
