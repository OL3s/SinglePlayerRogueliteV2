using Godot;

public partial class NewCharacterComponent : Control {
	private const string OutpostScenePath = "res://scenes/main/outpost/Outpost.tscn";

	[Export] public PlayerData PlayerData { get; set; } = new();

	public override void _Ready() {
		PlayerData.Skills ??= new PlayerSkillData();
		UpdatePanelStats();

		var selectButton = GetNodeOrNull<Button>("Button");
		if (selectButton != null)
			selectButton.Pressed += OnSelectPressed;
	}

	public void SetPlayerData(PlayerData playerData) {
		PlayerData = playerData ?? new PlayerData();
		PlayerData.Skills ??= new PlayerSkillData();
		UpdatePanelStats();
	}

	private void UpdatePanelStats() {
		var labelName = GetNodeOrNull<Label>("TopUi/Label");
		if (labelName != null)
			labelName.Text = string.IsNullOrWhiteSpace(PlayerData.PlayerName) ? "Player" : PlayerData.PlayerName;

		var panelStats = GetNodeOrNull<PanelStats>("PanelStats");
		if (panelStats != null) {
			panelStats.UseSaveNodeData = false;
			panelStats.SkillXp = PlayerData.Skills;
			panelStats.Update();
		}
	}

	private void OnSelectPressed() {
		var saveNode = SaveNode.Get();
		saveNode.RunData.PlayerData = PlayerData;
		saveNode.SaveRunData();
		saveNode.RefreshStartCharacters();

		CallDeferred(MethodName.GoToOutpost);
	}

	private void GoToOutpost() {
		var outpostScene = ResourceLoader.Load<PackedScene>(OutpostScenePath);
		GlobalOverlay.Get()?.ChangeRootScene(outpostScene);
	}
}
