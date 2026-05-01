using Godot;

public partial class NewCharacterComponent : Control {
	private const string OutpostScenePath = "res://scenes/main/outpost/Outpost.tscn";
	private readonly PlaceholderTexture2D _placeholderIcon = new() { Size = new Vector2I(64, 64) };

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
		var labelName = GetNodeOrNull<Label>("TopUi/PlayerPreview/NameLabel");
		if (labelName != null)
			labelName.Text = string.IsNullOrWhiteSpace(PlayerData.PlayerName) ? "Player" : PlayerData.PlayerName;

		var startingItemLabel = GetNodeOrNull<Label>("TopUi/StartingItem/ItemNameLabel");
		if (startingItemLabel != null)
			startingItemLabel.Text = PlayerData.StartingItem?.ItemName ?? "None";

		var startingItemIcon = GetNodeOrNull<TextureRect>("TopUi/StartingItem/ItemIcon");
		if (startingItemIcon != null)
			startingItemIcon.Texture = PlayerData.StartingItem?.Icon ?? _placeholderIcon;

		var panelStats = GetNodeOrNull<PanelStats>("PanelStats");
		if (panelStats != null) {
			panelStats.UseSaveNodeData = false;
			panelStats.SkillXp = PlayerData.Skills;
			panelStats.Update();
		}
	}

	private void OnSelectPressed() {
		var saveNode = SaveNode.Get();
		saveNode.WipeRun();
		saveNode.RunData.PlayerData = PlayerData.Duplicate(true) as PlayerData ?? new PlayerData();
		saveNode.RunData.PlayerData.InventoryData = new InventoryData();

		if (PlayerData.StartingItem != null)
			saveNode.RunData.PlayerData.InventoryData.AddItem(PlayerData.StartingItem.Duplicate(true) as ItemBase);

		saveNode.RefreshStartCharacters();

		CallDeferred(MethodName.GoToOutpost);
	}

	private void GoToOutpost() {
		var outpostScene = ResourceLoader.Load<PackedScene>(OutpostScenePath);
		GlobalOverlay.Get()?.ChangeRootScene(outpostScene);
	}
}
