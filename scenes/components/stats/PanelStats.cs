using Godot;

public partial class PanelStats : VBoxContainer {
	[Export] public bool UseSaveNodeData { get; set; } = true;
	[Export] public PlayerSkillData SkillXp { get; set; } = new();

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
		var skills = UseSaveNodeData ? SaveNode.Get()?.RunData?.PlayerData?.Skills ?? SkillXp : SkillXp;

		SetLabelText(_labelStrengthValue, skills.GetStrengthLevel().ToString());
		SetLabelText(_labelAgilityValue, skills.GetAgilityLevel().ToString());
		SetLabelText(_labelArcanaValue, skills.GetArcanaLevel().ToString());
		SetLabelText(_labelVitalityValue, skills.GetVitalityLevel().ToString());
	}

	private static void SetLabelText(Label label, string text) {
		if (label != null)
			label.Text = text;
	}
}
