using Godot;
using pixeljam;
using System;

public partial class Game : Node2D
{
	[Export] public Player PlayerScene;
	[Export] public Node2D HealthFlask;
	[Export] public Node2D WaterFlask;
	[Export] public Godot.Collections.Array<PackedScene> BlockScenes;
	public static Player MainPlayer;
	public static Node2D MainNode;
	public float healthFlaskHeight;
	public float healthFlaskMaxHeight = 100.0f;
	public float waterFlaskHeight;
	private int curBlockX;
	private int curBlockY;
	private int prevBlockX = 0;
	private int prevBlockY = 0;
	private BlockManager[,] blocks;

	public override void _Ready()
	{
		healthFlaskHeight = healthFlaskMaxHeight;
		MainPlayer = PlayerScene;
		MainNode = this;
		blocks = new BlockManager[16, 16];
	}

	public override void _Process(double delta)
	{
		healthFlaskHeight = Util.lerp(healthFlaskHeight, MainPlayer.Health / (float) MainPlayer.MaxHealth * healthFlaskMaxHeight, (float)delta * 10);
		(HealthFlask.Material as ShaderMaterial).SetShaderParameter("px_height", healthFlaskHeight);

		Vector2 pos = MainPlayer.Position;
		curBlockX = (int)(pos.X - pos.X % Constants.ROUND_X) / Constants.ROUND_X;
		curBlockY = (int)(pos.Y - pos.Y % Constants.ROUND_Y) / Constants.ROUND_Y;
		bool changeScreen = false;
		if(curBlockX != prevBlockX)
		{
			prevBlockX = curBlockX;
			changeScreen = true;
		}
		if(curBlockY != prevBlockY)
		{
			prevBlockY = curBlockY;
			changeScreen = true;
		}
		if(changeScreen)
		{
			GD.Print(curBlockX, ", ", curBlockY);
			if (blocks[curBlockX, curBlockY] == null)
			{
				GD.Print("Generated new scene");
				PackedScene blockScene = BlockScenes.PickRandom();
				BlockManager blockManager = blockScene.Instantiate<BlockManager>();
				blocks[curBlockX, curBlockY] = blockManager;
				blockManager.Position = new Vector2(curBlockX * Constants.ROUND_X, curBlockY * Constants.ROUND_Y);
				MainNode.AddChild(blockManager);
			}
		}
	}

	public static Node2D GetMain()
	{
		return MainNode;
	}
}
