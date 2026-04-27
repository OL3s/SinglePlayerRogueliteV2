using Godot;

[GlobalClass]
public partial class PlayerData : Resource {
	[Export] public string PlayerName { get; set; } = "Player";
	[Export] public EquipedItemsData EquipedItems { get; set; } = new();
	[Export] public PlayerSkillData Skills { get; set; } = new();

	public override string ToString() {
		return $"PlayerData:\n    PlayerName={PlayerName}\n    EquipedItems={EquipedItems}\n    Skills={Skills}";
	}
}
