using MyTypes;

public class ItemUseContext : ActionContext {
	public ItemBase Item { get; set; }
	public AmmoType? AmmoType { get; set; }
	public ItemAmmo AmmoItem { get; set; }
	public int UseCountCost { get; set; } = 1;
	public int ConditionDamage { get; set; } = 0;
	public AmmoType? CurrentAmmoType => AmmoItem != null ? AmmoItem.AmmoType : AmmoType;

	internal bool ConsumeAmmo(int amount) {
		if (amount <= 0)
			return true;

		return AmmoItem == null || AmmoItem.ConsumeUse(amount);
	}
}
