using Godot;

public partial class PanelPlayerTop : Control {
	private Label _labelName;
	private Label _labelWave;

	public override void _Ready() {
		_labelName = GetNodeOrNull<Label>("LabelContainer/LabelName");
		_labelWave = GetNodeOrNull<Label>("LabelContainer/LabelWave");
		Update();
	}

	public void Update() {
		var runData = SaveNode.Get().RunData;
		var playerData = runData.PlayerData;

		if (_labelName != null)
			_labelName.Text = string.IsNullOrWhiteSpace(playerData?.PlayerName) ? "Player" : playerData.PlayerName;

		if (_labelWave != null)
			_labelWave.Text = $"Wave {runData.ContractsCompleted + 1}";
	}
}
