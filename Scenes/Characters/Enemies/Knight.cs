// using Godot;
// using System;

// public partial class Knight : CharacterBody2D
// {
// 	// Called when the node enters the scene tree for the first time.
// 	public override void _Ready()
// 	{
// 	}

// 	// Called every frame. 'delta' is the elapsed time since the previous frame.
// 	public override void _Process(double delta)
// 	{
// 	}
// }
using Godot;
using System;
using System.Linq;

public partial class Knight : CharacterBody2D
{
	[Export]
	public float Speed { get; set; } = 100.0f;

	// 动画节点引用
	private AnimatedSprite2D _body;
	private AnimatedSprite2D _head;

	// 寻路相关
	private Node2D _player;
	private Vector2[] _path = [];
	private int _currentPathIndex = 0;
	private NavigationAgent2D _navigationAgent;

	public override void _Ready()
	{
		// 获取子节点
		_body = GetNode<AnimatedSprite2D>("Body");
		_head = GetNode<AnimatedSprite2D>("Head");

		// 创建并配置 NavigationAgent2D
		_navigationAgent = new NavigationAgent2D();
		AddChild(_navigationAgent);

		// 设置导航更新的回调
		_navigationAgent.PathDesiredDistance = 4.0f;
		_navigationAgent.TargetDesiredDistance = 4.0f;
		_navigationAgent.NavigationFinished += OnNavigationFinished;

		// 使用定时器定期更新寻路目标，避免每帧都计算，节省性能
		var timer = GetTree().CreateTimer(0.5, processAlways: false, processInPhysics: true);
		timer.Timeout += UpdateTargetPath;
	}

	public override void _PhysicsProcess(double delta)
	{
		this.ZIndex = (int)this.GlobalPosition.Y;
		// 尝试获取一次玩家节点
		if (_player == null)
		{
			_player = GetTree().GetFirstNodeInGroup("player") as Node2D;
			if (_player == null) return; // 如果找不到玩家，什么都不做
		}

		// 如果没有路径，就停下来
		if (_navigationAgent.IsNavigationFinished())
		{
			Velocity = Vector2.Zero;
			MoveAndSlide();
			return;
		}

		// 获取到下一个路径点的方向
		Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();
		Vector2 direction = GlobalPosition.DirectionTo(nextPathPosition);

		// 根据方向移动
		Velocity = direction * Speed;
		MoveAndSlide();

		// 更新动画
		UpdateAnimation(direction);
	}

	private void UpdateTargetPath()
	{
		if (_player != null)
		{
			_navigationAgent.TargetPosition = _player.GlobalPosition;
		}
	}

	private void UpdateAnimation(Vector2 direction)
	{
		// 优先判断垂直方向
		if (Mathf.Abs(direction.Y) > Mathf.Abs(direction.X))
		{
			if (direction.Y > 0)
			{
				_body.Play("front");
				_head.Play("front");
			}
			else
			{
				_body.Play("back");
				_head.Play("back");
			}
			_body.FlipH = false;
			_head.FlipH = false;
		}
		// 其次判断水平方向
		else if (Mathf.Abs(direction.X) > 0.1f) // 增加一个小的阈值避免原地抖动
		{
			_body.Play("right");
			_head.Play("right");
			_body.FlipH = direction.X < 0; // 如果向左，则水平翻转
			_head.FlipH = direction.X < 0;
		}
	}
	private void OnNavigationFinished()
	{
		// 寻路完成时，可以在这里执行一些操作
		// 例如，播放攻击动画或进入待机状态
		// 目前，我们只是让它停下来，这个逻辑已经在 _PhysicsProcess 中处理了
		GD.Print("Navigation finished.");
	}
}
        // 如果没有移动，可以播放 idle 动画（如果你的动画资源里有的话）
        // else
        // {
        //