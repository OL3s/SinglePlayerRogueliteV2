using MyTypes;

public class ItemUseContext {
	public PlayerSkillData PlayerSkills { get; set; }
	public AmmoType? AmmoType { get; set; }
	public ItemAmmo AmmoItem { get; set; }
	public int? Mana { get; set; }
	public int? Stamina { get; set; }
	public int UseCountCost { get; set; } = 1;
	public int ConditionDamage { get; set; } = 0;
	public AmmoType? CurrentAmmoType => AmmoItem != null ? AmmoItem.AmmoType : AmmoType;

	internal bool SpendMana(int amount) {
		if (amount <= 0)
			return true;

		if (Mana == null || Mana < amount)
			return false;

		Mana -= amount;
		return true;
	}

	internal bool SpendStamina(int amount) {
		if (amount <= 0)
			return true;

		if (Stamina == null || Stamina < amount)
			return false;

		Stamina -= amount;
		return true;
	}

	internal bool ConsumeAmmo(int amount) {
		if (amount <= 0)
			return true;

		return AmmoItem == null || AmmoItem.ConsumeUse(amount);
	}
}
