using Godot;
using System;

public class FreeCamera : Camera
{
	[Export]
	public float Speed = 50f;

	[Export]
	public float Sensivity = 0.5f;
	
	private Vector2 rotation;

	public override void _Ready()
	{
		Input.SetMouseMode(Input.MouseMode.Captured);
	}

	public override void _Input(InputEvent e)
	{
		if (e is InputEventMouseMotion)
			rotation = ((InputEventMouseMotion)e).Relative;
	}

	public override void _Process(float delta)
	{
		Vector3 direction = Vector3.Zero;

		if (Input.IsKeyPressed((int)KeyList.Z))
		{
			direction.z = -1;
		}
		if (Input.IsKeyPressed((int)KeyList.S))
		{
			direction.z = 1;
		}
		if (Input.IsKeyPressed((int)KeyList.D))
		{
			direction.x = 1;
		}
		if (Input.IsKeyPressed((int)KeyList.Q))
		{
			direction.x = -1;
		}
		if (Input.IsKeyPressed((int)KeyList.Space))
		{
			direction.y = 1;
		}
		if (Input.IsKeyPressed((int)KeyList.Shift))
		{
			direction.y = -1;
		}

		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
			Translate(delta * Speed * direction);
		}

		if (rotation != Vector2.Zero)
		{
			Rotate(Vector3.Up, delta * Sensivity * -rotation.x);
			Rotate(Transform.basis.x, delta * Sensivity * -rotation.y);
		}

		rotation = Vector2.Zero;
	}
}
