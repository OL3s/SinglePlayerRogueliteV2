using Godot;

[GlobalClass]
public partial class PlayerData : Resource {
	[Export] public EquipedItemsData EquipedItems { get; set; } = new();

	public override string ToString() {
		return $"PlayerData:\n    EquipedItems={EquipedItems}";
	}
}
