using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class CodexData {
	private const string ItemDirectory = "res://core/items/data";
	private const string EnemyEntryDirectory = "res://core/codex/entries/enemies";
	private const string BiomeEntryDirectory = "res://core/codex/entries/biomes";

	public static readonly IReadOnlyDictionary<CodexCategory, CodexSubcategory[]> Categories = new Dictionary<CodexCategory, CodexSubcategory[]> {
		{ CodexCategory.Items, new[] { CodexSubcategory.All, CodexSubcategory.Item_Weapons, CodexSubcategory.Item_Armor, CodexSubcategory.Item_Consumables, CodexSubcategory.Item_Ammo, CodexSubcategory.Item_Amulets } },
		{ CodexCategory.Enemies, new[] { CodexSubcategory.All, CodexSubcategory.Enemy_Slimes } },
		{ CodexCategory.Biomes, new[] { CodexSubcategory.All, CodexSubcategory.Biome_Greenlands, CodexSubcategory.Biome_Crossroads, CodexSubcategory.Biome_Wilds, CodexSubcategory.Biome_Boss_Realms } }
	};

	public static readonly IReadOnlyList<CodexEntryData> Entries = GetResourcePathsRecursive(ItemDirectory)
		.Select(path => ResourceLoader.Load<ItemBase>(path))
		.Where(item => item != null)
		.Select(CreateEntryFromItem)
		.Concat(GetEntryPaths().Select(path => ResourceLoader.Load<CodexEntryData>(path)))
		.Where(entry => entry != null)
		.OrderBy(entry => entry.Category)
		.ThenBy(entry => entry.Subcategory)
		.ThenBy(entry => entry.Title)
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
			ItemEquipable => CodexSubcategory.Item_Weapons,
			ItemArmor => CodexSubcategory.Item_Armor,
			ItemConsumable => CodexSubcategory.Item_Consumables,
			ItemAmmo => CodexSubcategory.Item_Ammo,
			ItemAmulet => CodexSubcategory.Item_Amulets,
			_ => CodexSubcategory.All
		};
	}

	private static IEnumerable<string> GetEntryPaths() {
		return GetResourcePathsRecursive(EnemyEntryDirectory)
			.Concat(GetResourcePathsRecursive(BiomeEntryDirectory));
	}

	private static IEnumerable<string> GetResourcePathsRecursive(string directory) {
		var dir = DirAccess.Open(directory);

		if (dir == null) {
			yield break;
		}

		foreach (var fileName in dir.GetFiles().Order(StringComparer.OrdinalIgnoreCase)) {
			if (IsResourceFile(fileName)) {
				yield return $"{directory}/{fileName}";
			}
		}

		foreach (var childDirectory in dir.GetDirectories().Order(StringComparer.OrdinalIgnoreCase)) {
			if (childDirectory.StartsWith('.')) {
				continue;
			}

			foreach (var resourcePath in GetResourcePathsRecursive($"{directory}/{childDirectory}")) {
				yield return resourcePath;
			}
		}
	}

	private static bool IsResourceFile(string fileName) {
		return fileName.EndsWith(".tres", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".res", StringComparison.OrdinalIgnoreCase);
	}
}
