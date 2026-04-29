using Godot;
using Godot.Collections;

[GlobalClass]
public partial class InventoryData : Resource {
	[Export] public Array<ItemBase> Items { get; set; } = new Array<ItemBase>();

	public void AddItem(ItemBase item) {
		Items.Add(item);
	}

	public void RemoveItem(ItemBase item) {
		Items.Remove(item);
	}

	public bool TryEquipItem(ItemBase item, EquipedItemsData equipedItems) {
		return TryEquipItem(item, equipedItems, false);
	}

	public bool TryEquipOffhandItem(ItemBase item, EquipedItemsData equipedItems) {
		return TryEquipItem(item, equipedItems, true);
	}

	public bool TryUnequipItem(ItemBase item, EquipedItemsData equipedItems) {
		if (item == null || equipedItems == null)
			return false;

		if (!equipedItems.TryUnequipItem(item))
			return false;

		AddItem(item);
		return true;
	}

	private bool TryEquipItem(ItemBase item, EquipedItemsData equipedItems, bool offhand) {
		if (item == null || equipedItems == null || !Items.Contains(item))
			return false;

		var equipped = offhand
			? equipedItems.TryEquipOffhandItem(item, out var replacedItem)
			: equipedItems.TryEquipItem(item, out replacedItem);

		if (!equipped)
			return false;

		RemoveItem(item);
		if (replacedItem != null)
			AddItem(replacedItem);

		return true;
	}

	public ItemBase GetItemByID(string itemID) {
		foreach (var item in Items) {
			if (item.ItemID == itemID)
				return item;
		}

		return null;
	}

	public override string ToString() {
		return $"InventoryData:\n    Items={FormatItems(Items)}";
	}

	private static string FormatItems(Array<ItemBase> items) {
		if (items == null || items.Count == 0)
			return "[]";

		var result = "[\n";
		for (var i = 0; i < items.Count; i++) {
			if (i > 0)
				result += ",\n";

			result += $"      {items[i]?.ToString() ?? "null"}";
		}

		return result + "\n    ]";
	}
}
