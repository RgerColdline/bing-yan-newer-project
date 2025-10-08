using Godot;
using System;

public partial class Missle : Area2D
{
	[Export]
    public float InheritVelocityFactor { get; set; } = 0.5f;
    private Vector2 _velocity = Vector2.Zero;

    public override void _Ready()
    {
        
        AreaEntered += HitEnemy;

        var timer = GetNode<Timer>("Timer");
        // 将子节点 Timer 的 Timeout 信号，连接到一个方法上，这里用 lambda 表达式直接销毁节点
        timer.Timeout += () => QueueFree(); 
    }

    public override void _Process(double delta)
    {
        Position += _velocity * (float)delta;
    }

    public void Shoot(Vector2 direction, float speed, Vector2 initialVelocity)
    {
        _velocity = direction.Normalized() * speed + initialVelocity*InheritVelocityFactor;
    }

	private void HitEnemy(Area2D body)
	{
        GD.Print("子弹碰敌人");
		QueueFree();
        
        // if (body is Enemy enemy)
		// {
		//     enemy.TakeDamage(10);
		// }
	}
}