using Godot;
using System;

public partial class DamageIndicator : RichTextLabel
{
	float RiseDistance = 10f;
	float RiseSpeed = 10f;
	double Lifetime = 0.5;
	double ElapsedLifetime = 0;
	public Vector2 StartPosition;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float scale = pixeljam.Util.easeOutExpo(ElapsedLifetime / Lifetime);
		float tint = pixeljam.Util.easeInOutQuad(ElapsedLifetime / Lifetime);
		Position = StartPosition + new Vector2(0, -RiseDistance * scale);
		StartPosition += new Vector2(0, -RiseSpeed * (float) delta);
		Scale = new Vector2(scale, scale);
		ElapsedLifetime += delta;
		(Material as ShaderMaterial).SetShaderParameter("tint", tint);
		if(ElapsedLifetime >= Lifetime)
		{
			QueueFree();
		}
	}
}
