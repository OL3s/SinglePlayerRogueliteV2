using Godot;
using InputConfig;

public partial class Player : CharacterBody2D {
	public const float Speed = 300.0f;
	private TouchControls TouchControls { get; set; }

	private TouchInput _previousTouchInput;

	public override void _Ready() {
		TouchControls = GetNodeOrNull<TouchControls>("CanvasLayer/TouchControls");
	}

	public override void _PhysicsProcess(double delta) {
		TouchInput touchInput = TouchControls?.GetTouchInput() ?? new TouchInput();
		Vector2 direction = touchInput.LeftStick;

		if (direction == Vector2.Zero) direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Velocity = direction * Speed;
		ShowButtonDebug(touchInput);

		MoveAndSlide();
	}

	private void ShowButtonDebug(TouchInput touchInput) {
		if (touchInput.Btn1 && !_previousTouchInput.Btn1) GD.Print("Btn1 pressed");
		if (touchInput.Btn2 && !_previousTouchInput.Btn2) GD.Print("Btn2 pressed");
		if (touchInput.Btn3 && !_previousTouchInput.Btn3) GD.Print("Btn3 pressed");
		if (touchInput.Btn4 && !_previousTouchInput.Btn4) GD.Print("Btn4 pressed");
		if (touchInput.Btn5 && !_previousTouchInput.Btn5) GD.Print("Btn5 pressed");
		if (touchInput.Btn6 && !_previousTouchInput.Btn6) GD.Print("Btn6 pressed");

		_previousTouchInput = touchInput;
	}
}
