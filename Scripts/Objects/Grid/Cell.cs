using Godot;
using System;

public partial class Cell : Node2D
{
	[Export] public int X;
	[Export] public int Y;
	[Export] public bool Empty;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
