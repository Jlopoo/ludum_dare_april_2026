using Godot;
using System;

public partial class player_controller : Node2D
{
    public Vector2 lastMousePosition;
    public Vector2 currentMousePosition;
    public Color mouseCircleColor = new Color(1, 0, 0, 1);

    public override void _Ready()
    {
        base._Ready();
        GD.Print("PlayerController Node Ready");
        lastMousePosition = GetGlobalMousePosition();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        currentMousePosition = GetGlobalMousePosition();
        if (lastMousePosition != currentMousePosition) {
            lastMousePosition = currentMousePosition;
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        base._Draw(); 
        DrawCircle(lastMousePosition, 10, mouseCircleColor);
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        base._UnhandledInput(inputEvent);
        if (inputEvent is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.ButtonIndex == MouseButton.Left)
        {
            mouseCircleColor = new Color(0, 1, 0, 1);
            QueueRedraw();
        }
        else 
        {
            mouseCircleColor = new Color(1, 0, 0, 1);
            QueueRedraw();
        }
    }
}