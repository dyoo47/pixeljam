using Godot;
using pixeljam;
using System;

public partial class Camera : Camera2D
{
	private int roundX = 640;
	private int roundY = 368;
	private Vector2 prevPos;
	private Vector2 fromPos;
	private Vector2 diff;
	private double moveTime = 0.5;
	private double elapsedMoveTime = 0;
	private int viewWidth;
	private int viewHeight;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		viewWidth = (int)ProjectSettings.GetSetting("display/window/size/viewport_width");
		viewHeight = (int)ProjectSettings.GetSetting("display/window/size/viewport_height");
		prevPos = Position;
		GD.Print(viewWidth, viewHeight);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		elapsedMoveTime += delta;
		var pos = Game.MainPlayer.Position + new Vector2(0, -16.0f);
		Vector2 targetPos = new Vector2(pos.X - pos.X % roundX + viewWidth / 2, pos.Y - pos.Y % roundY + viewHeight / 2);
		if(prevPos != targetPos)
		{
			fromPos = prevPos;
			prevPos = targetPos;
			elapsedMoveTime = 0;
			diff = targetPos - fromPos;
		}
		float n = (float)(elapsedMoveTime / moveTime);
		Position = fromPos + diff * Util.easeOutExpo(n);
	}
}
