using Godot;
using System;

public partial class Camera2d : Camera2D
{
	[Export] private Grid _grid;
	
	[Export] public float PanSpeed = 1000f;
	[Export] public float ZoomSpeed = 0.1f;
	[Export] public float MinZoom = 0.9f;
	[Export] public float MaxZoom = 2.5f;
	
	private Vector2 _dragStartMousePos;
	private Vector2 _dragStartCameraPos;
	private bool _isDragging = false;
	private Vector2 _lastMousePos;

	public override void _Ready()
	{
		PositionSmoothingEnabled = false;
	}

	public void UpdateCameraLimits()
	{
		if (_grid == null) return;

		// Используем встроенные лимиты Godot
		LimitLeft = (int)_grid.GlobalTopLeft.X - 600;
		LimitTop = (int)_grid.GlobalTopLeft.Y - 600;
		LimitRight = (int)(_grid.GlobalTopLeft.X + _grid.FieldSizeInPixels.X) - 600;
		LimitBottom = (int)(_grid.GlobalTopLeft.Y + _grid.FieldSizeInPixels.Y) - 600;
		
		GD.Print($"Лимиты камеры обновлены: {LimitLeft}, {LimitTop}, {LimitRight}, {LimitBottom}");
	}

	public override void _Process(double delta)
	{
		HandleKeyboardMovement((float)delta);
		HandleMiddleMouseDrag();
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.WheelUp && mouseButton.Pressed)
			{
				Zoom += new Vector2(ZoomSpeed, ZoomSpeed);
				ClampZoom();
			}
			else if (mouseButton.ButtonIndex == MouseButton.WheelDown && mouseButton.Pressed)
			{
				Zoom -= new Vector2(ZoomSpeed, ZoomSpeed);
				ClampZoom();
			}
		}
	}

	private void HandleKeyboardMovement(float delta)
	{
		Vector2 direction = Vector2.Zero;

		if (Input.IsActionPressed("ui_left"))  direction.X -= 1;
		if (Input.IsActionPressed("ui_right")) direction.X += 1;
		if (Input.IsActionPressed("ui_up"))    direction.Y -= 1;
		if (Input.IsActionPressed("ui_down"))  direction.Y += 1;

		if (direction != Vector2.Zero)
		{
			Position += direction.Normalized() * PanSpeed * delta * Zoom.X;
			
			// Округляем позицию, чтобы не было субпиксельной ряби
			Position = new Vector2(Mathf.Round(Position.X), Mathf.Round(Position.Y));
		}
	}
	
	//private void HandleMiddleMouseDrag()
	//{
		//if (Input.IsMouseButtonPressed(MouseButton.Middle) || Input.IsMouseButtonPressed(MouseButton.Right))
		//{
		//// Получаем текущую позицию мыши на экране (а не в мире!)
			//Vector2 currentMouseScreenPos = GetViewport().GetMousePosition();
//
			//if (!_isDragging)
			//{
				//_isDragging = true;
			//}
			//else
			//{
			//// Вычисляем, насколько мышь сдвинулась с прошлого кадра
				//Vector2 mouseDelta = currentMouseScreenPos - _lastMousePos;
			//
			//// Двигаем камеру в противоположную сторону (умножаем на зум, чтобы скорость совпадала)
				//Position -= mouseDelta;
			//
			//// Округляем ТОЛЬКО позицию камеры (без сдвига мыши)
				//Position = new Vector2(Mathf.Round(Position.X), Mathf.Round(Position.Y));
			//}
		//
			//_lastMousePos = currentMouseScreenPos;
		//}
		//else
		//{
			//_isDragging = false;
		//}
	//}
	
	private void HandleMiddleMouseDrag()
	{
		if (Input.IsMouseButtonPressed(MouseButton.Middle) || Input.IsMouseButtonPressed(MouseButton.Right))
		{
			
			if (!_isDragging)
			{
				_isDragging = true;
				_dragStartMousePos = GetViewport().GetMousePosition();
				_dragStartCameraPos = Position;
			}
			else
			{
				Vector2 currentMousePos = GetViewport().GetMousePosition();
				Vector2 difference = _dragStartMousePos - currentMousePos;
				
				Position = _dragStartCameraPos + difference;
			}
		}
		else
		{
			_isDragging = false;
		}
	}
	
	//private void HandleMiddleMouseDrag()
	//{
		//if (Input.IsMouseButtonPressed(MouseButton.Middle) || Input.IsMouseButtonPressed(MouseButton.Right))
		//{
			//if (!_isDragging)
			//{
				//_isDragging = true;
				//_dragStartMousePos = GetGlobalMousePosition();
				//_dragStartCameraPos = Position;
			//}
			//else
			//{
				//Vector2 currentMousePos = GetGlobalMousePosition();
				//Vector2 difference = _dragStartMousePos - currentMousePos;
				//
				//Position = _dragStartCameraPos + difference;
				//
				//// Округляем позицию, чтобы не было субпиксельной ряби
				//Position = new Vector2(Mathf.Round(Position.X), Mathf.Round(Position.Y));
			//}
		//}
		//else
		//{
			//_isDragging = false;
		//}
	//}

	private void ClampZoom()
	{
		Zoom = new Vector2(
			Mathf.Clamp(Zoom.X, MinZoom, MaxZoom),
			Mathf.Clamp(Zoom.Y, MinZoom, MaxZoom)
		);
	}
}
