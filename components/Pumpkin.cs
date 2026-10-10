using Godot;
using System;

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
	private Area2D _area_2d;

	private double _pumpkinLifetime;
	private const string _pumpkinFolder = "res://assets/pixel_gnome_pack/Individual Files/Pumpkin/";
	
	// use init here as well
	private const double MAX_PUMPKIN_LIFETIME = 100;

	private const int PERCENT_VARIANCE = 50;

	// can only be set once?
	private double first_phase_end;
	private double second_phase_end;
	private double third_phase_end;
	private int pumpkin_state_int = 0;
	public PumpkinState state = PumpkinState.Normal;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!seedGerminated())
		{
			// maybe there should be an indicator to the user that this seed/plot hasn't germinated
			// alternatively, IRL you don't know that germination has failed besides seeing no sprout
			return;
		}

		_sprite2D = GetNode<Sprite2D>("PumpkinSprite");
		_pumpkin_evil_sprite = GetNode<AnimatedSprite2D>("PumpkinEvilSprite");
		_animation_player = GetNode<AnimationPlayer>("AnimationPlayer");
		_gnome_laugh = GetNode<AudioStreamPlayer2D>("GnomeLaugh");
		_gnome_surprise = GetNode<AudioStreamPlayer2D>("GnomeSurprise");
		_area_2d = GetNode<Area2D>("Area2D");

		_pumpkin_evil_sprite.AnimationFinished += () => finish_corruption();
		_area_2d.MouseEntered += () => stop_corruption();

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
		

		double half = _pumpkinLifetime / 2;

		double quarter = half / 2;

		first_phase_end = _pumpkinLifetime- quarter;
		GD.Print(first_phase_end);

		second_phase_end = _pumpkinLifetime - half;
		GD.Print(second_phase_end);


		third_phase_end = _pumpkinLifetime - (half + quarter);
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
			QueueFree();
			return false;
		}

		return true;
	}

	// returns true if an image was actually changed
	private bool setPumpkinSprite(int  pumpkinSpriteNum)
	{
		if (pumpkinSpriteNum == pumpkin_state_int) return false;
		switch(pumpkinSpriteNum)
		{
			default: break;
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
			return;
		}
		else if ((_pumpkinLifetime <= first_phase_end) && (_pumpkinLifetime > second_phase_end))
		{
			setPumpkinSprite(3);
		}
		else if ((_pumpkinLifetime <= second_phase_end) && (_pumpkinLifetime > third_phase_end))
		{
			// Uncomment this instead of line 153 if you want to see corruption animation for fun 
			// if (setPumpkinSprite(4)) start_corruption();
			setPumpkinSprite(4);
		}
		else if ((_pumpkinLifetime <= third_phase_end) && (_pumpkinLifetime > 0))
		{
			setPumpkinSprite(5);
		}
		else if (_pumpkinLifetime <= 0)
		{
			// delete node
			QueueFree();
		}

	}

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

	public void stop_corruption()
	{
		if (state != PumpkinState.Corrupting) return;
		state = PumpkinState.Healing;
		_pumpkin_evil_sprite.SpeedScale = -1;
		_animation_player.Stop();
		_gnome_laugh.Stop();
		_gnome_surprise.Play();
	}

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
		if (state != PumpkinState.Normal) return;
		if (_pumpkinLifetime > 0)
		{
			_pumpkinLifetime -= 5;
		}
	}
}
