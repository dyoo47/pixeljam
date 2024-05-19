using Godot;
using System;
using System.Linq;

public partial class BlockManager : Node2D
{
	private TileMap tileMap;
	bool cleared = false;
	bool entered = false;
	int enemyCount;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup("block_manager");
		tileMap = GetNode<TileMap>("TileMap");
		enemyCount = GetTree().GetNodesInGroup("spawner").Count;
		GD.Print(enemyCount);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(entered) 
		{ 
			GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = true;
		}
	}

	public void OnEnemyDeath()
	{
		GD.Print("Enemy died.");
		enemyCount--;
		if(enemyCount <= 0)
		{
			RemoveFromGroup("block_manager");
			GetTree().CallGroup("wall_tile", WallTile.MethodName.OnBlockCleared);
			cleared = true;
		}
	}

	private void _on_area_2d_body_entered(Node2D body)
	{
		if (body.IsInGroup("player") && !cleared)
		{
			entered = true;
			GetTree().CallGroup("wall_tile", WallTile.MethodName.OnPlayerEnter);
			GetTree().CallGroup("spawner", Buoy.MethodName.OnPlayerEnter);
		}
	}

}
