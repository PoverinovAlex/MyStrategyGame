using Godot;
using System;

public partial class Grid : Node2D
{
	[Export] private PackedScene _cell;
	[Export] private int _width;
	[Export] private int _height;
	
	public Vector2 FieldSizeInPixels { get; private set; }
	public Vector2 GlobalTopLeft { get; private set; }
	
	private Cell[,] _cells;
	
	
	public void GenerateGrid()
	{
		if(_cell == null) return;
		
		_cells = new Cell[_width, _height];
		float cellWidth = 0;
		float cellHeight = 0;
		
		for(int w=0; w<_width; w++)
		{
			for(int h=0; h<_height;h++)
			{
				Node instance = _cell.Instantiate();
				Cell cellInstance = instance as Cell;
				if (cellInstance != null)
				{
					ColorRect myColorRect = cellInstance.GetNodeOrNull<ColorRect>("Border");
					Vector2 actualSize = myColorRect.Size * cellInstance.Scale;
					
					cellWidth = actualSize.X;
					cellHeight = actualSize.Y;
					
					cellInstance.Position = new Vector2(w * cellWidth, h*cellHeight);
					AddChild(cellInstance);
					cellInstance.X = w;
					cellInstance.Y = h;
					cellInstance.Empty = false;
					_cells[w, h] = cellInstance;
					
				}				
			}
		}
		
		FieldSizeInPixels = new Vector2(_width * cellWidth, _height * cellHeight);
		GlobalTopLeft = GlobalPosition;
		
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
