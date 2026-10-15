using Godot;
using System;

public partial class TimerLabel : Label
{
	// Set the starting match/level time in seconds (e.g., 2 minutes)
	[Export] 
	private int _timeRemaining = 120; 

	public override void _Ready()
	{
		UpdateTimerDisplay();
	}

	// This method runs every time the Timer node ticks (every 1 second)
	public void OnTimerTimeout()
	{
		GD.Print("called OnTimerTimeout()");

		if (_timeRemaining > 0)
		{
			_timeRemaining--;
			UpdateTimerDisplay();
		}
		else
		{
			TriggerGameOver();
		}
	}

	private void UpdateTimerDisplay()
	{
		// Format seconds into typical MM:SS display style
		int minutes = _timeRemaining / 60;
		int seconds = _timeRemaining % 60;
		Text = $"Time: {minutes:D2}:{seconds:D2}";
	}

	private void TriggerGameOver()
	{
		GD.Print("Time is up! Game Over.");
		// Add your game-over logic here (e.g., pause game, load menu scene)
	}
}
