using Godot;

namespace InputConfig
{
	public struct PlayerInput
	{
		public Vector2 LeftStick { get; set; }
		public Vector2 RightStick { get; set; }
		public bool Btn1 { get; set; }
		public bool Btn2 { get; set; }
		public bool Btn3 { get; set; }
		public bool Btn4 { get; set; }
		public bool Btn5 { get; set; }
		public bool Btn6 { get; set; }

		public override string ToString()
		{
			return $"LeftStick: ({LeftStick.X:F2}, {LeftStick.Y:F2})\nRightStick: ({RightStick.X:F2}, {RightStick.Y:F2})\nBtn1: {Btn1}\nBtn2: {Btn2}\nBtn3: {Btn3}\nBtn4: {Btn4}\nBtn5: {Btn5}\nBtn6: {Btn6}";
		}
	}
}
