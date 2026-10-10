using Godot;
using System;

public partial class Main : Node2D
{

	private TextureRect _texture_rect;
	private Node2D _tile_map_layers;
	private InventorySlot InventorySlotLeft;
	private InventorySlot InventorySlotRight;
	private TextureButton SeedBagItem;
	private TextureButton TrimmerItem;
	private TextureButton ScytheItem;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_texture_rect = GetNode<TextureRect>("TextureRect");
		_tile_map_layers = GetNode<Node2D>("TileMapLayers");
		InventorySlotLeft = GetNode<InventorySlot>("InventorySlotLeft");
		InventorySlotRight = GetNode<InventorySlot>("InventorySlotRight");
		SeedBagItem = GetNode<TextureButton>("TileMapLayers/SeedBagItem");
		TrimmerItem = GetNode<TextureButton>("TileMapLayers/TrimmerItem");
		ScytheItem = GetNode<TextureButton>("TileMapLayers/ScytheItem");

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
        // Input.IsActionPressed checks if the key is currently being held down
        if (Input.IsActionPressed("right"))
        {
            // direction.X += 1;
			GD.Print("right");
			move_right();
        }
        if (Input.IsActionPressed("left"))
        {
            // direction.X -= 1;
			GD.Print("left");
			move_left();
        }
        if (Input.IsActionPressed("down"))
        {
            // direction.Y += 1;
			GD.Print("down");
        }
        if (Input.IsActionPressed("up"))
        {
            // direction.Y -= 1;
			GD.Print("up");
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
		if (_tile_map_layers.Position.X >= -1300)
		{
			_texture_rect.Position += new Vector2(-0.1f, 0);
			_tile_map_layers.Position += new Vector2(-10, 0);
		}
	}

	public void move_left()
	{
		if (_tile_map_layers.Position.X <= 2000)
		{
			_texture_rect.Position += new Vector2(0.1f, 0);
			_tile_map_layers.Position += new Vector2(10, 0);
		}
	}

	public void PickupItem(InventorySlot.ToolType type)
	{
		SeedBagItem.Visible = type != InventorySlot.ToolType.SeedBag;
		TrimmerItem.Visible = type != InventorySlot.ToolType.Trimmer;
		ScytheItem.Visible = type != InventorySlot.ToolType.Scythe;
		InventorySlotRight.DisplayTool(type, true);
	}
}
