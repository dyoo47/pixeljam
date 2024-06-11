using Godot;
using System;

public partial class DeathLabel : RichTextLabel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Text = "[center]You have been slain!\nLevel Achieved: " + Game.MainPlayer.Level + 
			"\nScreens Cleared: " + Game.ScreensCleared + 
			"\nMonsters Slain: " + Game.MonstersSlain + " [/center]";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
