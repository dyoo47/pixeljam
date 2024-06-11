using Godot;
using System;

public partial class Destructible : CharacterBody2D
{

	[Export] public PackedScene fxScene;
	[Export] public Godot.Collections.Array<PackedScene> drops;
	public void OnDestroy()
	{
		Game.PlaySound("res://sound/crate_smash.wav");
		QueueFree();
		if (drops.Count > 0)
		{
			Node2D dropInstance = drops.PickRandom().Instantiate<Node2D>();
			dropInstance.Position = GlobalPosition;
			Game.MainNode.AddChild(dropInstance);
		}
		Node2D fxInstance = fxScene.Instantiate<Node2D>();
		fxInstance.Position = GlobalPosition;
		Game.MainNode.AddChild(fxInstance);
	}
}

