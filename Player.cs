using Godot;
using pixeljam;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public CollisionShape2D hitCollision;
	[Export] public Area2D hitArea;
	[Export] public Area2D refillArea;
	[Export] public Weapon meleeWeapon;
	[Export] public RangedWeapon rangedWeapon;
	[Export] public PackedScene slashEffectScene;
	[Export] public CpuParticles2D particles;
	[Export] public PackedScene damageIndicatorScene;
	[Export] public CpuParticles2D refillParticles;
	[Export] public CpuParticles2D xpParticles;
	[Export] public CpuParticles2D healthParticles;
	[Export] public PackedScene BombScene;
	[Export] public int Speed { get; set; } = 200;

	public int MaxHealth = 20;
	public int Health = 20;
	public float MaxWater = 50;
	public float Water = 50;
	public float MaxXp = 50;
	public float Xp = 0;
	public int XpStatLinear = 2;
	public float XpLimitMult = 1.1f;
	public int Level = 1;
	public int Attack = 7;

	public int[] ConsumableCounts = new int[10];

	private bool _refilling = false;
	public float refillRate = 50;

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
	private float _attackCooldown = 0.5f;
	private float _attackActiveLength = 0.5f;
	private float _minAttackSpeed = 0.1f;
	private bool _usingMelee = true;

	private WeaponState _weaponState = WeaponState.Equipping;
	private double equipTime = 0.5;
	private double elapsedEquipTime = 0;

	private AudioStreamPlayer2D refillSound;
	private bool dead = false;

	enum WeaponState
	{
		Equipping,
		Ready,
		Cooldown
	}

	
	public override void _Ready()
	{
		refillSound = GetNode<AudioStreamPlayer2D>("RefillAudio");
		ConsumableCounts[0] = 0;
		ConsumableCounts[1] = 0;
		ConsumableCounts[2] = 0;
		ConsumableCounts[3] = 0;
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
		if (dead) return;
		_attackTimer.WaitTime = _attackActiveLength;
		_refilling = false;
		foreach(Area2D area in refillArea.GetOverlappingAreas()){
			if (area.IsInGroup("refill") && Water < MaxWater)
			{
				_refilling = true;
				break;
			}
		}
		refillParticles.Emitting = _refilling;
		if (_refilling)
		{
			if(!refillSound.Playing) refillSound.Play();
			Water += refillRate * (float) delta;
			if(Water > MaxWater) Water = MaxWater;
		}

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
					Game.PlaySound("res://sound/swap_weapon.wav");
					_weaponState = WeaponState.Equipping;
					elapsedEquipTime = 0;
					_usingMelee = !_usingMelee;
				}
				if (Input.IsActionPressed("attack") && _attackTimer.TimeLeft <= 0 && _usingMelee)
				{
					if (_usingMelee)
					{
						Game.PlaySound("res://sound/hit.wav");
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
				}
				if (Input.IsActionJustPressed("attack") && !_usingMelee)
				{

					if(Water >= rangedWeapon.waterCost || ConsumableCounts[1] > 0)
					{
						Water -= rangedWeapon.waterCost;
						rangedWeapon.Attack();
					}
				}
				break;
		}


				
		if(_attackTimer.TimeLeft <= 0.05f)
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

		if (Input.IsActionJustPressed("use_1") || Health <= 0) UseConsumable(0);
		if (Input.IsActionJustPressed("use_2") || Water <= 0) UseConsumable(1);
		if (Input.IsActionJustPressed("use_3")) UseConsumable(2);

		Velocity = dir;
		MoveAndSlide();

		if(Xp > MaxXp)
		{
			var diff = MaxXp - Xp;
			Xp = diff;
			MaxXp = (int)Math.Round(MaxXp * XpLimitMult);
			_attackActiveLength -= 0.03f;
			if(_attackActiveLength < _minAttackSpeed) _attackActiveLength = _minAttackSpeed;
			Level++;
			Attack += XpStatLinear;
			MaxHealth += XpStatLinear * 2;
			Health = MaxHealth;
			MaxWater += XpStatLinear * 2;
			Water = MaxWater;
			Game.MainNode.PlayLevelAnim();
			xpParticles.Restart();
			SpawnIndicator("Level up!");
			Game.PlaySound("res://sound/levelup.wav");
			if(Level == 5)
			{
				Game.AddHardLevels();
			}
		}

		if(Health <= 0)
		{
			Visible = false;
			hitCollision.Disabled = true;
			dead = true;
			Game.Death();
			foreach(Node node in Game.MainNode.GetChildren()){
				if (node.IsInGroup("no_pause")) continue;
				node.SetProcess(false);
			}
		}
	}

	public void SpawnIndicator(String text)
	{
		DamageIndicator damageIndicator = damageIndicatorScene.Instantiate<DamageIndicator>();
		damageIndicator.StartPosition = Position;
		damageIndicator.Text = text;
		GetTree().Root.AddChild(damageIndicator);

	}

	public void Hit(int damage)
	{
		Game.PlaySound("res://sound/player_hit.wav");
		DamageIndicator damageIndicator = damageIndicatorScene.Instantiate<DamageIndicator>();
		damageIndicator.StartPosition = Position;
		damageIndicator.Text = damage.ToString();
		GetTree().Root.AddChild(damageIndicator);
		Health -= damage;
		redshift = 1.0f;
		elapsedHitTime = 0;
	}
	
	private void OnWeaponHit(Node2D body)
	{
		if(body.IsInGroup("enemy"))
		{
			Entity enemy = (Entity) body;
			Vector2 pointing = (enemy.Sprite.GlobalPosition - _sprite.GlobalPosition).Normalized();
			enemy.Hit(pointing * 200.0f, 0.5f, Attack);
		}else if (body.IsInGroup("destructible"))
		{
			Destructible destructible = (Destructible) body;
			destructible.OnDestroy();
		}
	}

	private void UseConsumable(int index)
	{
		ConsumableIcon icon = Game.MainNode.ConsumableIcons[index];
		icon.Wiggle();
		if (ConsumableCounts[index] == 0)
		{
			icon.RedShift();
		}
		else
		{
			ConsumableCounts[index]--;
			icon.Use();
		}
	}

	public void GiveConsumable(int index)
	{
		ConsumableIcon icon = Game.MainNode.ConsumableIcons[index];
		icon.Wiggle();
		ConsumableCounts[index]++;
	}

	public void AddHealth(int health)
	{
		Health += health;
		if(Health > MaxHealth)
		{
			Health = MaxHealth;
		}
	}

	public void AddWater(int water)
	{
		Water += water;
		if(Water > MaxWater) { 
			Water = MaxWater;
		}
	}

	public void AddXp(int xp)
	{
		Xp += xp;
		
	}

	public void RestoreFullHealth()
	{
		SpawnIndicator("Full heal!");
		healthParticles.Restart();
		Health = MaxHealth;
	}

	public void RestoreFullWater()
	{
		SpawnIndicator("Max refill!");
		Water = MaxWater;
	}


	public void ThrowBomb()
	{
		Projectile projInstance = BombScene.Instantiate<Projectile>();
		projInstance.Position = GlobalPosition;
		projInstance.Rotation = Rotation;
		GetTree().Root.AddChild(projInstance);
		Vector2 direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();
		projInstance.Knockback(direction * 200);
	}
}

