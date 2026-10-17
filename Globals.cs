using Godot;
using System;

public partial class Globals : Node
{

	public InventorySlot.ToolType ToolInRightHand { get; set; } = InventorySlot.ToolType.Scythe;
	public int Score = 0;
	public bool hasGameJustEnded = false;
	public bool can_sow_pumpkin = true;

	[Signal]
    public delegate void ToolCooldownStartEventHandler();

	[Signal]
    public delegate void ToolCooldownEndEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ToolCooldownStart += () => can_sow_pumpkin = false;
		ToolCooldownEnd += () => can_sow_pumpkin = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	
}
