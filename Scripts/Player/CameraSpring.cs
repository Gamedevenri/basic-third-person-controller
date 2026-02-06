using Godot;
using System;

public partial class CameraSpring : SpringArm3D
{
    public float mouse_sens = 0.005f;

    public override void _Ready()
    {
        base._Ready();
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion input)
        {
            Vector3 rotation = Rotation;
            BaseCharacter parent = GetParent() as BaseCharacter;
            //rotation.X -= (input.Relative.Y * mouse_sens);
            parent.Rotation -= new Vector3(0,(input.Relative.X * mouse_sens), 0);
            rotation.X -= (input.Relative.Y * mouse_sens);
            Rotation = rotation;
        }
            
    }

}
