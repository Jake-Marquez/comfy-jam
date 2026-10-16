using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{

	private TextureRect _texture_rect;
	private Node2D _tile_map_layers;
	private InventorySlot InventorySlotLeft;
	private InventorySlot InventorySlotRight;
	private TextureButton SeedBagItem;
	private TextureButton TrimmerItem;
	private TextureButton ScytheItem;
    private Node2D Pumpkins;
    private Camera2D Camera;
    private AudioStreamPlayer2D Walking;
	private Timer GameRoundTimer;
	private TimerLabel TimerLabel;

	[Export]
	public float PumpkinSproutSpeed { get; set; } = 0.85f;
	[Export]
	public float PumpkinMatureSpeed { get; set; } = 0.6f;
	[Export]
	public float PumpkinExpireSpeed { get; set; } = 0.15f;
	[Export]
	public double PumpkinLifetime { get; set; } = 500;
	[Export]
	public int PumpkinChanceDenominator { get; set; } = 50;
	[Export]
	public int GnomeChanceDenominator { get; set; } = 60;

	private bool camera_zoomed = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_texture_rect = GetNode<TextureRect>("TextureRect");
		_tile_map_layers = GetNode<Node2D>("TileMapLayers");
		InventorySlotLeft = GetNode<InventorySlot>("CanvasLayer/InventorySlotLeft");
		InventorySlotRight = GetNode<InventorySlot>("CanvasLayer/InventorySlotRight");
		SeedBagItem = GetNode<TextureButton>("TileMapLayers/SeedBagItem");
		TrimmerItem = GetNode<TextureButton>("TileMapLayers/TrimmerItem");
		ScytheItem = GetNode<TextureButton>("TileMapLayers/ScytheItem");
        Pumpkins = GetNode<Node2D>("TileMapLayers/Pumpkins");
        Camera = GetNode<Camera2D>("Camera2D");
        Walking = GetNode<AudioStreamPlayer2D>("Walking");
		GameRoundTimer = GetNode<Timer>("GameRoundTimer");
		TimerLabel = GetNode<TimerLabel>("CanvasLayer/TimerLabel");
        PackedScene PumpkinScene = GD.Load<PackedScene>("res://components/pumpkin.tscn");

		// init pumpkin patch nodes
		for (int x = -19; x < 18; x++)
		{
			for (int y = 0; y < 2; y++)
			{
                Pumpkin PumpkinInstance = (Pumpkin)PumpkinScene.Instantiate();
                Pumpkins.AddChild(PumpkinInstance);
                PumpkinInstance.Position = new Vector2(100 + (x * 64), 540 + (y * 64));
                PumpkinInstance.Scale = new(3, 3);
            }
		}

        InventorySlotLeft.DisplayTool(InventorySlot.ToolType.Flashlight, false, true);
		InventorySlotRight.DisplayTool(InventorySlot.ToolType.Scythe, true, true);

		SeedBagItem.ButtonUp += () => PickupItem(InventorySlot.ToolType.SeedBag);
		TrimmerItem.ButtonUp += () => PickupItem(InventorySlot.ToolType.Trimmer);
		ScytheItem.ButtonUp += () => PickupItem(InventorySlot.ToolType.Scythe);

		// Anything in the "dark" group is only visible under the flashlight.
		_tile_map_layers.AddToGroup("dark");
		_texture_rect.SetMeta("darkness", 0.7f); // background stays dimly visible
		_texture_rect.AddToGroup("dark");
		AddChild(new Flashlight());

		// Stagger the start times so the rects don't bob in unison.
		float delay = 0f;
		foreach (Node child in _tile_map_layers.GetChildren())
		{
			if (child is TextureButton rect)
			{
				TextureButton r = rect;
				GetTree().CreateTimer(delay).Timeout += () => StartBob(r);
				delay += 0.4f;
			}
		}

		TimerLabel.EndGame += EndGame;
		// Node toastParty = GetNode<Node>("/root/ToastParty");
		// var toastConfig = new Godot.Collections.Dictionary
        // {
        //     { "text", "🥑 Notification from C#! 🥑" },
        //     { "gravity", "top" },
        //     { "direction", "right" }
        // };
		// toastParty.Call("show", toastConfig);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 direction = Vector2.Zero;
        if (Input.IsActionPressed("right"))
        {
			move_right();
			if (!Walking.Playing) Walking.Play();
        }
        if (Input.IsActionPressed("left"))
        {
			move_left();
			if (!Walking.Playing) Walking.Play();
        }
		if (Input.IsActionJustReleased("right") || Input.IsActionJustReleased("left"))
		{
			Walking.Stop();
		}
	}

	// Gently bobs the node up and down forever around its starting height.
	private void StartBob(Control node, float height = 6f, float duration = 1.2f)
	{
		float baseY = node.Position.Y;
		Tween tween = node.CreateTween().SetLoops().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(node, "position:y", baseY - height, duration);
		tween.TweenProperty(node, "position:y", baseY, duration);
	}

	public void move_right() // 1200 - -1200
	{
		if (_tile_map_layers.Position.X >= -100)
		{
			_texture_rect.Position += new Vector2(-0.1f, 0);
			_tile_map_layers.Position += new Vector2(-10, 0);
		}
		if (_tile_map_layers.Position.X < 1500)
		{
			ZoomInCamera();
		}
	}

	public void move_left()
	{
		if (_tile_map_layers.Position.X <= 2000)
		{
			_texture_rect.Position += new Vector2(0.1f, 0);
			_tile_map_layers.Position += new Vector2(10, 0);
		}
		if (_tile_map_layers.Position.X > 1500)
		{
			ZoomOutCamera();
		}
	}

	public void PickupItem(InventorySlot.ToolType type)
	{
		SeedBagItem.Visible = type != InventorySlot.ToolType.SeedBag;
		TrimmerItem.Visible = type != InventorySlot.ToolType.Trimmer;
		ScytheItem.Visible = type != InventorySlot.ToolType.Scythe;
		InventorySlotRight.DisplayTool(type, true);
	}

	public void ZoomInCamera()
	{
		if (camera_zoomed) return;
		Tween tween = Camera.CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
		tween.SetParallel(true);
		tween.TweenProperty(Camera, "zoom", new Vector2(1.5f, 1.5f), 0.6);
		tween.TweenProperty(Camera, "position:y", 436, 0.6);
		tween.TweenProperty(Camera, "position:x", 768, 0.6);
		camera_zoomed = true;
	}

	public void ZoomOutCamera()
	{
		if (!camera_zoomed) return;
		Tween tween = Camera.CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
		tween.SetParallel(true);
		tween.TweenProperty(Camera, "zoom", new Vector2(1.0f, 1.0f), 0.6f);
		tween.TweenProperty(Camera, "position:y", 324, 0.6);
		tween.TweenProperty(Camera, "position:x", 576, 0.6);
		camera_zoomed = false;
	}

	public void EndGame()
	{
		var Globals = GetNode<Globals>("/root/Globals");
		Globals.hasGameJustEnded = true;
		GetTree().ChangeSceneToFile("res://menu.tscn");
	}
}
