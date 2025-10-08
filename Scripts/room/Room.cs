// using Godot;
// using System.Collections.Generic;
// using System.Linq;

// public partial class Room : Node2D
// {
//     [Export]
//     private PackedScene[] _enemyScenes; // 在编辑器中为每个房间拖入要生成的敌人场景

//     [Export]
//     private NodePath[] _spawnPoints; // 在编辑器中拖入多个 Marker2D 作为敌人出生点

//     private List<Node> _enemies = new List<Node>();
//     private
// 建议新文件路径: f:/Rger_project/the-binding-of-isaac/Scripts/Rooms/Room.cs
using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Room : Node2D
{
    [Export]
    private PackedScene[] _enemyScenes; // 在编辑器中为每个房间拖入要生成的敌人场景

    [Export]
    private NodePath[] _spawnPointPaths; // 在编辑器中拖入多个 Marker2D 作为敌人出生点

    private List<Node> _liveEnemies = new List<Node>();
    private bool _isCleared = false;

    public override void _Ready()
    {
        // 如果没有设置敌人或出生点，直接视为空房间
        if (_enemyScenes == null || _enemyScenes.Length == 0 || _spawnPointPaths == null || _spawnPointPaths.Length == 0)
        {
            GD.Print("No enemies or spawn points defined for this room. Considering it cleared.");
            _isCleared = true;
            // 如果你希望空房间也直接切换，可以在这里调用 GoToNextRoom()
            // GoToNextRoom(); 
            return;
        }

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        var spawnPoints = _spawnPointPaths.Select(path => GetNode<Marker2D>(path)).ToArray();

        for (int i = 0; i < _enemyScenes.Length; i++)
        {
            PackedScene enemyScene = _enemyScenes[i];
            if (enemyScene == null) continue;

            Node2D enemyInstance = enemyScene.Instantiate<Node2D>();

            // 为敌人选择一个出生点（循环使用）
            Marker2D spawnPoint = spawnPoints[i % spawnPoints.Length];
            enemyInstance.GlobalPosition = spawnPoint.GlobalPosition;

            // 将敌人添加到场景中
            AddChild(enemyInstance);
            _liveEnemies.Add(enemyInstance);

            // 关键：监听敌人的 "tree_exiting" 信号，当敌人被 QueueFree() 时会触发
            enemyInstance.TreeExiting += () => OnEnemyDied(enemyInstance);
        }

        GD.Print($"Spawned {_liveEnemies.Count} enemies.");
    }

    private void OnEnemyDied(Node enemy)
    {
        if (_liveEnemies.Contains(enemy))
        {
            _liveEnemies.Remove(enemy);
            GD.Print($"An enemy was defeated. {_liveEnemies.Count} enemies remaining.");

            // 检查是否所有敌人都被击败
            if (!_isCleared && _liveEnemies.Count == 0)
            {
                _isCleared = true;
                GD.Print("All enemies defeated! Room cleared.");
                
                // 延迟一小段时间再切换场景，给玩家一个喘息和反馈的时间
                GetTree().CreateTimer(1.0f).Timeout += GoToNextRoom;
            }
        }
    }

    private void GoToNextRoom()
    {
        GD.Print("Switching to the next room...");
        var gameManager = GetNode<GameManager>("/root/GameManager");
        gameManager.GoToNextRoom();
    }
}