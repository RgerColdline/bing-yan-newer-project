// using Godot;
// using System;

// public partial class SceneManager : Node
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

public partial class SceneManager : Node
{
    private Node _currentScene;
    private Action<Node> _onLoadCallback;
    private PackedScene _nextSceneResource;
    private string _nextScenePath;

    public override void _Ready()
    {
        // 在启动时，获取当前正在运行的场景（例如主菜单）
        var root = GetTree().Root;
        _currentScene = root.GetChild(root.GetChildCount() - 1);
    }

    /// <summary>
    /// 使用 PackedScene 资源切换场景。
    /// </summary>
    /// <param name="scene">要加载的场景资源</param>
    /// <param name="onLoadedCallback">场景加载完成后要执行的回调</param>
    public void SwitchToScene(PackedScene scene, Action<Node> onLoadedCallback = null)
    {
        _onLoadCallback = onLoadedCallback;
        _nextSceneResource = scene;
        _nextScenePath = null;

        // 使用 CallDeferred 可以安全地处理场景切换，避免在当前帧的逻辑中删除正在使用的节点。
        CallDeferred(nameof(PerformSceneSwitch));
    }

    /// <summary>
    /// 使用场景文件路径切换场景。
    /// </summary>
    /// <param name="path">场景文件的 res:// 路径</param>
    /// <param name="onLoadedCallback">场景加载完成后要执行的回调</param>
    public void SwitchToScene(string path, Action<Node> onLoadedCallback = null)
    {
        _onLoadCallback = onLoadedCallback;
        _nextSceneResource = null;
        _nextScenePath = path;
        
        CallDeferred(nameof(PerformSceneSwitch));
    }

    private void PerformSceneSwitch()
    {
        // 1. 卸载当前场景
        if (IsInstanceValid(_currentScene))
        {
            _currentScene.QueueFree();
        }

        // 2. 加载新场景
        PackedScene sceneToLoad = _nextSceneResource ?? GD.Load<PackedScene>(_nextScenePath);
        if (sceneToLoad == null)
        {
            GD.PrintErr($"Failed to load scene: {(_nextScenePath ?? "PackedScene was null")}");
            return;
        }

        // 3. 实例化并添加到场景树
        Node newSceneInstance = sceneToLoad.Instantiate();
        GetTree().Root.AddChild(newSceneInstance);

        // 4. 更新当前场景的引用
        _currentScene = newSceneInstance;

        // 5. 执行回调，通知调用者（GameManager）场景已加载完毕
        _onLoadCallback?.Invoke(newSceneInstance);
    }
}