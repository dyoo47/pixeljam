using Godot;
using System;

public partial class MeleeEnemy : Entity
{
	private Area2D HurtBox;
	private CollisionShape2D HurtShape;
	private Area2D AttackBox;
	private CollisionShape2D AttackShape;
	private bool OverlappingPlayer = false;

	public override void _Ready()
	{
		base._Ready();
		HurtBox = GetNode<Area2D>("HurtBox");
		HurtShape = GetNode<CollisionShape2D>("CollisionShape2D");
		AttackBox = GetNode<Area2D>("AttackBox");
		AttackShape = AttackBox.GetNode<CollisionShape2D>("CollisionShape2D");
		AttackBox.BodyEntered += OnAttackBoxEnter;
		AttackBox.BodyExited += OnAttackBoxExit;
		HurtBox.BodyEntered += OnHurtBoxEnter;
	}

	public override void Navigate()
	{
		Agent.TargetPosition = Game.MainPlayer.Position;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if(Agent.TargetPosition.X > Position.X)
		{
			ScaleCoef = 1.0f;
			HurtBox.Scale = new Vector2(1, 1);
			AttackBox.Scale = new Vector2(1, 1);
		}
		else
		{
			ScaleCoef = -1.0f;
			HurtBox.Scale = new Vector2(-1, 1);
			AttackBox.Scale = new Vector2(-1, 1);
		}

		switch (State)
		{
			case EntityState.Idle:
				AttackShape.Disabled = false;
				if (OverlappingPlayer)
				{
					State = EntityState.Attacking;
				}
				break;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Agent.IsNavigationFinished() || State != EntityState.Idle) return;
		var next = GlobalPosition.DirectionTo(Agent.GetNextPathPosition());
		Velocity = next * MoveSpeed;
		Agent.Velocity = Velocity;
	}

	private void OnAttackBoxEnter(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			OverlappingPlayer = true;
		}
	}

	private void OnAttackBoxExit(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			OverlappingPlayer = false;
		}
	}

	private void OnHurtBoxEnter(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			Player player = (Player)body;
			player.Hit(11);
		}
	}
}
