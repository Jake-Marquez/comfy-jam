using Godot;
using System;
using System.Collections.Generic;

public partial class Menu : Node2D
{

	private Node2D _Node2D;
	private Button _StartButton;
	private Button _InstructionsButton;
	private Button _RestartButton;
	private AudioStreamPlayer2D _MenuClick;
	private Control _GameOverScreen;
	private TextureRect _GameStartTextureRect;
	private Panel _GameStartPanel;
	private Control _GameHboxContainer;
	private Label _GameScoreLabel;

	private int instruction_state = -1;
	private List<PanelContainer> panels = new List<PanelContainer>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Node2D = GetNode<Node2D>("Node2D");
		_InstructionsButton = GetNode<Button>("HBoxContainer/MarginContainer/VBoxContainer/InstructionsButton");
		_StartButton = GetNode<Button>("HBoxContainer/MarginContainer/VBoxContainer/Button");
		_MenuClick = GetNode<AudioStreamPlayer2D>("MenuClick");
		_Node2D.AddToGroup("dark");
		_RestartButton = GetNode<Button>("Control2/PanelContainer/MarginContainer/VBoxContainer/Button");
		_GameOverScreen = GetNode<Control>("Control2");
		_GameStartTextureRect = GetNode<TextureRect>("TextureRect");
		_GameStartPanel = GetNode<Panel>("Panel");
		_GameStartTextureRect.AddToGroup("dark");
		_GameStartPanel.AddToGroup("dark");
		_GameHboxContainer = GetNode<Control>("HBoxContainer");
		_GameScoreLabel = GetNode<Label>("Control2/PanelContainer/MarginContainer/VBoxContainer/Label2");
		AddChild(new Flashlight());

		_InstructionsButton.ButtonUp += () =>
		{
			if (instruction_state < 0)
			{
				advance_instruction_state();
			}
		};

		_StartButton.ButtonUp += () => start_game();
		_RestartButton.ButtonUp += () => hide_game_over_screen();

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

		var Globals = GetNode<Globals>("/root/Globals");
		if (Globals.hasGameJustEnded)
		{
			Globals.hasGameJustEnded = false;
			show_game_over_screen();
		}

	}

	public void advance_instruction_state()
	{
		_MenuClick.Play();
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

	public void start_game()
	{
		_MenuClick.Play();
		GetTree().ChangeSceneToFile("res://main.tscn");
	}

	public void show_game_over_screen()
	{
		_GameOverScreen.Visible = true;
		_GameStartPanel.Visible = false;
		_GameHboxContainer.Visible = false;
		var Globals = GetNode<Globals>("/root/Globals");
		_GameScoreLabel.Text = "Points: " + Globals.score.ToString();
	}

	public void hide_game_over_screen()
	{
		_GameOverScreen.Visible = false;
		_GameStartPanel.Visible = true;
		_GameHboxContainer.Visible = true;
	}
}
