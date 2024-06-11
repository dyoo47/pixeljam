using Godot;
using pixeljam;
using System;
using System.Threading.Tasks;

public partial class Game : Node2D
{
	[Export] public Player PlayerScene;
	[Export] public Node2D HealthFlask;
	[Export] public Node2D WaterFlask;
	[Export] public Node2D XpFlask;
	[Export] public Godot.Collections.Array<PackedScene> BlockScenes;
	[Export] public Godot.Collections.Array<PackedScene> HardBlockScenes;
	[Export] public ConsumableIcon[] ConsumableIcons;
	[Export] public RichTextLabel LevelLabel;
	[Export] public RichTextLabel HealthLabel;
	[Export] public RichTextLabel WaterLabel;
	[Export] public PackedScene blocker;
	[Export] public PackedScene DeathScene;
	float displayHealth;
	float displayWater;
	public static Player MainPlayer;
	public static Game MainNode;
	public float healthFlaskHeight;
	public float healthFlaskMaxHeight = 100.0f;
	public float waterFlaskHeight;
	public float waterFlaskMaxHeight = 100.0f;
	public float xpFlaskHeight;
	public float xpFlaskMaxHeight = 120.0f;
	private int curBlockX;
	private int curBlockY;
	private int prevBlockX = 0;
	private int prevBlockY = 0;
	private static BlockManager[,] blocks;
	private PackedScene prevBlock;
	private Node AudioContainer;

	public static int ScreensCleared = 0;
	public static int MonstersSlain = 0;


	public override void _Ready()
	{
		healthFlaskHeight = healthFlaskMaxHeight;
		MainPlayer = PlayerScene;
		AudioContainer = MainPlayer.GetNode<Node>("AudioContainer");
		displayHealth = MainPlayer.Health;
		displayWater = MainPlayer.Water;
		MainNode = this;
		blocks = new BlockManager[17, 17];
		InstantiateBlock(ResourceLoader.Load<PackedScene>("res://blocks/block_start_1.tscn"), 9, 16);
		InstantiateBlock(ResourceLoader.Load<PackedScene>("res://blocks/block_start_2.tscn"), 9, 15);
		InstantiateBlock(ResourceLoader.Load<PackedScene>("res://blocks/block_start_3.tscn"), 9, 14);
		InstantiateBlock(ResourceLoader.Load<PackedScene>("res://blocks/block_start_4.tscn"), 9, 13);
		InstantiateBlock(ResourceLoader.Load<PackedScene>("res://blocks/block_start_5.tscn"), 9, 12);
		for(int i= 0; i < blocks.GetLength(0); i++)
		{
			for(int j=0; j < blocks.GetLength(1); j++)
			{
				if(i == 0 || i == blocks.GetLength(0) - 1 || j == 0 || j == blocks.GetLength(1) - 1)
				{
					if (i == 9 && j == 16) continue;
					Node2D blockerInstance = blocker.Instantiate<Node2D>();
					blockerInstance.Position = new Vector2(i * Constants.ROUND_X, j * Constants.ROUND_Y);
					MainNode.AddChild(blockerInstance);
				}
			}
		}
		MainPlayer.Position = blocks[9, 16].Position + new Vector2(Constants.ROUND_X / 2, Constants.ROUND_Y - 100) ;
	}

