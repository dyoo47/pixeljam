using Godot;
using System;
using pixeljam;

public partial class Explosion : Node2D
{
	private CpuParticles2D _particles;
	private CpuParticles2D _particles1;
	private Sprite2D _sprite;
	[Export] public double explosionTime = 1.0f;
	private double elapsedTime = 0.0f;
	[Export] public float minRadius = 8.0f;
	[Export] public float maxRadius = 80f;
	[Export] public float minBorderWidth = 0f;
	[Export] public float maxBorderWidth = 8.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_particles = GetNode<CpuParticles2D>("Primary");
		_particles1 = GetNode<CpuParticles2D>("Secondary");
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_particles.Restart();
		_particles1.Restart();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		elapsedTime += delta;
		if(elapsedTime > explosionTime)
		{
			QueueFree();
		}
		float diff = maxRadius - minRadius;
		(_sprite.Material as ShaderMaterial).SetShaderParameter("radius", minRadius + maxRadius * Util.easeOutExpo(elapsedTime / explosionTime));
		(_sprite.Material as ShaderMaterial).SetShaderParameter("border_width", minBorderWidth + maxBorderWidth * Util.easeInSin(1.0f - elapsedTime / explosionTime));
	}
}
