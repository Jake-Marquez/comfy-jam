using Godot;
using System;
using System.Collections.Generic;

public partial class Menu : Node2D
{

	private Node2D _Node2D;
	private Button _InstructionsButton;

	private int instruction_state = -1;
	private List<PanelContainer> panels = new List<PanelContainer>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Node2D = GetNode<Node2D>("Node2D");
		_InstructionsButton = GetNode<Button>("HBoxContainer/MarginContainer/VBoxContainer/InstructionsButton");
		_Node2D.AddToGroup("dark");
		AddChild(new Flashlight());

		_InstructionsButton.ButtonUp += () =>
		{
			if (instruction_state < 0)
			{
				advance_instruction_state();
			}
		};

		panels.Add(GetNode<PanelContainer>("Control/PanelContainer"));
		panels.Add(GetNode<PanelContainer>("Control/PanelContainer2"));
		panels.Add(GetNode<PanelContainer>("Control/PanelContainer3"));
		panels.Add(GetNode<PanelContainer>("Control/PanelContainer4"));
		panels.Add(GetNode<PanelContainer>("Control/PanelContainer5"));

		string[] instruction_panel_buttons = [
			"Control/PanelContainer/MarginContainer/HBoxContainer/VBoxContainer/Button",
			"Control/PanelContainer2/MarginContainer/HBoxContainer/VBoxContainer/Button",
			"Control/PanelContainer3/MarginContainer/HBoxContainer/VBoxContainer/Button",
			"Control/PanelContainer4/MarginContainer/HBoxContainer/VBoxContainer/Button",
			"Control/PanelContainer5/MarginContainer/VBoxContainer/Button"
			];
		foreach (string path in instruction_panel_buttons)
		{
			GetNode<Button>(path).ButtonUp += advance_instruction_state;
		}

	}

	public void advance_instruction_state()
	{
		foreach (var item in panels)
		{
			item.Visible = false;
		}
		if (instruction_state == -1) panels[0].Visible = true;
		if (instruction_state == 0) panels[1].Visible = true;
		if (instruction_state == 1) panels[2].Visible = true;
		if (instruction_state == 2) panels[3].Visible = true;
		if (instruction_state == 3) panels[4].Visible = true;

		instruction_state += 1;

		if (instruction_state > 4)
		{
			GD.Print("Resetting!");
			instruction_state = -1;
		}
	}
}
