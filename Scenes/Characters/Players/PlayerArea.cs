using Godot;
using System;

public partial class PlayerArea : Area2D
{
	[Signal]
	public delegate void EnemyTouchEventHandler(Area2D body);
	private void TouchEnemy(Area2D body)
	{
		GD.Print($"碰到敌人了{body?.Name ?? "null"}");
		EmitSignal("EnemyTouchEventHandler", body);

	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("脚本已加载");
		AreaEntered += TouchEnemy;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    internal void Connect(string v1, Player player, string v2)
    {
        throw new NotImplementedException();
    }

}
