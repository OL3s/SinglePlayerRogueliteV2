using Godot;
using System.Collections.Generic;
using System.Linq;

public static class StartingItems {
	public static readonly IReadOnlyDictionary<string, string> ItemPaths = new Dictionary<string, string> {
		{ "Iron Sword", "res://core/items/data/iron_sword.tres" },
		{ "Hunter Bow", "res://core/items/data/hunter_bow.tres" },
		{ "Leather Armor", "res://core/items/data/leather_armor.tres" },
		{ "Health Potion", "res://core/items/data/health_potion.tres" },
		{ "Arrow Bundle", "res://core/items/data/arrow_bundle.tres" },
		{ "Wolf Amulet", "res://core/items/data/wolf_amulet.tres" }
	};

	public static ItemBase GetRandomItem(RandomNumberGenerator random) {
		if (ItemPaths.Count == 0)
			return null;

		var itemPath = ItemPaths.Values.ElementAt(random.RandiRange(0, ItemPaths.Count - 1));
		var item = ResourceLoader.Load(itemPath) as ItemBase;
		if (item == null)
			GD.PushWarning($"Starting item could not be loaded: {itemPath}");

		return item?.Duplicate(true) as ItemBase;
	}
}
