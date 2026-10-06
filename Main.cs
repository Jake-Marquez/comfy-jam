using Godot;
using System;

public partial class Main : Node2D
{

	private TextureRect _texture_rect;
	private Node2D _tile_map_layers;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_texture_rect = GetNode<TextureRect>("TextureRect");
		_tile_map_layers = GetNode<Node2D>("TileMapLayers");

		// Anything in the "dark" group is only visible under the flashlight.
		_tile_map_layers.AddToGroup("dark");
		_texture_rect.SetMeta("darkness", 0.6f); // background stays dimly visible
		_texture_rect.AddToGroup("dark");
		AddChild(new Flashlight());
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
}
