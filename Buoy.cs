using Godot;
using System;

public partial class Buoy : Node2D
{
	[Export] public PackedScene enemyScene;
	[Export] public double delay = 0.0f;
	private double _elapsedTime;
	private bool _started = false;
	private bool _ended = false;
	private AnimationPlayer _player;

	public override void _Ready()
	{
		_player = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Process(double delta)
	{
		if (_started && !_ended)
		{
			_elapsedTime += delta;
			if(_elapsedTime >= delay)
			{
				OnSummon();
				_ended = true;
			}
		}
	}

	private void _on_secondary_finished()
	{
		QueueFree();
	}

	public void OnPlayerEnter()
	{
		_started = true;
	}

	public void OnSummon()
	{
		_player.Play("spawn");
	}

	public void OnEnemySpawn()
	{
		Node2D instance = enemyScene.Instantiate<Node2D>();
		instance.Position = GlobalPosition;
		Game.MainNode.AddChild(instance);
	}
}




