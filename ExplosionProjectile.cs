using Godot;
using System;

public partial class ExplosionProjectile : Node2D
{
	private double lifetime = 1;
	private double elapsedTime = 0;

	public override void _Process(double delta)
	{
		elapsedTime += delta;
		if(elapsedTime > lifetime)
		{
			QueueFree();
		}
		if(elapsedTime > 0.2f)
		{
			GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = true;
		}
	}

	private void _on_area_2d_body_entered(Node2D body)
	{
		if (body.IsInGroup("enemy"))
		{
			Entity enemy = (Entity) body;
			Vector2 pointing = (enemy.Position - Position).Normalized();
			enemy.Hit(pointing * new Vector2(100, 100), 0.5f, 60);
		}
	}
}



