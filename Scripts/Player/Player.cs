using Godot;
using System;

public partial class Player : Node3D
{
	[Export]
	BaseCharacter character;
	[Export]
	Camera3D camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Vector2 input = Input.GetVector("left", "right", "forward", "backward");
        character.input_dir = -input;

        if(Input.IsActionPressed("jump"))
        {
            character.jump = true;
        }
        else if (Input.IsActionJustReleased("jump"))
        {
            character.jump = false;
        }

        if (!character.IsOnFloor()) return;
        if (Input.IsActionPressed("run"))
        {
            character.change_player_state(BaseCharacter.Player_states.Running);
        }
        else if (Input.IsActionJustReleased("run"))
        {
            character.change_player_state(BaseCharacter.Player_states.Walking);
        }
    }


}
