using Godot;
using System;

public partial class Hud : Control
{
    public override void _Ready()
    {
        base._Ready();
        GD.Print("HUD Node Ready");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}
