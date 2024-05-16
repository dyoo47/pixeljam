using Godot;
using pixeljam;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public CollisionShape2D hitCollision;
	[Export] public Area2D hitArea;
	[Export] public Weapon meleeWeapon;
	[Export] public RangedWeapon rangedWeapon;
	[Export] public PackedScene slashEffectScene;
	[Export] public CpuParticles2D particles;
	[Export] public PackedScene damageIndicatorScene;
	[Export] public int Speed { get; set; } = 200;


	public float weaponRotationOffset = 1.2f;
	public Vector2 ScreenSize;
	private bool canAttack = true;
	private Node2D _weaponContainer;
	private Node2D _rangedWeaponContainer;
	private Node2D _heading;
	private Node2D _slashSpawnPoint;
	private float _targetRotation;
	private AnimatedSprite2D _sprite;
	private float redshift = 0.0f;
	private float redshiftSpeed = 2.0f;

	private double hitTime = 1.0f;
	private double elapsedHitTime = 0.0f;

	private Timer _timer;
	private Timer _attackTimer;
	private bool _left = true;
	private float _attackCooldown = 0.3f;
	private float _attackActiveLength = 0.2f;
	private bool _usingMelee = true;

	private WeaponState _weaponState = WeaponState.Equipping;
	private double equipTime = 0.5;
	private double elapsedEquipTime = 0;

	enum WeaponState
	{
		Equipping,
		Ready,
		Cooldown
	}

	
	public override void _Ready()
	{
		elapsedHitTime = hitTime;
		ScreenSize = GetViewportRect().Size;
		_weaponContainer = GetNode<Node2D>("WeaponContainer");
		_heading = GetNode<Node2D>("Heading");
		_rangedWeaponContainer = _heading.GetNode<Node2D>("RangedWeaponContainer");
		_slashSpawnPoint = _heading.GetNode<Node2D>("SlashSpawnPoint");
		_timer = GetNode<Timer>("Timer");
		_timer.WaitTime = _attackCooldown;
		_attackTimer = GetNode<Timer>("AttackTimer");
		_attackTimer.WaitTime = _attackActiveLength;
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _Process(double delta)
	{
		redshift = Math.Max(0, redshift - redshiftSpeed * (float) delta);
		(_sprite.Material as ShaderMaterial).SetShaderParameter("red", redshift);

		elapsedHitTime += delta;
		float _scale = pixeljam.Util.easeOutElastic(elapsedHitTime / hitTime);
		_sprite.Scale = new Vector2(_scale, _scale);


		_weaponContainer.LookAt(GetGlobalMousePosition());
		_heading.LookAt(GetGlobalMousePosition());
		
		if(Position.X > GetGlobalMousePosition().X)
		{
			_sprite.FlipH = true;
			rangedWeapon.FlipV = true;
		}
		else
		{
			_sprite.FlipH = false;
			rangedWeapon.FlipV = false;
		}

		if(_left)
		{
			_weaponContainer.Scale = new Vector2(1, -1);
			_targetRotation = _heading.Rotation + weaponRotationOffset;
		}
		else
		{
			_weaponContainer.Scale = new Vector2(1, 1);
			_targetRotation = _heading.Rotation - weaponRotationOffset;
		}


		float difference = _targetRotation - _weaponContainer.Rotation;
		float x = (float)(_timer.TimeLeft) / _attackCooldown;

		if(_usingMelee)
		{
			meleeWeapon.Visible = true;
			rangedWeapon.Visible = false;
		}
		else
		{
			meleeWeapon.Visible = false;
			rangedWeapon.Visible = true;
		}

		// Lerp
		//_weaponContainer.Rotation = (float)(_weaponContainer.Rotation + difference * ((_attackCooldown-_timer.TimeLeft) / _attackCooldown));
		// Quintic
		_weaponContainer.Rotation = (float)(1 - Math.Pow(x, 5)) * difference + _weaponContainer.Rotation;

				switch(_weaponState)
		{
			case WeaponState.Equipping:
				elapsedEquipTime += delta;
				float scale = Util.easeOutExpo(elapsedEquipTime / equipTime);
				meleeWeapon.Scale = new Vector2(scale, -scale);
				rangedWeapon.Scale = new Vector2(scale, scale);
				rangedWeapon.Rotation =(float) Math.PI * 2 * scale;
				if(elapsedEquipTime >= equipTime)
				{
					_weaponState = WeaponState.Ready;
				}
				break;
			case WeaponState.Ready:
				if(Input.IsActionJustPressed("swap_weapon"))
				{
					_weaponState = WeaponState.Equipping;
					elapsedEquipTime = 0;
					_usingMelee = !_usingMelee;
				}
				if (Input.IsActionJustPressed("attack"))
				{
					if (_usingMelee)
					{
						Node2D instance = (Node2D)slashEffectScene.Instantiate();
						GetTree().Root.AddChild(instance);
						instance.Position = _slashSpawnPoint.GlobalPosition;
						instance.Rotation = _heading.Rotation;
						meleeWeapon.Attack();
						_timer.Start();
						_left = !_left;
						_attackTimer.Start();
						hitCollision.Disabled = false;
					}
					else
					{
						rangedWeapon.Attack();
					}
				}
				break;
		}


				
		if(_attackTimer.TimeLeft <= 0)
		{
			hitCollision.Disabled = true;
		}
	

		var dir = Vector2.Zero;
		if (Input.IsActionPressed("move_right"))
		{
			dir.X += 1;
		}

		if (Input.IsActionPressed("move_left"))
		{
			dir.X -= 1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			dir.Y += 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			dir.Y -= 1;
		}
		
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (dir.Length() > 0)
		{
			dir = dir.Normalized() * Speed;
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}

		Velocity = dir;
		MoveAndSlide();
	}

	public void Hit(int damage)
	{
		DamageIndicator damageIndicator = damageIndicatorScene.Instantiate<DamageIndicator>();
		damageIndicator.StartPosition = Position;
		damageIndicator.Text = damage.ToString();
		GetTree().Root.AddChild(damageIndicator);
		redshift = 1.0f;
		elapsedHitTime = 0;
	}
	
	private void OnWeaponHit(Node2D body)
	{
		if(!body.IsInGroup("enemy"))
		{
			return;
		}
		Entity enemy = (Entity) body;
		Vector2 pointing = (enemy.Sprite.GlobalPosition - _sprite.GlobalPosition).Normalized();
		enemy.Hit(pointing * 200.0f, 0.5f, 7);
	}
	
	private void OnTimeout()
	{
		// Replace with function body.
	}
}
