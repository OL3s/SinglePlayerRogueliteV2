using Godot;

public partial class PanelCareerData : Control {
	private Label _labelName;

	public override void _Ready() {
		_labelName = GetNodeOrNull<Label>("HBoxContainer/Name/Label");
		Update();
	}

	public void Update() {
		var playerName = SaveNode.Get()?.RunData?.PlayerData?.PlayerName;

		if (_labelName != null)
			_labelName.Text = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName;
	}
}
