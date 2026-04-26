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
