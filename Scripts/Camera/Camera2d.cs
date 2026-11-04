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

	public void UpdateDynamicLimits()
	{
		if (_grid == null) return;

		Vector2 screenSize = GetViewportRect().Size;
		float halfScreenWidth = screenSize.X / 2f / Zoom.X;
		float halfScreenHeight = screenSize.Y / 2f / Zoom.Y;

		LimitLeft = (int)(_grid.GlobalTopLeft.X - halfScreenWidth);
		LimitRight = (int)(_grid.GlobalTopLeft.X + _grid.FieldSizeInPixels.X + halfScreenWidth);
		LimitTop = (int)(_grid.GlobalTopLeft.Y - halfScreenHeight);
		LimitBottom = (int)(_grid.GlobalTopLeft.Y - _grid.FieldSizeInPixels.Y - halfScreenHeight);

		// Защита от инверсии лимитов
		if (LimitLeft > LimitRight)
		{
			LimitLeft = (int)(_grid.GlobalTopLeft.X + _grid.FieldSizeInPixels.X / 2f);
			LimitRight = LimitLeft;
		}
		if (LimitTop > LimitBottom)
		{
			LimitTop = (int)(_grid.GlobalTopLeft.Y + _grid.FieldSizeInPixels.Y / 2f);
			LimitBottom = LimitTop;
		}
		GD.Print($"Camera Limits: LimitLeft = {LimitLeft}, LimitRight = {LimitRight}, LimitTop = {LimitTop}, LimitBottom = {LimitBottom}");
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
