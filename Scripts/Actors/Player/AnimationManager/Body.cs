using Godot;
using System;

public partial class Body : AnimatedSprite2D
{
	enum State { Idle, Left, Right, Up, Down }
	private State currentState = State.Idle;

	public override void _Ready()
	{
		Play("default");
	}

	public override void _Process(double delta)
	{
		var newState = GetInputState();
		if (newState != currentState)
		{
			ApplyState(newState);
			currentState = newState;
		}
	}
	private State GetInputState()
    {
        bool left = Input.IsActionPressed("move_left");
        bool right = Input.IsActionPressed("move_right");
        bool up = Input.IsActionPressed("move_up");
        bool down = Input.IsActionPressed("move_down");

        if (left && !right)
            return State.Left;
        if (right && !left)
            return State.Right;

        if (up && !down)
            return State.Up;
        if (down && !up)
            return State.Down;
            
        return State.Idle;
    }
	private void ApplyState(State s)
    {
        switch (s)
        {
            case State.Left:
                FlipH = true;
                Play("left");
                break;
            case State.Right:
                FlipH = false;
                Play("right");
                break;
            case State.Up:
                FlipH = false;
                Play("front"); 
                break;
            case State.Down:
                FlipH = false;
                Play("back"); 
                break;
            default:
                FlipH = false;
                Play("default");
                break;
        }
    }
}
