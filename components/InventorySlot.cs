using Godot;
using System;
using System.Collections.Generic;

public partial class InventorySlot : Node2D
{
	// Public, general access enums and maps
	public enum ToolType
	{
		Scythe,
		SeedBag,
		Trimmer,
		Flashlight
	}

	public Dictionary<ToolType, string> ToolPaths { get; private set; } = new Dictionary<ToolType, string>
	{
		[ToolType.Scythe] = "res://assets/tools/29.png",
		[ToolType.SeedBag] = "res://assets/tools/6.png",
		[ToolType.Trimmer] = "res://assets/tools/25.png",
		[ToolType.Flashlight] = "res://assets/flashlight.png"
	};

	public Dictionary<ToolType, string> ToolSounds { get; private set; } = new Dictionary<ToolType, string>
	{
		[ToolType.Scythe] = "res://assets/sounds/scythe.mp3",
		[ToolType.SeedBag] = "res://assets/sounds/bag.mp3",
		[ToolType.Trimmer] = "res://assets/sounds/trimmer.mp3",
	};

	// References to nodes in the scene
	private Sprite2D InventorySprite;
	private Sprite2D LeftHandSprite;
	private Sprite2D RightHandSprite;
	private AnimationPlayer AnimationPlayer;
	private AudioStreamPlayer2D AudioStreamPlayer2D;


	// Configuration properties for this instance
	private bool IsRightHand = false;
	private ToolType CurrentType = ToolType.Scythe;

	// Called when scene inserted into scene tree
	public override void _Ready()
	{
		InventorySprite = GetNode<Sprite2D>("InventorySprite");
		LeftHandSprite = GetNode<Sprite2D>("LeftHandSprite");
		RightHandSprite = GetNode<Sprite2D>("RightHandSprite");
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AudioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");

		DisplayTool(CurrentType, IsRightHand, true);
	}

	// Public method that loads tool and hand at start up and dynamically throughout game
	public void DisplayTool(ToolType type, bool? isRightHand, bool isSilent = false)
	{
		if (isRightHand.HasValue && isRightHand.Value == true)
		{
			LeftHandSprite.Visible = false;
			RightHandSprite.Visible = true;
		}

		if (isRightHand.HasValue && isRightHand.Value == false)
		{
			LeftHandSprite.Visible = true;
			RightHandSprite.Visible = false;
		}

		CurrentType = type;

		InventorySprite.Texture = GD.Load<Texture2D>(ToolPaths[CurrentType]);

		InventorySprite.Scale = type == ToolType.Flashlight ? new Vector2(0.15f, 0.15f) : new Vector2(1, 1);

		if (isSilent) return;

		AnimationPlayer.Stop(true);
		AnimationPlayer.Play("pulse");

		if (ToolSounds.ContainsKey(CurrentType))
		{
			AudioStreamPlayer2D.Stream = GD.Load<AudioStream>(ToolSounds[CurrentType]);
			AudioStreamPlayer2D.Play();
		}

	}
}
