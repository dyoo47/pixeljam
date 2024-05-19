using Godot;
using System;

public partial class Grassmap : TileMap
{
	[Export] public PackedScene GrassParticle;
	[Export] public int bladesPerTile = 8;
	private Random random;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		random = new Random();
		foreach (var tile in GetUsedCellsById(0, 0, new Vector2I(7, 2)))
		{
			for(int i=0; i < bladesPerTile; i++)
			{
				Sprite2D blade = GrassParticle.Instantiate<Sprite2D>();
				int rect = 16 * random.Next(0, 3);
				//blade.RegionRect.Position = new Vector2I(rect, 0);
				blade.RegionRect = new Rect2(rect, 0, 16, 16);
				blade.Position = tile * new Vector2I(16, 16) + new Vector2I(random.Next(16), random.Next(16));
				blade.Rotation = random.Next(-1, 1);
				AddChild(blade);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
