using Godot;

public partial class PanelStats : VBoxContainer {
	[Export] public bool UseSaveNodeData { get; set; } = true;
	[Export] public PlayerSkillData SkillXp { get; set; } = new();
	[Export] public bool SkillDescriptionIsClickable { get; set; } = false;

	private Label _labelStrengthValue;
	private Label _labelAgilityValue;
	private Label _labelArcanaValue;
	private Label _labelVitalityValue;

	public override void _Ready() {
		_labelStrengthValue = GetNodeOrNull<Label>("StatStr/LabelValue");
		_labelAgilityValue = GetNodeOrNull<Label>("StatAgi/LabelValue");
		_labelArcanaValue = GetNodeOrNull<Label>("StatArc/LabelValue");
		_labelVitalityValue = GetNodeOrNull<Label>("StatVit/LabelValue");
		ConfigureSkillRows();

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

	private void ConfigureSkillRows() {
		ConfigureSkillRow("StatStr", "Strength", "Strength improves direct physical power.");
		ConfigureSkillRow("StatAgi", "Agility", "Agility improves speed and dexterity.");
		ConfigureSkillRow("StatArc", "Arcana", "Arcana improves magical power and effects.");
		ConfigureSkillRow("StatVit", "Vitality", "Vitality improves survivability and endurance.");
	}

	private void ConfigureSkillRow(string path, string title, string description) {
		var row = GetNodeOrNull<Control>(path);
		if (row == null)
			return;

		row.MouseDefaultCursorShape = SkillDescriptionIsClickable
			? CursorShape.PointingHand
			: CursorShape.Arrow;
		row.TooltipText = SkillDescriptionIsClickable ? description : string.Empty;
		row.Modulate = SkillDescriptionIsClickable ? new Color(0.55f, 1.0f, 0.55f, 1.0f) : Colors.White;

		foreach (var child in row.GetChildren()) {
			if (child is Control controlChild)
				controlChild.MouseFilter = MouseFilterEnum.Ignore;
		}

		if (!SkillDescriptionIsClickable)
			return;

		row.GuiInput += inputEvent => {
			if (inputEvent is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
				return;

			GlobalOverlay.Get()?.ShowBlurredPopup(title, description);
		};
	}
}
