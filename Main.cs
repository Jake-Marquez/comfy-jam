using Godot;
using System;

public partial class Main : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Node toastParty = GetNode<Node>("/root/ToastParty");
		var toastConfig = new Godot.Collections.Dictionary
        {
            { "text", "🥑 Notification from C#! 🥑" },
            { "gravity", "top" },
            { "direction", "right" }
        };
		toastParty.Call("show", toastConfig);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
