using Godot;

public partial class PanelStats : VBoxContainer {
	[Export] public bool UseSaveNodeData { get; set; } = true;
	[Export] public PlayerSkillData SkillXp { get; set; } = new();

	private bool _skillDescriptionIsClickable;

	[Export]
	public bool SkillDescriptionIsClickable {
		get => _skillDescriptionIsClickable;
		set {
			_skillDescriptionIsClickable = value;
			UpdateSkillRowStates();
		}
	}

	private Label _labelStrengthValue;
	private Label _labelAgilityValue;
	private Label _labelArcanaValue;
	private Label _labelVitalityValue;

	public override void _Ready() {
		_labelStrengthValue = GetNodeOrNull<Label>("StatStr/LabelValue");
		_labelAgilityValue = GetNodeOrNull<Label>("StatAgi/LabelValue");
		_labelArcanaValue = GetNodeOrNull<Label>("StatArc/LabelValue");
		_labelVitalityValue = GetNodeOrNull<Label>("StatVit/LabelValue");
		ConnectSkillRows();
		UpdateSkillRowStates();

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

	private void ConnectSkillRows() {
		ConnectSkillRow("StatStr", "Strength", "Strength improves direct physical power.");
		ConnectSkillRow("StatAgi", "Agility", "Agility improves speed and dexterity.");
		ConnectSkillRow("StatArc", "Arcana", "Arcana improves magical power and effects.");
		ConnectSkillRow("StatVit", "Vitality", "Vitality improves survivability and endurance.");
	}

	private void UpdateSkillRowStates() {
		ApplySkillRowState("StatStr", "Strength improves direct physical power.");
		ApplySkillRowState("StatAgi", "Agility improves speed and dexterity.");
		ApplySkillRowState("StatArc", "Arcana improves magical power and effects.");
		ApplySkillRowState("StatVit", "Vitality improves survivability and endurance.");
	}

	private void ConnectSkillRow(string path, string title, string description) {
		var row = GetNodeOrNull<Control>(path);
		if (row == null)
			return;

		foreach (var child in row.GetChildren()) {
			if (child is Control controlChild)
				controlChild.MouseFilter = MouseFilterEnum.Ignore;
		}

		row.GuiInput += inputEvent => {
			if (!SkillDescriptionIsClickable)
				return;

			if (inputEvent is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
				return;

			GlobalOverlay.Get()?.ShowBlurredPopup(title, description);
		};
	}

	private void ApplySkillRowState(string path, string description) {
		var row = GetNodeOrNull<Control>(path);
		if (row == null)
			return;

		row.MouseDefaultCursorShape = SkillDescriptionIsClickable
			? CursorShape.PointingHand
			: CursorShape.Arrow;
		row.Modulate = SkillDescriptionIsClickable ? new Color(0.55f, 1.0f, 0.55f, 1.0f) : Colors.White;
	}
}
