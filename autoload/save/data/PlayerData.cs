using Godot;

[GlobalClass]
public partial class PlayerData : Resource {
	[Export] public string PlayerName { get; set; } = "Player";
	[Export] public InventoryData InventoryData { get; set; } = new();
	[Export] public EquipedItemsData EquipedItems { get; set; } = new();
	[Export] public PlayerSkillData Skills { get; set; } = new();
	[Export] public int StartingTotalSkillXp { get; set; } = 0;
	[Export] public ItemBase StartingItem { get; set; }

	public int GetCurrentRunLevel() {
		var currentTotalXp = Skills?.GetTotalXp() ?? 0;
		var earnedXp = Mathf.Max(0, currentTotalXp - StartingTotalSkillXp);
		return PlayerSkillData.GetLevelFromXp(earnedXp);
	}

	public override string ToString() {
		return $"PlayerData:\n    PlayerName={PlayerName}\n    InventoryData={InventoryData}\n    EquipedItems={EquipedItems}\n    Skills={Skills}\n    StartingTotalSkillXp={StartingTotalSkillXp}\n    StartingItem={StartingItem}";
	}
}
