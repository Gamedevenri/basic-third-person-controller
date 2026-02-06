using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static Godot.WebSocketPeer;

public partial class BaseCharacter : CharacterBody3D
{
	[Export] public float speed = 2.0f;
	[Export] public float jump_velocity = 4.5f;
    [Export] AnimationTree animation_tree;

    public Vector2 input_dir;
	public bool jump;
	private bool has_control = true;

	Vector2 target_vector;
	Vector2 current_vector;
	float strafe_acceleration = 4;

	public enum Player_states
	{
		Walking,
		Running,
		InAir,
	}
	private Player_states current_player_state = Player_states.Walking;

    public override void _Ready()
    {
		current_player_state = Player_states.Walking;
    }
    public override async void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
			if (velocity.Y < -4)
			{
                if (current_player_state != Player_states.InAir)
                {
                    change_player_state(Player_states.InAir);
                    animation_tree.Set("parameters/InAir/conditions/fall", true);
                }
                else
                {
                    animation_tree.Set("parameters/InAir/conditions/fall", true);
                }
            }


        } else
		{
			if (current_player_state == Player_states.InAir)
			{
                has_control = false;
                animation_tree.Set("parameters/InAir/conditions/land", true);
				animation_tree.Set("parameters/InAir/conditions/jump", false);
                animation_tree.Set("parameters/InAir/conditions/fall", false);
                await ToSignal(GetTree().CreateTimer(1.1), "timeout");
                change_player_state(Player_states.Walking);
				has_control = true;
            }
		}



		// Handle Jump.
		if (jump && IsOnFloor())
		{
			velocity.Y = jump_velocity;
			change_player_state(Player_states.InAir);
			animation_tree.Set("parameters/InAir/conditions/jump", true);
		}


        Vector3 direction = (Transform.Basis * new Vector3(input_dir.X, 0, input_dir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		target_vector = new Vector2(input_dir.X, input_dir.Y).Normalized();
		current_vector = current_vector.MoveToward(-target_vector, strafe_acceleration * (float)delta);
		Vector2 strafe_input = new Vector2(current_vector.X, -current_vector.Y);

		animation_tree.Set($"parameters/{current_player_state.ToString()}/blend_position", strafe_input);

    }

	public void change_player_state(Player_states state)
	{

        switch (state)
		{
			case Player_states.Walking:
				speed = 2.0f;
                break;
			case Player_states.Running:
				speed = 5.0f;
				break;
			case Player_states.InAir:
                animation_tree.Set("parameters/InAir/conditions/land", false);
                animation_tree.Set("parameters/InAir/conditions/jump", false);
                animation_tree.Set("parameters/InAir/conditions/fall", false);
                break;
		}

        current_player_state = state;
        animation_tree.Set("parameters/Transition/transition_request", state.ToString());
    }


}
