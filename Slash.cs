using Godot;
using System;

public partial class Slash : Node2D
{
	AnimatedSprite2D sprite;
	public float velocity = 100.0f;
	public float bufferVelocity = 0f;
	public double lifetime = 0.8f;
	private double timeLived;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LookAt(GetGlobalMousePosition());
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play();
		timeLived = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timeLived += delta;
		var x = timeLived / lifetime;
		var comp = Math.Pow(1 - x, 5) * velocity * delta + bufferVelocity;
		//Position += Transform.X * new Vector2((float)(velocity * delta), (float)(velocity * delta));
		Position += Transform.X * new Vector2((float)comp, (float)comp);
		if(timeLived >= lifetime)
		{
			QueueFree();
		}
	}

	private void _on_animated_sprite_2d_animation_finished()
	{
		QueueFree();
	}

}



