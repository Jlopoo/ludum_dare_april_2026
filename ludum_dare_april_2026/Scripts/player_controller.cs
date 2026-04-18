using Godot;
using System;

public partial class player_controller : Node2D
{
    public Vector2 mousePosition;
    public Color mouseCircleColor;

    public override void _Ready()
    {
        base._Ready();
        GD.Print("PlayerController Node Ready");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        mousePosition = GetGlobalMousePosition();
    }

    public override void _Draw()
    {
        base._Draw();
        DrawCircle(mousePosition, 10, mouseCircleColor);
    }
}