using Godot;
using pixeljam;
using System;

public partial class ConsumablePickup : Sprite2D
{
	[Export] public int consumableIndex = 0;
	private double pickupTime = 1;
	private double elapsedTime = 0;
	private bool pickedUp = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (pickedUp)
		{
			elapsedTime += delta;
			float scale = 1 - Util.easeOutElastic(elapsedTime / pickupTime);
			Scale = new Vector2(scale, scale);
		}
		if(elapsedTime >= pickupTime)
		{
			QueueFree();
		}
	}

	private void _on_area_2d_body_entered(Node2D body)
	{
		if (body.IsInGroup("player") && !pickedUp)
		{
			Game.PlaySound("res://sound/consumable_pickup.wav");
			pickedUp = true;
			Game.MainPlayer.GiveConsumable(consumableIndex);
		}
	}

}
