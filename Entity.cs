using Godot;
using System;

public partial class Entity : CharacterBody2D
{
	[Export] public bool UseNavigation = true;
	protected NavigationAgent2D Agent;
	[Export] public float MoveSpeed = 100.0f;
	[Export] public bool IsEnemy = true;
	private float Damping = 0.005f;
	private float Epsilon = 0.0001f;
	public AnimatedSprite2D Sprite;
	public Node2D SpriteContainer;
	private float _RedShift = 0f;
	private float _RedShiftSpeed = 2.0f;
	protected EntityState State;
	protected float ScaleCoef = 1.0f;

	private double _StunTime = 0.5;
	private double _ElapsedStunTime = 0;

	private AnimationPlayer HurtBoxAnim;

	private PackedScene _DamageIndicatorScene;
	private CpuParticles2D _DeathParticles;
	private CpuParticles2D _HitParticles;

	public BlockManager blockOwner;

	[Export] public int Health = 21;

	protected enum EntityState
	{
		Idle,
		Hurt,
		Attacking,
		Aiming,
		Dying,
		Dead
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(UseNavigation)
		{
			Agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
			Agent.TargetPosition = new Vector2(0, 0);
		}
		SpriteContainer = GetNode<Node2D>("SpriteContainer");
		HurtBoxAnim = GetNode<AnimationPlayer>("AnimationPlayer");
		_ElapsedStunTime = _StunTime;
		_DamageIndicatorScene = ResourceLoader.Load<PackedScene>("res://damage_indicator.tscn");
		Velocity = new Vector2 (0, 0);
		Sprite = GetNode<Node2D>("SpriteContainer").GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Sprite.AnimationFinished += OnAnimationFinished;
		_DeathParticles = GetNode<CpuParticles2D>("DeathParticles");
		_DeathParticles.Finished += OnParticlesFinished;
		_HitParticles = GetNode<CpuParticles2D>("HitParticles");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		_RedShift = Math.Max(0, _RedShift - _RedShiftSpeed * (float) delta);
		(Sprite.Material as ShaderMaterial).SetShaderParameter("red", _RedShift);


		_ElapsedStunTime += delta;
		SpriteContainer.Scale = new Vector2(ScaleCoef, pixeljam.Util.easeOutElastic(_ElapsedStunTime / _StunTime));
		switch(State)
		{
			case EntityState.Idle: 
				Sprite.Play("idle");
				if (UseNavigation)
				{
					Navigate();
				}
				break;
			case EntityState.Hurt: 
				Sprite.Play("hit");
				if(_ElapsedStunTime >= _StunTime)
				{
					State = EntityState.Idle;
				}
				if (Health <= 0)
				{
					State = EntityState.Dying;
					GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
				}
				
				break;
			case EntityState.Attacking:
				Sprite.Play("attack");
				HurtBoxAnim.Play("attack");
				break;
			case EntityState.Dying:
				Sprite.Play("death");
				break;
			case EntityState.Dead:
				break;
		}
		float damp = (float)Math.Pow(Damping, delta);
		Velocity *= damp;
		if (Math.Abs(Velocity.X) < Epsilon)
		{
			Velocity = new Vector2(0, Velocity.Y);
		}
		if (Math.Abs(Velocity.Y) < Epsilon)
		{
			Velocity = new Vector2(Velocity.X, 0);
		}

		MoveAndSlide();
	}

	public void OnParticlesFinished()
	{
		QueueFree();
	}

	public void OnAnimationFinished()
	{
		if(Sprite.Animation == "death")
		{
			//(GetTree().GetFirstNodeInGroup("block_manager") as BlockManager).OnEnemyDeath();
			blockOwner.OnEnemyDeath();
			State = EntityState.Dead;
			Sprite.Visible = false;
			_DeathParticles.Restart();
			Game.MainPlayer.AddXp(10);
		}else if(Sprite.Animation == "attack")
		{
			State = EntityState.Idle;
		}
	}

	public void Hit(Vector2 vel, float stunTime, int damage)
	{
		if (State == EntityState.Dying || State == EntityState.Dead) return;
		Health -= damage;
		Knockback(vel);
		_RedShift = 1.0f;
		State = EntityState.Hurt;
		_StunTime = stunTime;
		_ElapsedStunTime = 0;
		DamageIndicator damageIndicator = _DamageIndicatorScene.Instantiate<DamageIndicator>();
		damageIndicator.StartPosition = GlobalPosition;
		damageIndicator.Text = damage.ToString();
		Game.MainNode.AddChild(damageIndicator);
		_HitParticles.Restart();
	}

	public void Knockback(Vector2 vel)
	{
		Velocity += vel;
	}

	public virtual void Navigate()
	{

	}
}
