using Godot;
using System;

public partial class MustardEnemy : Area2D
{
	private Vector2 Velocity;
	private float Damping = 0.005f;
	private float Epsilon = 0.0001f;
	private AnimatedSprite2D Sprite;
	private float _RedShift = 0f;
	private float _RedShiftSpeed = 2.0f;
	private EntityState _State;

	private double _StunTime = 0.5;
	private double _ElapsedStunTime = 0;

	private PackedScene _DamageIndicatorScene;
	private CpuParticles2D _DeathParticles;
	private CpuParticles2D _HitParticles;

	public int Health = 21;

	enum EntityState
	{
		Idle,
		Hurt,
		Dying,
		Dead
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_ElapsedStunTime = _StunTime;
		_DamageIndicatorScene = ResourceLoader.Load<PackedScene>("res://damage_indicator.tscn");
		Velocity = new Vector2 (0, 0);
		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Sprite.AnimationFinished += OnAnimationFinished;
		_DeathParticles = GetNode<CpuParticles2D>("DeathParticles");
		_DeathParticles.Finished += OnParticlesFinished;
		_HitParticles = GetNode<CpuParticles2D>("HitParticles");
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

		_RedShift = Math.Max(0, _RedShift - _RedShiftSpeed * (float) delta);
		(Sprite.Material as ShaderMaterial).SetShaderParameter("red", _RedShift);


		_ElapsedStunTime += delta;
		Sprite.Scale = new Vector2(1, pixeljam.Util.easeOutElastic(_ElapsedStunTime / _StunTime));
		switch(_State)
		{
			case EntityState.Idle: Sprite.Play("idle"); break;
			case EntityState.Hurt: 
				Sprite.Play("hit");
				if(_ElapsedStunTime >= _StunTime)
				{
					_State = EntityState.Idle;
				}
				if (Health <= 0) _State = EntityState.Dying;
				break;
			case EntityState.Dying:
				Sprite.Play("death");
				break;
			case EntityState.Dead:
				break;
		}
	}

	public void OnParticlesFinished()
	{
		QueueFree();
	}

	public void OnAnimationFinished()
	{
		if(Sprite.Animation == "death")
		{
			_State = EntityState.Dead;
			Sprite.Visible = false;
			_DeathParticles.Restart();
		}
	}

	public void Hit(Vector2 vel, float stunTime, int damage)
	{
		if (_State == EntityState.Dying || _State == EntityState.Dead) return;
		Health -= damage;
		Knockback(vel);
		_RedShift = 1.0f;
		_State = EntityState.Hurt;
		_StunTime = stunTime;
		_ElapsedStunTime = 0;
		DamageIndicator damageIndicator = _DamageIndicatorScene.Instantiate<DamageIndicator>();
		damageIndicator.StartPosition = Position;
		damageIndicator.Text = damage.ToString();
		GetTree().Root.AddChild(damageIndicator);
		_HitParticles.Restart();
	}

	public void Knockback(Vector2 vel)
	{
		Velocity += vel;
	}
}
