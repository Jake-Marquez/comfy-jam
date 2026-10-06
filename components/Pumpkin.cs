using Godot;
using System;

public partial class Pumpkin : Node2D
{

	private Sprite2D _sprite2D;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite2D = GetNode<Sprite2D>("PumpkinSprite");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
