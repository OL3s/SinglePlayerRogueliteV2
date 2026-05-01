using Godot;

[GlobalClass]
public partial class PlayerData : Resource {
	[Export] public string PlayerName { get; set; } = "Player";
	[Export] public InventoryData InventoryData { get; set; } = new();
	[Export] public EquipedItemsData EquipedItems { get; set; } = new();
	[Export] public PlayerSkillData Skills { get; set; } = new();
	[Export] public ItemBase StartingItem { get; set; }

	public override string ToString() {
		return $"PlayerData:\n    PlayerName={PlayerName}\n    InventoryData={InventoryData}\n    EquipedItems={EquipedItems}\n    Skills={Skills}\n    StartingItem={StartingItem}";
	}
}
