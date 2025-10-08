using Godot;
using System;

public partial class Temp : Area2D
{
	private void TestColl(Node2D body)
	{
		GD.Print("测试成功");
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("初始化成功");
		BodyEntered += TestColl;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
