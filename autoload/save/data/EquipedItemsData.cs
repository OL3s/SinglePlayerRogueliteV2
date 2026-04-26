using Godot;

public partial class EquipedItemsData : Resource {
	[Export] public ItemEquipable MainHandItem { get; set; }
	[Export] public ItemEquipable OffHandItem { get; set; }
	[Export] public ItemArmor ArmorItem { get; set; }
	[Export] public ItemAmulet AmuletItem { get; set; }
	[Export] public ItemAmmo AmmoItem { get; set; }
	[Export] public ItemConsumable ConsumableItem { get; set; }
	public EquipedItemsData() { }

	public override string ToString() {
		return $"EquipedItemsData:\n"
			+ $"      MainHand={FormatItem(MainHandItem)}\n"
			+ $"      OffHand={FormatItem(OffHandItem)}\n"
			+ $"      Armor={FormatItem(ArmorItem)}\n"
			+ $"      Amulet={FormatItem(AmuletItem)}\n"
			+ $"      Ammo={FormatItem(AmmoItem)}\n"
			+ $"      Consumable={FormatItem(ConsumableItem)}";
	}

	private static string FormatItem(ItemBase item) {
		return item?.ToString() ?? "None";
	}
}
