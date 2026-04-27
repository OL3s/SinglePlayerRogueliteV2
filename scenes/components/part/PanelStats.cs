using Godot;

public partial class PanelStats : VBoxContainer {
	[Export] public bool UseSaveNodeData { get; set; } = true;

	public string StrengthValue { get; set; } = "0";
	public string AgilityValue { get; set; } = "0";
	public string ArcanaValue { get; set; } = "0";
	public string VitalityValue { get; set; } = "0";

	private Label _labelStrengthValue;
	private Label _labelAgilityValue;
	private Label _labelArcanaValue;
	private Label _labelVitalityValue;

	public override void _Ready() {
		_labelStrengthValue = GetNodeOrNull<Label>("StatStr/LabelValue");
		_labelAgilityValue = GetNodeOrNull<Label>("StatAgi/LabelValue");
		_labelArcanaValue = GetNodeOrNull<Label>("StatArc/LabelValue");
		_labelVitalityValue = GetNodeOrNull<Label>("StatVit/LabelValue");

		Update();
	}

	public void Update() {
		var skills = UseSaveNodeData ? SaveNode.Get()?.RunData?.PlayerData?.Skills : null;

		SetLabelText(_labelStrengthValue, skills?.GetStrengthLevel().ToString() ?? StrengthValue);
		SetLabelText(_labelAgilityValue, skills?.GetAgilityLevel().ToString() ?? AgilityValue);
		SetLabelText(_labelArcanaValue, skills?.GetArcanaLevel().ToString() ?? ArcanaValue);
		SetLabelText(_labelVitalityValue, skills?.GetVitalityLevel().ToString() ?? VitalityValue);
	}

	private static void SetLabelText(Label label, string text) {
		if (label != null)
			label.Text = text;
	}
}
