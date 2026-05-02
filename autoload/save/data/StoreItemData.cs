using Combat;
using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class StoreItemData : Resource {
	[Export] public Array<ItemBase> TavernItems { get; set; }
	[Export] public Array<ItemBase> MerchantItems { get; set; }
	[Export] public Array<ItemBase> BlacksmithItems { get; set; }
	[Export] public Array<ItemBase> GoldsmithItems { get; set; }
	[Export] public Array<ItemBase> AlchemistItems { get; set; }
	[Export] public Array<ItemBase> FletcherItems { get; set; }
	[Export] public Array<ItemBase> ArcanistItems { get; set; }
	[Export] public Array<ItemBase> EnchanterItems { get; set; }

	public StoreItemData() {
	}

	public void GenerateMissingItems(MyTypes.Biomes biome) {
		TavernItems ??= GenerateRandomStoreItemsForBuilding(MyTypes.BuildingTypes.Tavern, biome);
		MerchantItems ??= GenerateRandomStoreItemsForBuilding(MyTypes.BuildingTypes.Merchant, biome);
		BlacksmithItems ??= GenerateRandomStoreItemsForBuilding(MyTypes.BuildingTypes.Blacksmith, biome);
		GoldsmithItems ??= GenerateRandomStoreItemsForBuilding(MyTypes.BuildingTypes.Goldsmith, biome);
		AlchemistItems ??= GenerateRandomStoreItemsForBuilding(MyTypes.BuildingTypes.Alchemist, biome);
		FletcherItems ??= GenerateRandomStoreItemsForBuilding(MyTypes.BuildingTypes.Fletcher, biome);
		ArcanistItems ??= GenerateRandomStoreItemsForBuilding(MyTypes.BuildingTypes.Arcanist, biome);
		EnchanterItems ??= GenerateRandomStoreItemsForBuilding(MyTypes.BuildingTypes.Enchanter, biome);
	}

	public void RegenerateItemsForBuilding(MyTypes.BuildingTypes buildingType, MyTypes.Biomes biome) {
		switch (buildingType) {
			case MyTypes.BuildingTypes.Tavern:
				TavernItems = GenerateRandomStoreItemsForBuilding(buildingType, biome);
				break;
			case MyTypes.BuildingTypes.Merchant:
				MerchantItems = GenerateRandomStoreItemsForBuilding(buildingType, biome);
				break;
			case MyTypes.BuildingTypes.Blacksmith:
				BlacksmithItems = GenerateRandomStoreItemsForBuilding(buildingType, biome);
				break;
			case MyTypes.BuildingTypes.Goldsmith:
				GoldsmithItems = GenerateRandomStoreItemsForBuilding(buildingType, biome);
				break;
			case MyTypes.BuildingTypes.Alchemist:
				AlchemistItems = GenerateRandomStoreItemsForBuilding(buildingType, biome);
				break;
			case MyTypes.BuildingTypes.Fletcher:
				FletcherItems = GenerateRandomStoreItemsForBuilding(buildingType, biome);
				break;
			case MyTypes.BuildingTypes.Arcanist:
				ArcanistItems = GenerateRandomStoreItemsForBuilding(buildingType, biome);
				break;
			case MyTypes.BuildingTypes.Enchanter:
				EnchanterItems = GenerateRandomStoreItemsForBuilding(buildingType, biome);
				break;
		}
	}

	public void RegenerateItemsForAllBuildings(MyTypes.Biomes biome) {
		foreach (MyTypes.BuildingTypes buildingType in Enum.GetValues(typeof(MyTypes.BuildingTypes))) {
			RegenerateItemsForBuilding(buildingType, biome);
		}
	}

	public Array<ItemBase> GetItemsForBuildingType(MyTypes.BuildingTypes buildingType) {
		return buildingType switch {
			MyTypes.BuildingTypes.Tavern => TavernItems,
			MyTypes.BuildingTypes.Merchant => MerchantItems,
			MyTypes.BuildingTypes.Blacksmith => BlacksmithItems,
			MyTypes.BuildingTypes.Goldsmith => GoldsmithItems,
			MyTypes.BuildingTypes.Alchemist => AlchemistItems,
			MyTypes.BuildingTypes.Fletcher => FletcherItems,
			MyTypes.BuildingTypes.Arcanist => ArcanistItems,
			MyTypes.BuildingTypes.Enchanter => EnchanterItems,
			_ => null
		};
	}

	private Array<ItemBase> GenerateRandomStoreItemsForBuilding(MyTypes.BuildingTypes buildingType, MyTypes.Biomes biome) {
		var placeholderimage = new PlaceholderTexture2D();
		placeholderimage.Size = new Vector2(64, 64);
		GD.Print($"Generating items for {buildingType} in {biome} biome");
		return new Array<ItemBase>() {
			new ItemBase($"{buildingType} Item 1", null, placeholderimage, 10, 100),
			new ItemConsumable($"{buildingType} Consumable 1", null, null, placeholderimage, 10, 50, 5, 5),
			new ItemEquipable($"{buildingType} Equipable 1", null, null, placeholderimage, 1, 200, placeholderimage) {
				HasCondition = true,
				ConditionMax = 100,
				ConditionDefault = 100,
				ConditionCurrent = 100
			},
			new ItemArmor($"{buildingType} Armor 1", null, placeholderimage, 1, 300,
				new Defence(
					new Dictionary<DamageType, float> { { DamageType.Slashing, 35f }, { DamageType.Fire, 20f } },
					new Dictionary<StatusEffectType, float> { { StatusEffectType.Burn, 0.5f } }
				)
			) {
				HasCondition = true,
				ConditionMax = 100,
				ConditionDefault = 100,
				ConditionCurrent = 100
			}
		};
	}

	public void ClearAllItems() {
		TavernItems = new Array<ItemBase>();
		MerchantItems = new Array<ItemBase>();
		BlacksmithItems = new Array<ItemBase>();
		GoldsmithItems = new Array<ItemBase>();
		AlchemistItems = new Array<ItemBase>();
		FletcherItems = new Array<ItemBase>();
		ArcanistItems = new Array<ItemBase>();
		EnchanterItems = new Array<ItemBase>();
	}

	public override string ToString() {
		return $"StoreItemData:\n"
			+ $"    Tavern={FormatItems(TavernItems)}\n"
			+ $"    Merchant={FormatItems(MerchantItems)}\n"
			+ $"    Blacksmith={FormatItems(BlacksmithItems)}\n"
			+ $"    Goldsmith={FormatItems(GoldsmithItems)}\n"
			+ $"    Alchemist={FormatItems(AlchemistItems)}\n"
			+ $"    Fletcher={FormatItems(FletcherItems)}\n"
			+ $"    Arcanist={FormatItems(ArcanistItems)}\n"
			+ $"    Enchanter={FormatItems(EnchanterItems)}";
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
