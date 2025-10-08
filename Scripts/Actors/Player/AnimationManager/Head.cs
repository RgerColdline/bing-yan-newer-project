using Godot;
using System;

public partial class Head : AnimatedSprite2D
{
    enum State { Idle, Left, Right, Up, Down }
    private State currentState = State.Idle;
    private State _lastDirection = State.Down; 

    public override void _Ready()
    {
        Play("default");
        ApplyState(State.Down); 
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("shoot_up") || @event.IsActionPressed("move_up"))
            _lastDirection = State.Up;
        else if (@event.IsActionPressed("shoot_down") || @event.IsActionPressed("move_down"))
            _lastDirection = State.Down;
        else if (@event.IsActionPressed("shoot_left") || @event.IsActionPressed("move_left"))
            _lastDirection = State.Left;
        else if (@event.IsActionPressed("shoot_right") || @event.IsActionPressed("move_right"))
            _lastDirection = State.Right;
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
        bool shoot_left = Input.IsActionPressed("shoot_left");
        bool shoot_right = Input.IsActionPressed("shoot_right");
        bool shoot_up = Input.IsActionPressed("shoot_up");
        bool shoot_down = Input.IsActionPressed("shoot_down");

        if (shoot_left && !shoot_right) return State.Left;
        if (shoot_right && !shoot_left) return State.Right;
        if (shoot_up && !shoot_down) return State.Up;
        if (shoot_down && !shoot_up) return State.Down;

        bool isMoving = Input.IsActionPressed("move_left") || Input.IsActionPressed("move_right") ||
                        Input.IsActionPressed("move_up") || Input.IsActionPressed("move_down");

        if (isMoving)
        {
            return _lastDirection;
        }
         
        return State.Down; 
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
                Play("back");
                break;
            case State.Down:
                FlipH = false;
                Play("default"); 
                break;
            default:
                FlipH = false;
                Play("default");
                break;
        }
    }
}