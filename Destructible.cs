using Godot;
using System;

public partial class Destructible : CharacterBody2D
{

	[Export] public PackedScene fxScene;
	[Export] public Godot.Collections.Array<PackedScene> drops;
	public void OnDestroy()
	{
		Node2D dropInstance = drops.PickRandom().Instantiate<Node2D>();
		dropInstance.Position = GlobalPosition;
		Node2D fxInstance = fxScene.Instantiate<Node2D>();
		fxInstance.Position = GlobalPosition;
		Game.MainNode.AddChild(fxInstance);
		Game.MainNode.AddChild(dropInstance);
		QueueFree();
	}
}

