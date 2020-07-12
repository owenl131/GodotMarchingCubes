using Godot;
using System;

public class CameraContainer : Spatial
{
    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {
        float factor = 5;
        if (Input.IsActionPressed("ui_page_down"))
        {
            GetNode<Camera>("Camera").Translate(Vector3.Back * delta * factor);
        }
        if (Input.IsActionPressed("ui_page_up"))
        {
            GetNode<Camera>("Camera").Translate(Vector3.Forward * delta * factor);
        }
        if (Input.IsActionPressed("ui_left"))
        {
            RotateY(-factor * delta / 3);
        }
        if (Input.IsActionPressed("ui_right"))
        {
            RotateY(factor * delta / 3);
        }
        if (Input.IsActionPressed("ui_up"))
        {
            Translate(Vector3.Up * delta * factor);
        }
        if (Input.IsActionPressed("ui_down"))
        {
            Translate(Vector3.Down * delta * factor);
        }
        GetNode<Camera>("Camera").LookAt(Vector3.Up * 1, Vector3.Up);
    }
}
