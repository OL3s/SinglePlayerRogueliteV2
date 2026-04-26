using Godot;

public partial class CameraTiltEffect : Camera2D
{
	[Export] public float TiltStrength { get; set; } = 20.0f;
	[Export] public float MaxTiltOffset { get; set; } = 50.0f;

	private Vector2 _originalPosition;
	private Vector3 _initialTilt;

	public override void _Ready()
	{
		_originalPosition = Position;
		_initialTilt = Input.GetGravity();
	}

	public override void _Process(double delta)
	{
		Vector3 tilt = Input.GetGravity() - _initialTilt;
		Vector2 tiltOffset = new Vector2(tilt.X, -tilt.Y) * TiltStrength;

		if (tiltOffset.Length() > MaxTiltOffset)
		{
			tiltOffset = tiltOffset.Normalized() * MaxTiltOffset;
		}

		Position = _originalPosition + tiltOffset;
	}
}
