// using Godot;
// using System;

// public partial class GameManager : Node
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
using System.Collections.Generic;

public partial class GameManager : Node
{
	[Export]
	private PackedScene _playerScene; // 在编辑器中拖入 Player.tscn

	[Export]
	private PackedScene[] _roomScenes; // 在编辑器中按顺序拖入你的房间场景

	private int _currentRoomIndex = -1;
	private Player _playerInstance;
	private Node _currentRoomInstance;

	// 当从主菜单点击 "Play" 时调用此方法
	public void StartGame()
	{
		if (_playerScene == null || _roomScenes.Length == 0)
		{
			GD.PrintErr("Player scene or room scenes not set in GameManager!");
			return;
		}

		// 实例化并添加玩家
		_playerInstance = _playerScene.Instantiate<Player>();
		GetTree().Root.AddChild(_playerInstance);

		// 开始第一关
		_currentRoomIndex = -1;
		GoToNextRoom();
	}

	// 切换到下一个房间
	public void GoToNextRoom()
	{
		_currentRoomIndex++;

		if (_currentRoomIndex >= _roomScenes.Length)
		{
			GD.Print("Congratulations! You've cleared all rooms!");
			// 这里可以添加游戏胜利的逻辑，比如返回主菜单
			EndGame();
			return;
		}

		// 获取 SceneManager 来处理场景切换
		var sceneManager = GetNode<SceneManager>("/root/SceneManager");
		sceneManager.SwitchToScene(_roomScenes[_currentRoomIndex], OnSceneLoaded);
	}

	// 场景加载完成后的回调
	private void OnSceneLoaded(Node newRoom)
	{
		_currentRoomInstance = newRoom;
		
		// 将玩家移动到新房间的出生点
		// 假设每个房间都有一个叫 "PlayerSpawn" 的 Marker2D 节点
		var spawnPosition = newRoom.GetNode<Marker2D>("PlayerSpawn");
		if (spawnPosition != null)
		{
			_playerInstance.GlobalPosition = spawnPosition.GlobalPosition;
		}
		else
		{
			GD.PrintErr($"Room '{newRoom.SceneFilePath}' is missing a 'PlayerSpawn' Marker2D node.");
			_playerInstance.GlobalPosition = Vector2.Zero; // 备用位置
		}
	}
	
	private void EndGame()
	{
		// 清理玩家实例
		if (IsInstanceValid(_playerInstance))
		{
			_playerInstance.QueueFree();
		}
		_playerInstance = null;
		_currentRoomIndex = -1;
		
		// 返回主菜单（假设主菜单场景路径是 "res://Scenes/GUI/Menu.tscn"）
		var sceneManager = GetNode<SceneManager>("/root/SceneManager");
		sceneManager.SwitchToScene("res://Scenes/GUI/Menu.tscn");
	}
}