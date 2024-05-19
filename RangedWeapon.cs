using Godot;
using System;

public partial class RangedWeapon : Weapon
{

	[Export] public PackedScene ProjectileScene;
	[Export] public float ProjectileSpeed = 250.0f;
	private AnimationPlayer _AnimationPlayer;
	private Node2D _ProjectileSpawnPoint;
	public int waterCost = 10;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_ProjectileSpawnPoint = GetNode<Node2D>("ProjectileSpawnPoint");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void Attack()
	{
		Projectile projInstance = ProjectileScene.Instantiate<Projectile>();
		projInstance.Position = _ProjectileSpawnPoint.GlobalPosition;
		projInstance.Rotation = Rotation;
		GetTree().Root.AddChild(projInstance);
		Vector2 direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();
		projInstance.Knockback(direction * ProjectileSpeed);
		_AnimationPlayer.Play("shoot");
		Play();
	}
}
