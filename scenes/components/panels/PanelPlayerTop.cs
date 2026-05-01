using Godot;

public partial class PanelPlayerTop : Control {
	private Label _labelName;
	private Label _labelLevel;

	public override void _Ready() {
		_labelName = GetNodeOrNull<Label>("LabelContainer/LabelName");
		_labelLevel = GetNodeOrNull<Label>("LabelContainer/LabelLevel");
		Update();
	}

	public void Update() {
		var runData = SaveNode.Get()?.RunData;
		var playerData = runData?.PlayerData;

		if (_labelName != null)
			_labelName.Text = string.IsNullOrWhiteSpace(playerData?.PlayerName) ? "Player" : playerData.PlayerName;


		if (_labelLevel != null)
			_labelLevel.Text = $"Level {playerData?.GetCurrentRunLevel() ?? 0}";
	}
}
