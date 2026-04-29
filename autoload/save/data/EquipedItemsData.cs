using Godot;

public partial class EquipedItemsData : Resource {
	[Export] public ItemEquipable MainHandItem { get; set; }
	[Export] public ItemEquipable OffHandItem { get; set; }
	[Export] public ItemArmor ArmorItem { get; set; }
	[Export] public ItemAmulet AmuletItem { get; set; }
	[Export] public ItemAmmo AmmoItem { get; set; }
	[Export] public ItemConsumable ConsumableItem { get; set; }
	public EquipedItemsData() { }

	public bool TryEquipItem(ItemBase item, out ItemBase replacedItem) {
		replacedItem = null;

		switch (item) {
			case ItemArmor armor:
				replacedItem = ArmorItem;
				ArmorItem = armor;
				return true;
			case ItemAmulet amulet:
				replacedItem = AmuletItem;
				AmuletItem = amulet;
				return true;
			case ItemAmmo ammo:
				replacedItem = AmmoItem;
				AmmoItem = ammo;
				return true;
			case ItemConsumable consumable:
				replacedItem = ConsumableItem;
				ConsumableItem = consumable;
				return true;
			case ItemEquipable equipable:
				replacedItem = MainHandItem;
				MainHandItem = equipable;
				return true;
			default:
				return false;
		}
	}

	public bool TryEquipOffhandItem(ItemBase item, out ItemBase replacedItem) {
		replacedItem = null;

		if (item is not ItemEquipable equipable)
			return false;

		replacedItem = OffHandItem;
		OffHandItem = equipable;
		return true;
	}

	public bool TryUnequipItem(ItemBase item) {
		if (item == null)
			return false;

		if (MainHandItem == item) {
			MainHandItem = null;
			return true;
		}

		if (OffHandItem == item) {
			OffHandItem = null;
			return true;
		}

		if (ArmorItem == item) {
			ArmorItem = null;
			return true;
		}

		if (AmuletItem == item) {
			AmuletItem = null;
			return true;
		}

		if (AmmoItem == item) {
			AmmoItem = null;
			return true;
		}

		if (ConsumableItem == item) {
			ConsumableItem = null;
			return true;
		}

		return false;
	}

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
