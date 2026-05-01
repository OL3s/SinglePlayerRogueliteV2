public enum CodexCategory {
	Items = 0,
	Enemies = 1,
	Biomes = 2
}

/// <summary>
/// Codex subcategory values are serialized into codex .tres files as integers.
/// Keep explicit values stable. Name subcategories as Category_Display_Name: the first underscore separates
/// the backend category prefix from the display name, and later underscores become spaces in the UI.
/// </summary>
public enum CodexSubcategory {
	All = 0,
	Item_Weapons = 1,
	Item_Armor = 2,
	Item_Consumables = 3,
	Item_Ammo = 4,
	Item_Amulets = 5,
	Enemy_Slimes = 6,
	Biome_Greenlands = 7,
	Biome_Crossroads = 8,
	Biome_Wilds = 9,
	Biome_Boss_Realms = 10
}
