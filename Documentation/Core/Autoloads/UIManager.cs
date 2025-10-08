using Godot;
using System;
using System.Collections.Generic;
using TheBindingofIsaac;

public partial class UIManager : CanvasLayer
{
    // 单例实例（可选，保持一致性）
    public static UIManager Instance { get; private set; }
    
    // UI引用
    private Control _currentHUD;
    private Control _activeMenu;
    private Stack<Control> _uiStack = new Stack<Control>();
    
    // 信号定义
    [Signal] public delegate void UIStateChangedEventHandler(string newState);
    [Signal] public delegate void HealthUpdatedEventHandler(int currentHealth, int maxHealth, int tempHealth);
	[Signal] public delegate void InventoryUpdatedEventHandler(Item[] items);
	[Signal] public delegate void TrinketUpdatedEventHandler(Item[] items);
    
    public override void _Ready()
	{
		// 单例初始化
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree();
			return;
		}

		GD.Print("UIManager 已初始化");

		// 连接其他管理器的信号
		ConnectToOtherManagers();
	}
    
    private void ConnectToOtherManagers()
    {
        // 连接到GameManager的健康事件
        var gameManager = GetNodeOrNull<GameManager>("/root/GameManager");
        if (gameManager != null && gameManager.HasSignal("PlayerHealthChanged"))
        {
            gameManager.Connect("PlayerHealthChanged", new Callable(this, nameof(OnPlayerHealthChanged)));
        }
    }
    
    // === 血量UI管理 ===
    public void UpdateHealthDisplay(int currentHealth, int maxHealth)
    {
        EmitSignal(SignalName.HealthUpdated, currentHealth, maxHealth);
        
        // 直接更新HUD中的血量显示
        var healthBar = GetNodeOrNull<TextureProgressBar>("HUD/HealthBar");
        if (healthBar != null)
        {
            healthBar.MaxValue = maxHealth;
            healthBar.Value = currentHealth;
        }
    }
    
    private void OnPlayerHealthChanged(int newHealth, int maxHealth)
    {
        UpdateHealthDisplay(newHealth, maxHealth);
    }
    
    // === 道具栏管理 ===
    public void AddItemToInventory(Texture2D itemIcon, string itemName)
    {
        var inventory = GetNodeOrNull<GridContainer>("HUD/InventoryGrid");
        if (inventory != null)
        {
            var itemSlot = new TextureRect();
            itemSlot.Texture = itemIcon;
            itemSlot.ExpandMode = TextureRect.ExpandModeEnum.FitWidth;
            itemSlot.StretchMode = TextureRect.StretchModeEnum.KeepAspect;
            itemSlot.CustomMinimumSize = new Vector2(32, 32);
            
            // 添加提示文本
            var tooltip = new TooltipPanel();
            tooltip.ItemName = itemName;
            itemSlot.AddChild(tooltip);
            
            inventory.AddChild(itemSlot);
        }
    }
    
    // === UI场景管理 ===
    public void ShowHUD()
    {
        // 隐藏现有HUD
        if (_currentHUD != null)
        {
            _currentHUD.QueueFree();
        }
        
        // 加载新的HUD
        var hudScene = GD.Load<PackedScene>("res://UI/HUD.tscn");
        _currentHUD = hudScene.Instantiate<Control>();
        AddChild(_currentHUD);
    }
    
    public void ShowPauseMenu()
    {
        // 暂停游戏
        GetTree().Paused = true;
        
        // 显示暂停菜单
        var pauseMenu = GD.Load<PackedScene>("res://UI/PauseMenu.tscn").Instantiate<Control>();
        AddChild(pauseMenu);
        _activeMenu = pauseMenu;
        
        EmitSignal(SignalName.UIStateChanged, "Paused");
    }
    
    public void HidePauseMenu()
    {
        if (_activeMenu != null)
        {
            _activeMenu.QueueFree();
            _activeMenu = null;
        }
        
        GetTree().Paused = false;
        EmitSignal(SignalName.UIStateChanged, "Playing");
    }
    
    // === 房间UI管理 ===
    public void ShowRoomTransition(string roomName, float duration = 1.0f)
    {
        var transition = GD.Load<PackedScene>("res://UI/RoomTransition.tscn").Instantiate<RoomTransition>();
        AddChild(transition);
        transition.StartTransition(roomName, duration);
    }
    
    // === 输入处理 ===
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
        {
            if (_activeMenu == null)
            {
                ShowPauseMenu();
            }
            else
            {
                HidePauseMenu();
            }
        }
    }
}

// 辅助类：道具提示面板
public partial class TooltipPanel : Panel
{
    public string ItemName { get; set; } = "未知道具";
    
    public override void _Ready()
    {
        var label = new Label();
        label.Text = ItemName;
        AddChild(label);
        Hide(); // 默认隐藏
    }
    
    public void _MouseEnter()
    {
        Show();
    }
    
    public void _MouseExit()
    {
        Hide();
    }
}

// 房间过渡效果
public partial class RoomTransition : ColorRect
{
    private Label _roomLabel;
    
    public override void _Ready()
    {
        _roomLabel = GetNode<Label>("RoomLabel");
    }
    
    public void StartTransition(string roomName, float duration)
    {
        _roomLabel.Text = roomName;
        
        var tween = CreateTween();
        tween.TweenProperty(this, "color:a", 1.0f, duration / 2);
        tween.TweenInterval(duration / 4);
        tween.TweenProperty(this, "color:a", 0.0f, duration / 2);
        tween.TweenCallback(new Callable(this, "queue_free"));
    }
}