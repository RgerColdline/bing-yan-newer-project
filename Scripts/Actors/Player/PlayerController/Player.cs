
using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export]
    public int Health { get; set; } = 6;
    [Export]
    public float HurtCDTimer { get; set; } = 0.0f;
    [Export]
    public float HorizontalSpeed { get; set; } = 800.0f;
    [Export]
    public float VerticalSpeed { get; set; } = 600.0f;
    [Export]
    public float Acceleration { get; set; } = 4000.0f;
    [Export]
    public float Friction { get; set; } = 3000.0f;
    [Export]
    public PackedScene ProjectileScene { get; set; }
    [Export]
    public float ProjectileSpeed { get; set; } = 600.0f;

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
        HandleShooting();
        this.ZIndex = (int)this.GlobalPosition.Y;

    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        Vector2 currentVelocity = Velocity;

        if (inputDirection != Vector2.Zero)
        {
            var targetVelocity = new Vector2(inputDirection.X * HorizontalSpeed, inputDirection.Y * VerticalSpeed);
            currentVelocity = currentVelocity.MoveToward(targetVelocity, Acceleration * (float)delta);
        }
        else
        {
            currentVelocity = currentVelocity.MoveToward(Vector2.Zero, Friction * (float)delta);
        }

        Velocity = currentVelocity;
        MoveAndSlide();
        if (HurtCDTimer > 0.0f)
        {
            HurtCDTimer -= (float)delta;
        }
    }

    private void HandleShooting()
    {
        if (ProjectileScene == null) return;

        Vector2 shootDirection = Vector2.Zero;

        if (Input.IsActionJustPressed("shoot_up"))
            shootDirection = Vector2.Up;
        else if (Input.IsActionJustPressed("shoot_down"))
            shootDirection = Vector2.Down;
        else if (Input.IsActionJustPressed("shoot_left"))
            shootDirection = Vector2.Left;
        else if (Input.IsActionJustPressed("shoot_right"))
            shootDirection = Vector2.Right;

        if (shootDirection != Vector2.Zero)
        {
            var projectile = ProjectileScene.Instantiate<Missle>();
            GetTree().Root.AddChild(projectile);
            projectile.GlobalPosition = this.GlobalPosition;

            projectile.Shoot(shootDirection, ProjectileSpeed, this.Velocity);
        }
    }
    private void HandleCollision(Area2D body)
    {
        if (HurtCDTimer <= 0.0f && (body is Neuro || body is KnightArea || body is Fly))
        {
            GD.Print("碰到小敌人了");
            Health--;
            HurtCDTimer = 1.0f; 
        }
        if (HurtCDTimer <= 0.0f && (body is Boss))
        {
            GD.Print("碰到Boss了");
            Health -= 2; 
            HurtCDTimer = 1.0f; 
        }
        
    }

    private void EnemyTouch(Area2D body)
    {
        HandleCollision(body);
    }
}