using Godot;
using System;

public partial class Pumpkin : Node2D
{

	private Sprite2D _sprite2D;

	private double _pumpkinLifetime;
	private const string _pumpkinFolder = "res://assets/pixel_gnome_pack/Individual Files/Pumpkin/";
	
	// use init here as well
	private const double MAX_PUMPKIN_LIFETIME = 100;

	private const int PERCENT_VARIANCE = 50;

	// can only be set once?
	private double first_phase_end;
	private double second_phase_end;
	private double third_phase_end;

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

	private void setPumpkinSprite(int  pumpkinSpriteNum)
	{
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

	private void _on_timer_timeout()
	{
		GD.Print(_pumpkinLifetime);

		if (_pumpkinLifetime > 0)
		{
			_pumpkinLifetime -= 5;
		}
	}
}
