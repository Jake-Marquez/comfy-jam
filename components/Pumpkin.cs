using Godot;
using System;
using System.Collections.Generic;

public partial class Pumpkin : Node2D
{

	public enum PumpkinState
	{
		Normal,
		Corrupting,
		Evil,
		Healing
	}

	private Sprite2D _sprite2D;
	private AnimatedSprite2D _pumpkin_evil_sprite;
	private AnimationPlayer _animation_player;
	private AudioStreamPlayer2D _gnome_laugh;
	private AudioStreamPlayer2D _gnome_surprise;
	private TextureButton _btn;
	private Timer _gnome_timer;
	private AudioStreamPlayer2D _audio_stream_player;

	private double _pumpkinLifetime;
	private const string _pumpkinFolder = "res://assets/pixel_gnome_pack/Individual Files/Pumpkin/";
	
	// use init here as well
	private const double MAX_PUMPKIN_LIFETIME = 300;

	private const int PERCENT_VARIANCE = 50;

	// can only be set once?
	private double first_phase_end;
	private double second_phase_end;
	private double third_phase_end;
	private int pumpkin_state_int = 0;
	public PumpkinState state = PumpkinState.Normal;
	// This is the denominator for spawn chance, default 1 in 30
	// Increases when scared away with a flashlight
	private int gnome_spawn_chance = 30;
	// The lifetime percentage transitions take place at
	// for example, 85% lifetime left - transition to phase 2, 60% left - transition to phase 3
	private List<float> lifetime_phase_transitions = new List<float>(){ 0.85f, 0.6f, 0.15f };

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite2D = GetNode<Sprite2D>("PumpkinSprite");
		_pumpkin_evil_sprite = GetNode<AnimatedSprite2D>("PumpkinEvilSprite");
		_animation_player = GetNode<AnimationPlayer>("AnimationPlayer");
		_gnome_laugh = GetNode<AudioStreamPlayer2D>("GnomeLaugh");
		_gnome_surprise = GetNode<AudioStreamPlayer2D>("GnomeSurprise");
		_btn = GetNode<TextureButton>("TextureButton");
		_gnome_timer = GetNode<Timer>("GnomeTimer");
		_audio_stream_player = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");

		_pumpkin_evil_sprite.AnimationFinished += () => finish_corruption();
		_btn.ButtonUp += _on_pumpkin_click;
		_gnome_timer.Timeout += try_corruption;

		if (!seedGerminated())
		{
			// maybe there should be an indicator to the user that this seed/plot hasn't germinated
			// alternatively, IRL you don't know that germination has failed besides seeing no sprout
			return;
		}

		setPumpkinSprite(2);

