using Godot;
using System;

public partial class Camera2d : Camera2D
{
	[Export] private Grid _grid;
	
	[Export] public float PanSpeed = 1000f;
	[Export] public float ZoomSpeed = 0.1f;
	[Export] public float MinZoom = 0.9f;
	[Export] public float MaxZoom = 2.5f;
	
	[Export] public float MarginOutside = 100f;
	
	private Vector2 _dragStartMousePos;
	private Vector2 _dragStartCameraPos;
	private bool _isDragging = false;
	
	private float _minX, _maxX, _minY, _maxY;

	public override void _Ready()
	{
		PositionSmoothingEnabled = false;
	}

	public void SetupCamera()
	{
		if (_grid == null) return;

		// Ставим камеру ровно в центр сетки (используем GlobalPosition)
		Vector2 gridCenter = _grid.GlobalTopLeft + (_grid.FieldSizeInPixels / 2f);
		GlobalPosition = gridCenter;
		
		// Задаем базовый зум
		Zoom = new Vector2(0.9f, 0.9f);
		
		UpdateDynamicLimits();
	}

	public override void _Process(double delta)
	{
		HandleKeyboardMovement((float)delta);
		HandleMiddleMouseDrag();

		// Принудительно удерживаем GlobalPosition центра камеры в рассчитанных рамках
		GlobalPosition = new Vector2(
			Mathf.Clamp(GlobalPosition.X, _minX - MarginOutside, _maxX + MarginOutside),
			Mathf.Clamp(GlobalPosition.Y,  _minY - MarginOutside, _maxY + MarginOutside)
		);
	}

	
	public void UpdateDynamicLimits()
	{
		if (_grid == null) return;

		// 1. Ваша логика вычисления половины видимой области
		Vector2 screenSize = GetViewportRect().Size;
		float halfScreenWidth = screenSize.X / 2f / Zoom.X;
		float halfScreenHeight = screenSize.Y / 2f / Zoom.Y;

		// 2. Границы карты
		float mapMinX = _grid.GlobalTopLeft.X;
		float mapMaxX = _grid.GlobalTopLeft.X + _grid.FieldSizeInPixels.X;
		float mapMinY = _grid.GlobalTopLeft.Y;
		float mapMaxY = _grid.GlobalTopLeft.Y + _grid.FieldSizeInPixels.Y;

		// 3. Вычисляем, где может находиться центр камеры
		_minX = mapMinX + halfScreenWidth;
		_maxX = mapMaxX - halfScreenWidth;
		_minY = mapMinY + halfScreenHeight;
		_maxY = mapMaxY - halfScreenHeight;

		// 4. Защита (если карта меньше экрана)
		if (_minX > _maxX) { _minX = _maxX = (mapMinX + mapMaxX) / 2f; }
		if (_minY > _maxY) { _minY = _maxY = (mapMinY + mapMaxY) / 2f; }
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
			// Используем GlobalPosition
			GlobalPosition += direction.Normalized() * PanSpeed * delta * Zoom.X;
		}
	}

	private void HandleMiddleMouseDrag()
	{
		if (Input.IsMouseButtonPressed(MouseButton.Middle) || Input.IsMouseButtonPressed(MouseButton.Right))
		{
			if (!_isDragging)
			{
				_isDragging = true;
				_dragStartMousePos = GetViewport().GetMousePosition();
				_dragStartCameraPos = GlobalPosition; // Запоминаем GlobalPosition
			}
			else
			{
				Vector2 currentMousePos = GetViewport().GetMousePosition();
				Vector2 difference = _dragStartMousePos - currentMousePos;
				
				GlobalPosition = _dragStartCameraPos + difference; // И двигаем GlobalPosition
			}
		}
		else
		{
			_isDragging = false;
		}
	}
	
	private void ClampZoom()
	{
		Zoom = new Vector2(
			Mathf.Clamp(Zoom.X, MinZoom, MaxZoom),
			Mathf.Clamp(Zoom.Y, MinZoom, MaxZoom)
		);
		
		UpdateDynamicLimits(); 
	}
}
