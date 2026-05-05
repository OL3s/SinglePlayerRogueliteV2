using Godot;
using MyTypes;

[GlobalClass]
public partial class PlayerRuntimeStats : Resource {
	private const int BaseMaxHealth = 100;
	private const int HealthPerVitalityLevel = 10;
	private const int BaseMaxMana = 50;
	private const int ManaPerArcanaLevel = 5;
	private const int BaseMaxStamina = 50;
	private const int StaminaPerAgilityLevel = 5;

	[Export] public int CurrentHealth { get; set; } = BaseMaxHealth;
	[Export] public int CurrentMana { get; set; } = BaseMaxMana;
	[Export] public int CurrentStamina { get; set; } = BaseMaxStamina;
	[Export] public ItemEquipable MainHandItem { get; set; }
	[Export] public ItemEquipable OffHandItem { get; set; }
	[Export] public ItemEquipable ActiveHandItem { get; set; }
	[Export] public ItemArmor ArmorItem { get; set; }
	[Export] public ItemAmulet AmuletItem { get; set; }
	[Export] public ItemAmmo AmmoItem { get; set; }
	[Export] public ItemAmmo StoredAmmoItem { get; set; }
	[Export] public ItemConsumable ConsumableItem { get; set; }

	public void ResetToDefaults(PlayerSkillData skills) {
		CurrentHealth = GetMaxHealth(skills);
		CurrentMana = GetMaxMana(skills);
		CurrentStamina = GetMaxStamina(skills);
	}

	public static PlayerRuntimeStats CreateWithDefaults(PlayerData playerData) {
		if (playerData == null)
			GD.PushError("PlayerRuntimeStats: Cannot create defaults because player data is missing.");

		var skills = playerData?.Skills;
		if (skills == null)
			GD.PushError("PlayerRuntimeStats: Cannot calculate player stat defaults because skill data is missing.");

		var stats = new PlayerRuntimeStats();
		stats.ResetToDefaults(skills);
		stats.SetEquipment(playerData?.EquipedItems);
		stats.SetStoredAmmo(playerData?.InventoryData);
		return stats;
	}

	public void SetEquipment(EquipedItemsData equipedItems) {
		if (equipedItems == null)
			GD.PushError("PlayerRuntimeStats: Cannot resolve equipped items because equipment data is missing.");

		MainHandItem = equipedItems?.MainHandItem;
		OffHandItem = equipedItems?.OffHandItem;
		ActiveHandItem = MainHandItem ?? OffHandItem;
		ArmorItem = equipedItems?.ArmorItem;
		AmuletItem = equipedItems?.AmuletItem;
		AmmoItem = equipedItems?.AmmoItem;
		ConsumableItem = equipedItems?.ConsumableItem;
	}

	public void SetStoredAmmo(InventoryData inventoryData) {
		StoredAmmoItem = null;

		if (inventoryData?.Items == null) {
			GD.PushError("PlayerRuntimeStats: Cannot resolve stored ammo because inventory data is missing.");
			return;
		}

		foreach (var item in inventoryData.Items) {
			if (item is not ItemAmmo ammoItem)
				continue;

			if (StoredAmmoItem != null) {
				GD.PushError("PlayerRuntimeStats: Multiple stored ammo items were found, but runtime stats expects a single stored ammo item.");
				return;
			}

			StoredAmmoItem = ammoItem;
		}
	}

	public ItemAmmo GetAmmoItem(AmmoType ammoType) {
		if (AmmoItem != null && AmmoItem.AmmoType == ammoType)
			return AmmoItem;

		if (StoredAmmoItem != null && StoredAmmoItem.AmmoType == ammoType)
			return StoredAmmoItem;

		GD.PushError($"PlayerRuntimeStats: Expected ammo item of type {ammoType}, but none was equipped or stored.");

		return null;
	}

	public int GetAmmoCount(AmmoType ammoType) {
		var ammoItem = GetAmmoItem(ammoType);
		var count = ammoItem?.UseCountCurrent ?? 0;

		return count;
	}

	public static int GetMaxHealth(PlayerSkillData skills) {
		return BaseMaxHealth + ((skills?.GetVitalityLevel() ?? 1) - 1) * HealthPerVitalityLevel;
	}

	public static int GetMaxMana(PlayerSkillData skills) {
		return BaseMaxMana + ((skills?.GetArcanaLevel() ?? 1) - 1) * ManaPerArcanaLevel;
	}

	public static int GetMaxStamina(PlayerSkillData skills) {
		return BaseMaxStamina + ((skills?.GetAgilityLevel() ?? 1) - 1) * StaminaPerAgilityLevel;
	}
}
