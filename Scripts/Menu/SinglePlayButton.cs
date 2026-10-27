using Godot;
using System;

public partial class SinglePlayButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Pressed += () => GoToPlayScene("res://Scenes/GameScene.tscn");
	}

	public void GoToPlayScene(string path)
	{
		// Переключение на сцену по пути
		Error result = GetTree().ChangeSceneToFile(path);
		
		if (result != Error.Ok)
		{
			GD.PrintErr("Не удалось загрузить сцену!");
		}
	}
	
}
