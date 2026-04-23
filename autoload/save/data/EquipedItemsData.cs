using Godot;

public partial class EquipedItemsData : Resource
{
	[Export] public ItemEquipable MainHandItem { get; set; }
	[Export] public ItemEquipable OffHandItem { get; set; }
	[Export] public ItemArmor ArmorItem { get; set; }
	[Export] public ItemAmulet AmuletItem { get; set; }
	[Export] public ItemAmmo AmmoItem { get; set; }
	[Export] public ItemConsumable ConsumableItem { get; set; }
	public EquipedItemsData() { }
}
