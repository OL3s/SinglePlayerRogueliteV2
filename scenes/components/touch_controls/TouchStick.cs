using Godot;
using System;

public partial class TouchStick : Panel {
	[Export] public Panel DrawStick { get; set; }
	[Export] public TouchControls InputRoot { get; set; }
	[Export] public float Deadzone { get; set; } = 0.5f;
	[Export] private int TapPressedFrames { get; set; }
	[Export] public int TapMaxHoldFrames { get; set; } = 12;
	public Vector2 StickVector { get; private set; }
	public bool ButtonPressed =>
		_buttonPressedUntilFrame != ulong.MaxValue &&
		Engine.GetProcessFrames() <= _buttonPressedUntilFrame;

	private Vector2 _stickCenter;
	private int _activeFingerIndex = -1;
	private bool _tapCandidate;
	private ulong _touchStartedFrame;
	private ulong _buttonPressedUntilFrame = ulong.MaxValue;

	public override void _Ready() {
		if (InputRoot == null) throw new ArgumentNullException(nameof(InputRoot), "InputRoot is not assigned in the inspector.");
		if (DrawStick == null) throw new ArgumentNullException(nameof(DrawStick), "DrawStick is not assigned in the inspector.");

		TapPressedFrames = InputRoot.ButtonPressedFrames;
		UpdateStickCenter();
		ResetStick();
	}

	public override void _Input(InputEvent @event) {
		base._Input(@event);

		switch (@event) {
			case InputEventScreenTouch touchEvent:
				HandleScreenTouch(touchEvent);
				break;
			case InputEventScreenDrag dragEvent:
				HandleScreenDrag(dragEvent);
				break;
		}
	}

	private void HandleScreenTouch(InputEventScreenTouch touchEvent) {
		if (touchEvent.Pressed) {
			if (_activeFingerIndex != -1 || !GetGlobalRect().HasPoint(touchEvent.Position)) return;

			_activeFingerIndex = touchEvent.Index;
			_touchStartedFrame = Engine.GetProcessFrames();
			_tapCandidate = IsWithinDeadzone(touchEvent.Position);
			UpdateStickFromPosition(touchEvent.Position);
			GetViewport().SetInputAsHandled();
			return;
		}

		if (touchEvent.Index != _activeFingerIndex) return;

		UpdateTapCandidate(touchEvent.Position);
		if (ShouldRegisterTap()) RegisterTapPress();

		ResetStick();
		GetViewport().SetInputAsHandled();
	}

	private void HandleScreenDrag(InputEventScreenDrag dragEvent) {
		if (dragEvent.Index != _activeFingerIndex) return;

		UpdateTapCandidate(dragEvent.Position);
		UpdateStickFromPosition(dragEvent.Position);
		GetViewport().SetInputAsHandled();
	}

	private void UpdateStickFromPosition(Vector2 globalPosition) {
		UpdateStickCenter();

		Vector2 localPosition = GetGlobalTransformWithCanvas().AffineInverse() * globalPosition;
		Vector2 offset = localPosition - _stickCenter;
		float maxDistance = GetMaxDistance();
		Vector2 clampedOffset = maxDistance > 0.0f ? offset.LimitLength(maxDistance) : Vector2.Zero;
		float clampedLength = clampedOffset.Length();

		StickVector = GetDeadzonedVector(clampedOffset, clampedLength, maxDistance);
		PositionDrawStick(clampedOffset);
	}

	private void UpdateTapCandidate(Vector2 globalPosition) {
		if (!_tapCandidate) return;

		if (!IsWithinDeadzone(globalPosition)) _tapCandidate = false;
	}

	private bool ShouldRegisterTap() {
		if (!_tapCandidate) return false;

		ulong heldFrames = Engine.GetProcessFrames() - _touchStartedFrame;
		return heldFrames <= (ulong)Mathf.Max(0, TapMaxHoldFrames);
	}

	private void RegisterTapPress() {
		int pressedFrames = Mathf.Max(1, TapPressedFrames);
		_buttonPressedUntilFrame = Engine.GetProcessFrames() + (ulong)(pressedFrames - 1);
	}

	private Vector2 GetDeadzonedVector(Vector2 clampedOffset, float clampedLength, float maxDistance) {
		if (maxDistance <= 0.0f || clampedLength <= 0.0f) return Vector2.Zero;

		float normalizedMagnitude = clampedLength / maxDistance;
		float clampedDeadzone = Mathf.Clamp(Deadzone, 0.0f, 1.0f);

		if (normalizedMagnitude <= clampedDeadzone) return Vector2.Zero;

		float adjustedMagnitude = (normalizedMagnitude - clampedDeadzone) / (1.0f - clampedDeadzone);
		return clampedOffset.Normalized() * adjustedMagnitude;
	}

	private float GetMaxDistance() {
		return Mathf.Max(0.0f, Mathf.Min(Size.X - DrawStick.Size.X, Size.Y - DrawStick.Size.Y) * 0.5f);
	}

	private bool IsWithinDeadzone(Vector2 globalPosition) {
		UpdateStickCenter();

		float maxDistance = GetMaxDistance();
		if (maxDistance <= 0.0f) return false;

		Vector2 localPosition = GetGlobalTransformWithCanvas().AffineInverse() * globalPosition;
		float normalizedMagnitude = (localPosition - _stickCenter).Length() / maxDistance;
		float clampedDeadzone = Mathf.Clamp(Deadzone, 0.0f, 1.0f);
		return normalizedMagnitude <= clampedDeadzone;
	}

	private void PositionDrawStick(Vector2 offset) {
		DrawStick.Position = _stickCenter - (DrawStick.Size * 0.5f) + offset;
	}

	private void ResetStick() {
		_activeFingerIndex = -1;
		_tapCandidate = false;
		_touchStartedFrame = 0;
		StickVector = Vector2.Zero;
		UpdateStickCenter();
		PositionDrawStick(Vector2.Zero);
	}

	private void UpdateStickCenter() {
		_stickCenter = Size * 0.5f;
	}
}
