using Godot;
using System;

public partial class ScoreLabel : Label
{
    private int _score = 0;

    public override void _Ready()
    {
        // Initialize the text display right away
        UpdateScoreDisplay();
    }

    // Call this public method from other game objects to increase the score
    public void AddPoints(int points)
    {
        var Globals = GetNode<Globals>("/root/Globals");
		Globals.Score += 1;
        _score += points;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        Text = $"Score: {_score}";
    }
}