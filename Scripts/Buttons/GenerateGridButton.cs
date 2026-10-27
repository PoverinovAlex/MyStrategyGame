using Godot;
using System;

public partial class GenerateGridButton : Button
{
	private Grid _grid;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(_grid == null) _grid = GetNodeOrNull<Grid>("/root/Main/Grid");
		Pressed += () => {
		_grid.GenerateGrid();
		// Находим камеру на сцене и обновляем её лимиты
		var cam = GetNodeOrNull<Camera2d>("/root/Main/Camera2D");
		if (cam != null) cam.UpdateCameraLimits();
		else GD.Print("Not found!");
		Disabled = true;
		Visible = false;
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
