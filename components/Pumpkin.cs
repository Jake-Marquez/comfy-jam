using Godot;
using System;

public partial class Pumpkin : Node2D
{

	private Sprite2D _sprite2D;

	private double _pumpkinLifetime;
	private const string _pumpkinFolder = "res://assets/pixel_gnome_pack/Individual Files/Pumpkin/";
	private const double MAX_PUMPKIN_LIFETIME = 100;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!seedGerminated())
		{
			return;
		}

		_sprite2D = GetNode<Sprite2D>("PumpkinSprite");

		setPumpkinSprite(2);

		_pumpkinLifetime = MAX_PUMPKIN_LIFETIME;
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
		if (_pumpkinLifetime > 75)
		{
			return;
		}
		else if (_pumpkinLifetime <= 75 && _pumpkinLifetime > 50)
		{
			setPumpkinSprite(3);
		}
		else if (_pumpkinLifetime <= 50 && _pumpkinLifetime > 25)
		{
			setPumpkinSprite(4);
		}
		else if (_pumpkinLifetime <= 25 && _pumpkinLifetime > 0)
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
