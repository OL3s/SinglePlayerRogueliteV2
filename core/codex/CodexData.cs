using System.Collections.Generic;
using System.Linq;
using Godot;

public static class CodexData {
	public static readonly IReadOnlyDictionary<CodexCategory, CodexSubcategory[]> Categories = new Dictionary<CodexCategory, CodexSubcategory[]> {
		{ CodexCategory.Items, new[] { CodexSubcategory.All, CodexSubcategory.Weapons, CodexSubcategory.Armor, CodexSubcategory.Consumables, CodexSubcategory.Ammo, CodexSubcategory.Amulets } },
		{ CodexCategory.Enemies, new[] { CodexSubcategory.All, CodexSubcategory.Slimes } },
		{ CodexCategory.Locations, new[] { CodexSubcategory.All } }
	};

	private static readonly string[] ItemPaths = {
		"res://core/items/data/iron_sword.tres",
		"res://core/items/data/hunter_bow.tres",
		"res://core/items/data/leather_armor.tres",
		"res://core/items/data/health_potion.tres",
		"res://core/items/data/arrow_bundle.tres",
		"res://core/items/data/wolf_amulet.tres"
	};

	private static readonly string[] EntryPaths = {
		"res://core/codex/entries/enemy_green_slime.tres",
		"res://core/codex/entries/enemy_blue_slime.tres",
		"res://core/codex/entries/enemy_red_slime.tres",
		"res://core/codex/entries/location_village.tres",
		"res://core/codex/entries/location_grasslands.tres"
	};

	public static readonly IReadOnlyList<CodexEntryData> Entries = ItemPaths
		.Select(path => ResourceLoader.Load<ItemBase>(path))
		.Where(item => item != null)
		.Select(CreateEntryFromItem)
		.Concat(EntryPaths.Select(path => ResourceLoader.Load<CodexEntryData>(path)))
		.Where(entry => entry != null)
		.ToArray();

	public static IReadOnlyList<CodexEntryData> GetEntries(CodexCategory category, CodexSubcategory subcategory = CodexSubcategory.All) {
		return Entries
			.Where(entry => entry.Category == category && (subcategory == CodexSubcategory.All || entry.Subcategory == subcategory))
			.ToArray();
	}

	private static CodexEntryData CreateEntryFromItem(ItemBase item) {
		return new CodexEntryData {
			Id = item.ItemID,
			Category = CodexCategory.Items,
			Subcategory = GetItemSubcategory(item),
			Title = item.ItemName,
			Description = GetItemDescription(item),
			Icon = item.Icon
		};
	}

	private static string GetItemDescription(ItemBase item) {
		return $"{item.ItemName}\nCost: {item.Cost}\nMax Stack: {item.MaxStackSize}";
	}

	private static CodexSubcategory GetItemSubcategory(ItemBase item) {
		return item switch {
			ItemEquipable => CodexSubcategory.Weapons,
			ItemArmor => CodexSubcategory.Armor,
			ItemConsumable => CodexSubcategory.Consumables,
			ItemAmmo => CodexSubcategory.Ammo,
			ItemAmulet => CodexSubcategory.Amulets,
			_ => CodexSubcategory.All
		};
	}
}