	public override void _Process(double delta)
	{
		LevelLabel.Text = "Lv." + MainPlayer.Level;
		for(int i=0; i < ConsumableIcons.Length; i++)
		{
			ConsumableIcons[i].count = MainPlayer.ConsumableCounts[i];
		}
		healthFlaskHeight = Util.lerp(healthFlaskHeight, MainPlayer.Health / (float) MainPlayer.MaxHealth * healthFlaskMaxHeight, (float)delta * 10);
		(HealthFlask.Material as ShaderMaterial).SetShaderParameter("px_height", healthFlaskHeight);
		displayHealth = Util.lerp(displayHealth, MainPlayer.Health, (float)delta * 10);
		if (Math.Abs(displayHealth - MainPlayer.Health) < 1) displayHealth = MainPlayer.Health;
		HealthLabel.Text = "[center]" + (int)displayHealth + "/" + MainPlayer.MaxHealth + "[/center]";

		waterFlaskHeight = Util.lerp(waterFlaskHeight, MainPlayer.Water / MainPlayer.MaxWater * waterFlaskMaxHeight, (float)delta * 10);
		(WaterFlask.Material as ShaderMaterial).SetShaderParameter("px_height", waterFlaskHeight);
		displayWater = Util.lerp(displayWater, MainPlayer.Water, (float)delta * 10);
		if (Math.Abs(displayWater - MainPlayer.Water) < 1) displayWater = MainPlayer.Water;
		WaterLabel.Text = "[center]" + (int)displayWater + "/" + MainPlayer.MaxWater + "[/center]";

		xpFlaskHeight = Util.lerp(xpFlaskHeight, MainPlayer.Xp / MainPlayer.MaxXp * xpFlaskMaxHeight, (float)delta * 10);
		(XpFlask.Material as ShaderMaterial).SetShaderParameter("px_height", xpFlaskHeight);

		Vector2 pos = MainPlayer.Position + new Vector2(0, -16.0f);
		curBlockX = (int)(pos.X - pos.X % Constants.ROUND_X) / Constants.ROUND_X;
		curBlockY = (int)(pos.Y - pos.Y % Constants.ROUND_Y) / Constants.ROUND_Y;
		bool changeScreen = false;
		if(curBlockX != prevBlockX)
		{
			changeScreen = true;
		}
		if(curBlockY != prevBlockY)
		{
			changeScreen = true;
		}
		if(changeScreen)
		{
			GD.Print(curBlockX, ", ", curBlockY);
			if (blocks[curBlockX, curBlockY] == null)
			{
				GD.Print("Generated new scene");
				PackedScene blockScene = BlockScenes.PickRandom();
				while (prevBlock != null && blockScene == prevBlock)
				{
					blockScene = BlockScenes.PickRandom();
				}
				prevBlock = blockScene;
					
				
				InstantiateBlock(blockScene, curBlockX, curBlockY);
			}
			// Delete previous scene
			BlockManager prev = blocks[prevBlockX, prevBlockY];
			if(prev != null)
			{
				Node2D blockerInstance = blocker.Instantiate<Node2D>();
				blockerInstance.Position = prev.Position;
				MainNode.AddChild(blockerInstance);
				AsyncDeleteBlock(prev);
			}
			prevBlockX = curBlockX;
			prevBlockY = curBlockY;
		}

		foreach(AudioStreamPlayer2D stream in AudioContainer.GetChildren())
		{
			if (!stream.Playing)
			{
				stream.QueueFree();
			}
		}
	}

	public static BlockManager GetBlock(Vector2 pos)
	{
		int blockIdxX = (int)(pos.X - pos.X % Constants.ROUND_X) / Constants.ROUND_X;
		int blockIdxY = (int)(pos.Y - pos.Y % Constants.ROUND_Y) / Constants.ROUND_Y;
		return blocks[blockIdxX, blockIdxY];
	}

	private void InstantiateBlock(PackedScene blockScene, int x, int y)
	{
		BlockManager blockManager = blockScene.Instantiate<BlockManager>();
		blocks[x, y] = blockManager;
		blockManager.Position = new Vector2(x * Constants.ROUND_X, y * Constants.ROUND_Y);
		MainNode.AddChild(blockManager);
	}

	public async void AsyncDeleteBlock(Node2D block)
	{
		await Task.Delay(TimeSpan.FromSeconds(1));
		block.QueueFree();
	}

	public void PlayLevelAnim()
	{
		AnimationPlayer player = LevelLabel.GetNode<AnimationPlayer>("AnimationPlayer");
		player.Stop();
		player.Play("wiggle");
	}

	public static Node2D GetMain()
	{
		return MainNode;
	}

	public static void PlaySound(string soundName)
	{
		GD.Print("playing sound");
		AudioStreamPlayer2D stream = new AudioStreamPlayer2D();
		stream.Stream = ResourceLoader.Load<AudioStreamWav>(soundName);
		MainNode.AudioContainer.AddChild(stream);
		GD.Print(MainNode.AudioContainer.GetChildren().Count);
		stream.Play();
		GD.Print(stream.Playing);
		GD.Print(stream.Bus);
	}

	public static void Death()
	{
		CanvasLayer deathInstance = MainNode.DeathScene.Instantiate<CanvasLayer>();
		MainNode.GetNode<Camera2D>("Camera2D").AddChild(deathInstance);
		PlaySound("res://sound/death.wav");
	}

	public static void AddHardLevels()
	{
		//foreach(PackedScene scene in MainNode.HardBlockScenes)
		//{
		//	MainNode.BlockScenes.Add(scene);
		//}
		MainNode.BlockScenes = MainNode.HardBlockScenes;
	}
}
