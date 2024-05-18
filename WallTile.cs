using Godot;
using System;

public partial class WallTile : AnimatedSprite2D
{
	// Called when the node enters the scene tree for the first time.
	private CollisionShape2D _collisionShape;
	public override void _Ready()
	{
		_collisionShape = GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void SetCollision(bool value)
	{
		_collisionShape.Disabled = value;
	}

	public void OnPlayerEnter()
	{
		CallDeferred(MethodName.SetCollision, false);
		Play("rise");
		GetNode<CpuParticles2D>("Particles").Restart();
	}

	public void OnBlockCleared()
	{
		CallDeferred(MethodName.SetCollision, true);
		Play("fall");
		GetNode<CpuParticles2D>("Particles").Restart();
	}
}
