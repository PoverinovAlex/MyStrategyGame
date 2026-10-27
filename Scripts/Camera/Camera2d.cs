using Godot;
using System;

public partial class Camera2d : Camera2D
{
	[Export] private Grid _grid;
	
	[Export] public float PanSpeed = 1000f; // Скорость на WASD
	[Export] public float ZoomSpeed = 0.1f; // Скорость зума (СКМ)
	[Export] public float MinZoom = 0.9f;   // Максимальное отдаление
	[Export] public float MaxZoom = 2.5f;   // Максимальное приближение
	
	private Vector2 _dragStartMousePos;
	private Vector2 _dragStartCameraPos;
	private bool _isDragging = false;
	
	//borders
	private float _limitLeft;
	private float _limitTop;
	private float _limitRight;
	private float _limitBottom;

	public override void _Ready()
	{
		PositionSmoothingEnabled = false;
	}

	public void UpdateCameraLimits()
	{
		if (_grid == null) return;

		// Устанавливаем лимиты движения камеры (чтобы не уезжала за поле)
		_limitLeft = (int)_grid.GlobalTopLeft.X - 1.0f;
		_limitTop = (int)_grid.GlobalTopLeft.Y - 1.0f;
		_limitRight = (int)(_grid.GlobalTopLeft.X + _grid.FieldSizeInPixels.X + 1.0f);
		_limitBottom = (int)(_grid.GlobalTopLeft.Y + _grid.FieldSizeInPixels.Y + 1.0f);
		
		GD.Print($"Лимиты камеры обновлены: {_limitLeft}, {_limitTop}, {_limitRight}, {_limitBottom}");
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
			// Колесико вверх — приближаем (Zoom уменьшается)
			if (mouseButton.ButtonIndex == MouseButton.WheelUp && mouseButton.Pressed)
			{
				Zoom += new Vector2(ZoomSpeed, ZoomSpeed);
				ClampZoom();
				GD.Print(Zoom);
			}
			// Колесико вниз — отдаляем (Zoom увеличивается)
			else if (mouseButton.ButtonIndex == MouseButton.WheelDown && mouseButton.Pressed)
			{
				Zoom -= new Vector2(ZoomSpeed, ZoomSpeed);
				ClampZoom();
				GD.Print(Zoom);
			}
		}
	}

	// 1. Движение на WASD / Стрелки
	private void HandleKeyboardMovement(float delta)
	{
		Vector2 direction = Vector2.Zero;

		if (Input.IsActionPressed("ui_left"))  direction.X -= 1;
		if (Input.IsActionPressed("ui_right")) direction.X += 1;
		if (Input.IsActionPressed("ui_up"))    direction.Y -= 1;
		if (Input.IsActionPressed("ui_down"))  direction.Y += 1;

		if (direction != Vector2.Zero)
		{
			// Двигаем камеру. В Godot 4 Position камеры - это её центр, поэтому просто прибавляем вектор.
			// Умножаем на Zoom.X, чтобы скорость не зависела от масштаба.
			Position += direction.Normalized() * PanSpeed * delta * Zoom.X;
		}
	}
	
	private void HandleMiddleMouseDrag()
	{
		if (Input.IsMouseButtonPressed(MouseButton.Middle) || Input.IsMouseButtonPressed(MouseButton.Right))
		{
			if (!_isDragging)
			{
				// Запоминаем точку старта
				_isDragging = true;
				_dragStartMousePos = GetGlobalMousePosition();
				_dragStartCameraPos = Position;
			}
			else
			{
				// Вычисляем разницу и двигаем камеру в противоположную сторону
				Vector2 currentMousePos = GetGlobalMousePosition();
				Vector2 difference = _dragStartMousePos - currentMousePos;
				
				Position = _dragStartCameraPos + difference;
				
			Position = _dragStartCameraPos + difference;
			
			// Получаем текущую позицию в переменную
			Vector2 clampedPos = Position;

			// Ограничиваем по Y
			if (clampedPos.Y > _limitBottom)
				clampedPos.Y = _limitBottom;
			if (clampedPos.Y < _limitTop)
				clampedPos.Y = _limitTop;
				
			// Ограничиваем по X (исправлены опечатки _limitBottom -> _limitRight и < _limitRight -> < _limitLeft)
			if (clampedPos.X > _limitRight)
				clampedPos.X = _limitRight;
			if (clampedPos.X < _limitLeft)
				clampedPos.X = _limitLeft;

			// Присваиваем обратно
			Position = clampedPos;
			}
		}
		else
		{
			_isDragging = false;
		}
	}

	private void ClampZoom()
	{
		// Ограничиваем зум, чтобы не приблизиться/отдалиться слишком сильно
		Zoom = new Vector2(
			Mathf.Clamp(Zoom.X, MinZoom, MaxZoom),
			Mathf.Clamp(Zoom.Y, MinZoom, MaxZoom)
		);
	}
}
