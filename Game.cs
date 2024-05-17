using Godot;
using pixeljam;
using System;

public partial class Game : Node2D
{
	[Export] public Player PlayerScene;
	[Export] public Node2D HealthFlask;
	[Export] public Node2D WaterFlask;
	public static Player MainPlayer;
	public static Node2D MainNode;
	public float healthFlaskHeight;
	public float healthFlaskMaxHeight = 100.0f;
	public float waterFlaskHeight;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthFlaskHeight = healthFlaskMaxHeight;
		MainPlayer = PlayerScene;
		MainNode = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		healthFlaskHeight = Util.lerp(healthFlaskHeight, MainPlayer.Health / (float) MainPlayer.MaxHealth * healthFlaskMaxHeight, (float)delta * 10);
		(HealthFlask.Material as ShaderMaterial).SetShaderParameter("px_height", healthFlaskHeight);
	}

	public static Node2D GetMain()
	{
		return MainNode;
	}
}
