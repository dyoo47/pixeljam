using Godot;
using System;

public partial class Projectile : Sprite2D
{
	[Export] public float StartVelocity = 50.0f;
	[Export] public string TargetGroup = "enemy";
	[Export] public int Damage = 16;
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
		_Area.BodyEntered += OnProjectileHit;
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
		LookAt(Velocity + Position);
	}

	public void OnProjectileHit(Node2D body)
	{
		if (!body.IsInGroup(TargetGroup))
		{
			return;
		}
		if(TargetGroup == "player")
		{
			Player player = (Player)body;
			player.Hit(Damage);
			OnTargetHit(body);
		}
		else if(TargetGroup == "enemy")
		{
			Entity enemy = (Entity) body;
			Vector2 pointing = (enemy.Position - Position).Normalized();
			enemy.Hit(pointing * new Vector2(100, 100), 0.5f, Damage);
			OnTargetHit(body);
		}
		
		QueueFree();
	}

	public virtual void OnTargetHit(Node2D body)
	{

	}

	public void Knockback(Vector2 vel)
	{
		Velocity += vel;
	}

}
