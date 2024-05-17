using Godot;
using System;

public partial class CannonEnemy : Entity
{
	[Export] public PackedScene projectileScene;
	private Node2D projectileSpawn;
	private float shootDistance = 400.0f;
	private float projectileSpeed = 200.0f;
	private AnimatedSprite2D eyes;
	private AnimatedSprite2D wheels;
	private double aimTime = 1.0f;
	private double elapsedAimTime = 0.0f;

	public override void _Ready()
	{
		base._Ready();
		projectileSpawn = Sprite.GetNode<Node2D>("ProjectileSpawn");
		eyes = SpriteContainer.GetNode<AnimatedSprite2D>("Eyes");
		wheels = SpriteContainer.GetNode<AnimatedSprite2D>("Wheels");
		eyes.Play("idle");
		wheels.Play();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		switch (State)
		{
			case EntityState.Idle:
				if(Position.DistanceTo(Game.MainPlayer.Position) < shootDistance)
				{
					elapsedAimTime = 0;
					State = EntityState.Aiming;
				}
				break;
			case EntityState.Aiming:
				elapsedAimTime += delta;
				Sprite.Play("aiming");
				Sprite.LookAt(Game.MainPlayer.Position);

				if(elapsedAimTime >= aimTime)
				{
					State = EntityState.Attacking;
					Projectile projInstance = projectileScene.Instantiate<Projectile>();
					projInstance.Position = projectileSpawn.GlobalPosition;
					projInstance.Rotation = Rotation;
					GetTree().Root.AddChild(projInstance);
					Vector2 direction = (Game.MainPlayer.Position - GlobalPosition).Normalized();
					projInstance.Knockback(direction * projectileSpeed);

				}
				break;
			case EntityState.Attacking:
				Sprite.LookAt(Game.MainPlayer.Position);
				break;
			case EntityState.Dead:
				eyes.Visible = false;
				wheels.Visible = false;
				break;
		}
	}

}
