using Godot;
using MyTypes;

public class ItemUseContext : ActionContext {
	public ItemBase Item { get; set; }
	public AmmoType? AmmoType { get; set; }
	public ItemAmmo AmmoItem { get; set; }
	public int UseCountCost { get; set; } = 1;
	public int ConditionDamage { get; set; } = 0;
	public AmmoType? CurrentAmmoType => GetCurrentAmmoItem() != null ? GetCurrentAmmoItem().AmmoType : AmmoType;

	internal ItemAmmo GetCurrentAmmoItem() {
		if (AmmoItem != null)
			return AmmoItem;

		if (AmmoType != null && PlayerStats != null)
			return PlayerStats.GetAmmoItem(AmmoType.Value);

		return null;
	}

	internal bool ConsumeAmmo(int amount) {
		if (amount <= 0)
			return true;

		var ammoItem = GetCurrentAmmoItem();
		if (ammoItem == null) {
			GD.PushError("ItemUseContext: Cannot consume ammo because no ammo item was found.");
			return false;
		}

		return ammoItem.ConsumeUse(amount);
	}
}
