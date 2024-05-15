using Godot;
using System;

public partial class Projectile : Sprite2D
{
	[Export] public float StartVelocity = 50.0f;
	private AnimationPlayer _AnimationPlayer;
	public Vector2 Velocity;
	private float Damping = 1.0f;
	private float Epsilon = 0.0001f;
	private Area2D _Area;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_AnimationPlayer.Play("moving");
		_Area = GetNode<Area2D>("Area2D");
		_Area.AreaEntered += OnProjectileHit;
		Velocity = new Vector2(0, 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += Velocity * (float)delta;
		Velocity.X = Velocity.X * (float)Math.Pow(Damping, delta);
		if (Math.Abs(Velocity.X) < Epsilon)
		{
			Velocity.X = 0;
		}
		Velocity.Y = Velocity.Y * (float)Math.Pow(Damping, delta);
		if (Math.Abs(Velocity.Y) < Epsilon)
		{
			Velocity.Y = 0;
		}
	}

	public void OnProjectileHit(Area2D area)
	{
		if (!area.IsInGroup("enemy"))
		{
			return;
		}
		MustardEnemy enemy = (MustardEnemy) area;
		Vector2 pointing = (enemy.Position - Position).Normalized();
		enemy.Hit(pointing * new Vector2(100, 100), 0.5f, 12);
	}

	public void Knockback(Vector2 vel)
	{
		Velocity += vel;
	}

}
