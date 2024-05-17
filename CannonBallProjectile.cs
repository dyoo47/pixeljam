using Godot;
using System;

public partial class CannonBallProjectile : Projectile
{
	[Export] public PackedScene fxScene;
	public override void OnTargetHit(Node2D body)
	{
		Node2D fxInstance = fxScene.Instantiate<Node2D>();
		fxInstance.Position = Position;
		Game.MainNode.AddChild(fxInstance);
	}
}
