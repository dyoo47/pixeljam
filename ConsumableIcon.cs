using Godot;
using pixeljam;
using System;

public partial class ConsumableIcon : Sprite2D
{
	[Export] public ConsumableType type;
	AnimationPlayer _player;
	RichTextLabel _label;
	public int count = 0;
	private float redTime = 1.0f;
	private float elapsedRedTime = 0.0f;

	public enum ConsumableType
	{
		Potion,
		WaterBottle,
		Bomb
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetNode<AnimationPlayer>("AnimationPlayer");
		_label = GetNode<RichTextLabel>("RichTextLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		elapsedRedTime += (float) delta;
		(Material as ShaderMaterial).SetShaderParameter("red", 1 - Util.easeOutExpo(elapsedRedTime / redTime));
		_label.Text = count.ToString();
	}

	public void Wiggle()
	{
		_player.Stop();
		_player.Play("wiggle");
	}

	public void RedShift()
	{
		elapsedRedTime = 0;
	}

	public void Use()
	{
		switch (type)
		{
			case ConsumableType.Potion:
				Game.MainPlayer.AddHealth(20);
				break;
			case ConsumableType.WaterBottle:
				Game.MainPlayer.AddWater(20);
				break;
			case ConsumableType.Bomb:
				Game.MainPlayer.ThrowBomb();
				break;
		}
	}


}
