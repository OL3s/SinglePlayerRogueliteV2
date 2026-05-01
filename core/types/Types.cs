using Godot;

namespace MyTypes {

	public enum Biomes {
		Undefined = 0,
		GrasslandsA = 1,
		TundraB = 2,
		DesertB = 3,
		IcyC = 4,
		JungleC = 5,
		LavaC = 6,
		IceBossD = 7,
		JungleBossD = 8,
		LavaBossD = 9,
	}

	public enum Locations {
		Undefined = 0,
		Village = 1,
		Sanctuary = 2,
		Campsite = 3
	}

	public enum BuildingTypes {
		Tavern,
		Merchant,
		Blacksmith,
		Goldsmith,
		Alchemist,
		Fletcher,
		Arcanist,
		Enchanter,
	}

	public enum OutpostBuildingCompatibility {
		None = 0,
		General = 1,
		Smith = 2,
		Jewelry = 3,
		Alchemy = 4,
		Fletching = 5,
		Arcana = 6,
		Enchanting = 7,
	}

	public enum AmmoType {
		None,
		Arrow
	}

	public enum ItemType {
		ItemBase,
		ItemEquipable,
		ItemConsumable,
		ItemAmmo,
		ItemArmor,
		ItemAmulet
	}
}

namespace SaveData {
	public enum FileType {
		Meta,
		Run,
		Settings
	}
}
