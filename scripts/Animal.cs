using Godot;
using System;

public class Animal : KinematicBody
{
	private enum Direction
	{
		BACKWARD = 0,
		FORWARD,
		RIGHT,
		LEFT,
		IDLE
	}

	private enum State
	{
		SEARCHING_FOOD = 0,
		SEARCHING_WATER,
		FOUND_FOOD,
		FOUND_WATER
	}

	// Attribs
	private Vector3 velocity;
	private Vector3 forward;
	float speed;
	private Vector3 gravity;
	private float jumpSpeed;

	private RandomNumberGenerator random;
	private float timer;
	private float maxTime;

	private Direction direction;
	private float hunger;
	private float thirst;
	private float hungerAmount;
	private float thirstAmount;

	private State state;
	private Vector3 target;

	// Editor objects
	private Area viewArea;
	private MeshInstance mesh;
	private ProgressBar hungerLevel;
	private ProgressBar thirstLevel;
	private Label stateLabel;
	
	private void Jump()
	{
		velocity.y = jumpSpeed;
	}

	private void TurnLeft()
	{
		float angle = Mathf.Deg2Rad(90f);
		forward = forward.Rotated(Vector3.Up, angle);
		mesh.RotateY(angle);
	}

	private void TurnRight()
	{
		float angle = -Mathf.Deg2Rad(90f);
		forward = forward.Rotated(Vector3.Up, angle);
		mesh.RotateY(angle);
	}

	private void TurnBackward()
	{
		float angle = Mathf.Deg2Rad(180f);
		forward = forward.Rotated(Vector3.Up, angle);
		mesh.RotateY(angle);
	}

	private void Move(float delta)
	{
		velocity = new Vector3(0f, velocity.y, 0f);

		// Direction
		if (IsOnWall())
			timer = 0;

		if (timer <= 0)
		{
			timer = random.Randf() * maxTime;
			direction = (Direction)random.RandiRange(0, 4);
		
			switch (direction)
			{
				case Direction.RIGHT:
					TurnRight();
					break;
				case Direction.LEFT:
					TurnLeft();
					break;
				case Direction.BACKWARD:
					TurnBackward();
					break;

				default:
					break;
			}
		}

		// Movement
		if (direction != Direction.IDLE)
		{
			if (IsOnFloor())
				Jump();
		
			velocity += speed * forward; // Go forward
		}
		
		velocity += gravity * delta;
		MoveAndSlide(velocity * delta, Vector3.Up);

		int slideCount = GetSlideCount();
		if (slideCount > 0)
		for (int i=0; i<slideCount; i++)
			HandleCollision(GetSlideCollision(i));

		timer -= delta;
	}

	private void HandleCollision(KinematicCollision collision)
	{
		Node collider = (Node)collision.Collider;

		if (collider.Name == "Water")
		{
			thirst = 100f;
			hunger -= 1f;
			direction = Direction.IDLE;
			state = State.SEARCHING_WATER;
			timer = maxTime;
		}
	}

	private void ColorLevel(ProgressBar bar)
	{
		StyleBoxFlat style = (StyleBoxFlat)bar.Get("custom_styles/fg");

		if (bar.Value >= 75)
		{
			style.BgColor = Colors.Green;
		}
		else if (bar.Value >= 50)
		{
			style.BgColor = Colors.Yellow;
		}
		else if (bar.Value >= 25)
		{
			style.BgColor = Colors.Orange;
		}
		else
		{
			style.BgColor = Colors.Red;
		}
	}

	public override void _Ready()
	{
		gravity = new Vector3(0f, -1200f, 0f);
		forward = -Vector3.Forward;
		velocity = Vector3.Zero;
		jumpSpeed = 400f;
		speed = 300f;

		random = new RandomNumberGenerator();
		timer = 0;
		maxTime = 3f;

		direction = Direction.IDLE;
		thirst = 100f;
		hunger = 100f;
		thirstAmount = 3f;
		hungerAmount = 1f;

		state = State.SEARCHING_WATER;
		target = Vector3.Zero;

		mesh = GetNode<MeshInstance>("Mesh");
		hungerLevel = GetNode<ProgressBar>("InfoViewport/Info/Hunger/Level");
		thirstLevel = GetNode<ProgressBar>("InfoViewport/Info/Thirst/Level");
		stateLabel = GetNode<Label>("InfoViewport/Info/State/Label");
		viewArea = GetNode<Area>("ViewArea");
		viewArea.Connect("body_entered", this, "onViewAreaBodyEntered");
	}

	public override void _PhysicsProcess(float delta)
	{
		Move(delta);

		// Update animal attributes
		thirst -= delta * thirstAmount;
		hunger -= delta * hungerAmount;

		// Update UI
		thirstLevel.Value = thirst;
		hungerLevel.Value = hunger;

		ColorLevel(thirstLevel);
		ColorLevel(hungerLevel);
		
		switch (state)
		{
			case State.SEARCHING_WATER:
				stateLabel.Text = "Looking for water";
				break;
			case State.FOUND_WATER:
				stateLabel.Text = "Found water";
				break;
			case State.SEARCHING_FOOD:
				stateLabel.Text = "Looking for food";
				break;
			case State.FOUND_FOOD:
				stateLabel.Text = "Found food";
				break;	

			default:
				stateLabel.Text = "Don't know what I'm doing";
				break;
		}
	}

	public void onViewAreaBodyEntered(Node body)
	{
		// Get target water or food position
	}
}
