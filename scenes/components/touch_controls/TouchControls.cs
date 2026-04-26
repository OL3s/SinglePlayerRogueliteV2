using Godot;
using InputConfig;
using System;

public partial class TouchControls : Control {
	[Export] public Button Btn1 { get; set; }
	[Export] public Button Btn2 { get; set; }
	[Export] public Button Btn3 { get; set; }
	[Export] public Button Btn4 { get; set; }
	[Export] public int ButtonPressedFrames { get; set; } = 5;
	[Export] public Label DebugLabel { get; set; }
	[Export] public TouchStick LeftStick { get; set; }
	[Export] public TouchStick RightStick { get; set; }
	[Export] public bool EnableDebugOutput { get; set; } = false;

	public TouchInput TouchInput = new TouchInput();
	private ulong _btn1PressedUntilFrame = ulong.MaxValue;
	private ulong _btn2PressedUntilFrame = ulong.MaxValue;
	private ulong _btn3PressedUntilFrame = ulong.MaxValue;
	private ulong _btn4PressedUntilFrame = ulong.MaxValue;
	private int _btn1FingerIndex = -1;
	private int _btn2FingerIndex = -1;
	private int _btn3FingerIndex = -1;
	private int _btn4FingerIndex = -1;

	public override void _Ready() {
		if (Btn1 == null) throw new ArgumentNullException("Btn1 is not assigned in the inspector.");
		if (Btn2 == null) throw new ArgumentNullException("Btn2 is not assigned in the inspector.");
		if (Btn3 == null) throw new ArgumentNullException("Btn3 is not assigned in the inspector.");
		if (Btn4 == null) throw new ArgumentNullException("Btn4 is not assigned in the inspector.");
		if (LeftStick == null) throw new ArgumentNullException("LeftStick is not assigned in the inspector.");
		if (RightStick == null) throw new ArgumentNullException("RightStick is not assigned in the inspector.");

		Btn1.ButtonDown += () => OnButtonDown(ref _btn1PressedUntilFrame);
		Btn2.ButtonDown += () => OnButtonDown(ref _btn2PressedUntilFrame);
		Btn3.ButtonDown += () => OnButtonDown(ref _btn3PressedUntilFrame);
		Btn4.ButtonDown += () => OnButtonDown(ref _btn4PressedUntilFrame);

		if (DebugLabel != null) DebugLabel.Visible = EnableDebugOutput;
	}

	public override void _Input(InputEvent @event) {
		base._Input(@event);

		bool handled = @event switch {
			InputEventScreenTouch touchEvent =>
				HandleButtonTouch(Btn1, touchEvent, ref _btn1FingerIndex, ref _btn1PressedUntilFrame) |
				HandleButtonTouch(Btn2, touchEvent, ref _btn2FingerIndex, ref _btn2PressedUntilFrame) |
				HandleButtonTouch(Btn3, touchEvent, ref _btn3FingerIndex, ref _btn3PressedUntilFrame) |
				HandleButtonTouch(Btn4, touchEvent, ref _btn4FingerIndex, ref _btn4PressedUntilFrame),
			_ => false,
		};

		if (handled) GetViewport().SetInputAsHandled();
	}

	public override void _Process(double delta) {
		TouchInput.Btn1 = IsBufferedPressActive(_btn1PressedUntilFrame);
		TouchInput.Btn2 = IsBufferedPressActive(_btn2PressedUntilFrame);
		TouchInput.Btn3 = IsBufferedPressActive(_btn3PressedUntilFrame);
		TouchInput.Btn4 = IsBufferedPressActive(_btn4PressedUntilFrame);
		TouchInput.Btn5 = LeftStick.ButtonPressed;
		TouchInput.Btn6 = RightStick.ButtonPressed;
		TouchInput.LeftStick = LeftStick.StickVector;
		TouchInput.RightStick = RightStick.StickVector;

		if (DebugLabel != null) DebugLabel.Text = TouchInput.ToString();
	}

	public TouchInput GetTouchInput() {
		return TouchInput;
	}

	private void OnButtonDown(ref ulong pressedUntilFrame) {
		int pressedFrames = Mathf.Max(1, ButtonPressedFrames);
		pressedUntilFrame = Engine.GetProcessFrames() + (ulong)(pressedFrames - 1);
	}

	private bool HandleButtonTouch(Button button, InputEventScreenTouch touchEvent, ref int activeFingerIndex, ref ulong pressedUntilFrame) {
		if (touchEvent.Pressed) {
			if (activeFingerIndex != -1 || !button.GetGlobalRect().HasPoint(touchEvent.Position)) return false;

			activeFingerIndex = touchEvent.Index;
			button.SetPressedNoSignal(true);
			OnButtonDown(ref pressedUntilFrame);
			return true;
		}

		if (touchEvent.Index != activeFingerIndex) return false;

		activeFingerIndex = -1;
		button.SetPressedNoSignal(false);
		return true;
	}

	private static bool IsBufferedPressActive(ulong pressedUntilFrame) {
		return pressedUntilFrame != ulong.MaxValue && Engine.GetProcessFrames() <= pressedUntilFrame;
	}
}
