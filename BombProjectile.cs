using Godot;
using System;

public partial class BombProjectile : Projectile
{
	[Export] PackedScene fxScene;

	public void OnExplode()
	{
		Node2D fxInstance = fxScene.Instantiate<Node2D>();
		fxInstance.Position = Position;
		Game.MainNode.AddChild(fxInstance);
		QueueFree();
	}
}