		setPumpkinLifetime();
	}

	private void setPumpkinLifetime()
	{
		double randomVariance = Random.Shared.Next(0, PERCENT_VARIANCE + 1) / MAX_PUMPKIN_LIFETIME;

		bool coin = Random.Shared.Next(0, 2) == 1;

		if (coin)
		{
			_pumpkinLifetime = MAX_PUMPKIN_LIFETIME - randomVariance;
		}
		else
		{
			_pumpkinLifetime = MAX_PUMPKIN_LIFETIME + randomVariance;
		}

		first_phase_end = _pumpkinLifetime * lifetime_phase_transitions[0];
		second_phase_end = _pumpkinLifetime * lifetime_phase_transitions[1];
		third_phase_end = _pumpkinLifetime * lifetime_phase_transitions[2];
	}

	private bool seedGerminated()
	{
		//randomly delete node aka the seed did not germinate

		// a cursory google search finds that the germination rate is 85-90%
		// let's choose a 3/20 chance of failure
		// we may want to make the odds worse to make the gameplay more interesting

		int diceRoll = Random.Shared.Next(1, 21);

		if (diceRoll < 4)
		{
			//GD.Print("Failed to germinate");
			_sprite2D.Hide();
			_pumpkinLifetime = 0;
			
			return false;
		}

		return true;
	}

	// returns true if an image was actually changed
	// also sets pumpkin_state_int
	private bool setPumpkinSprite(int  pumpkinSpriteNum)
	{
		if (pumpkinSpriteNum == pumpkin_state_int) return false;

		switch(pumpkinSpriteNum)
		{
			case 2:
				_sprite2D.Texture = GD.Load<Texture2D>(_pumpkinFolder + "2 - Pumpkin Sprout.png");
				break;
			case 3:
				_sprite2D.Texture = GD.Load<Texture2D>(_pumpkinFolder + "3 - Pumpkin Mid.png");
				break;
			case 4:
				_sprite2D.Texture = GD.Load<Texture2D>(_pumpkinFolder + "4 - Pumpkin Full.png");
				break;
			case 5:
				_sprite2D.Texture = GD.Load<Texture2D>(_pumpkinFolder + "5 - Pumpkin Wilt.png");
				break;
		}
		pumpkin_state_int = pumpkinSpriteNum;
		return true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// make this prettier
		if (_pumpkinLifetime > first_phase_end)
		{
			// do nothing
		}
		else if ((_pumpkinLifetime <= first_phase_end) && (_pumpkinLifetime > second_phase_end))
		{
			setPumpkinSprite(3);
		}
		else if ((_pumpkinLifetime <= second_phase_end) && (_pumpkinLifetime > third_phase_end))
		{
			// Uncomment this instead of line 153 if you want to see corruption animation for fun 
			//if (setPumpkinSprite(4)) start_corruption();
			setPumpkinSprite(4);
		}
		else if ((_pumpkinLifetime <= third_phase_end) && (_pumpkinLifetime > 0))
		{
			setPumpkinSprite(5);
		}
		else if (_pumpkinLifetime <= 0)
		{
			// hide node
			_sprite2D.Hide();
		}

		// Calculate distance from mouse to center of scene
		Vector2 mousePosition = GetGlobalMousePosition();
		float distance = GlobalPosition.DistanceTo(mousePosition);

		// If inside flashlight, try and stop corruption
		if (distance < 75)
		{
			stop_corruption();
		}
	}

	private void try_corruption()
	{
		if (state != PumpkinState.Normal) return;
		if (pumpkin_state_int != 4) return;
		var rand = GD.RandRange(1, gnome_spawn_chance);
		if (rand == 1) start_corruption();
	}

	// called by the main script in the future, to start corruption
	public void start_corruption()
	{
		if (state == PumpkinState.Corrupting) return;
		state = PumpkinState.Corrupting;
		_sprite2D.Visible = false;
		_pumpkin_evil_sprite.Visible = true;
		_pumpkin_evil_sprite.SpeedScale = 1;
		_pumpkin_evil_sprite.Play("corruption");
		_animation_player.Play("gnome_in_front");
	}

	// called when the mouse (flashlight) gets close to the pumpkin
	// reverses the corruption animation direction
	public void stop_corruption()
	{
		if (state != PumpkinState.Corrupting) return;
		state = PumpkinState.Healing;
		_pumpkin_evil_sprite.SpeedScale = -1;
		_animation_player.Stop();
		_gnome_laugh.Stop();
		_gnome_surprise.Play();
		gnome_spawn_chance += 10;
	}

	// called when the corruption animation finishes, going forward or backwards
	private void finish_corruption()
	{
		// If the animation was going backwards, it reverts to original lifecycle
		if (_pumpkin_evil_sprite.SpeedScale == -1)
		{
			_sprite2D.Visible = true;
			_pumpkin_evil_sprite.Visible = false;
			state = PumpkinState.Normal;
		}

		// If the animation was going forwards, its permanently evil now
		if (_pumpkin_evil_sprite.SpeedScale == 1)
		{
			_animation_player.Stop();
			state = PumpkinState.Evil;
		}
	}

	private void _on_timer_timeout()
	{
		// If the state is evil, corrupting, corrupted, this timer should'nt do anything
		if (state != PumpkinState.Normal) return;
		if (_pumpkinLifetime > 0)
		{
			_pumpkinLifetime -= 5;
		}
	}

	private void _on_pumpkin_click()
	{
		var Globals = GetNode<Globals>("/root/Globals");
		InventorySlot.ToolType InHand = Globals.ToolInRightHand;
		GD.Print("Pumpkin clicked, tool is: " + InHand.ToString());

		switch(InHand)
		{
			case InventorySlot.ToolType.Flashlight:
				GD.PrintErr("Flashlight should not be in right hand");
				break;
			case InventorySlot.ToolType.SeedBag:
				sowNewPumpkin();
				break;
			case InventorySlot.ToolType.Scythe:
				killPumpkin();
				break;
			case InventorySlot.ToolType.Trimmer:
				if (harvestPumpkin())
				{
					// add points to some counter;
				}
				break;
		}

		if (InventorySlot.ToolSounds.ContainsKey(InHand))
		{
			_audio_stream_player.Stream = GD.Load<AudioStream>(InventorySlot.ToolSounds[InHand]);
			_audio_stream_player.Play();
		}
	}

	private void sowNewPumpkin()
	{
		// seedbag should place a new pumpkin seed only if the plot is completely empty
		if (isDead())
		{
			// change sprite temporarily to seed?
			setPumpkinSprite(2);
			setPumpkinLifetime();
			_sprite2D.Visible = true;
		}
		else
		{
			GD.PrintErr("Cannot sow, plot is not empty!");
		}
	}

	private void killPumpkin()
	{
		_pumpkinLifetime = 0;
		pumpkin_state_int = 0;
		_sprite2D.Hide();

		GD.Print("Pumpkin eliminated!");
	}

	private bool isDead()
	{
		return (_pumpkinLifetime <= 0) && (pumpkin_state_int == 0) && !_sprite2D.IsVisibleInTree();
	}

	private bool harvestPumpkin()
	{
		if (pumpkin_state_int == 4)
		{
			killPumpkin();
			return true;
		}
		else
		{
			GD.PrintErr("Cannot harvest!");
			return false;
		}
	}
}
