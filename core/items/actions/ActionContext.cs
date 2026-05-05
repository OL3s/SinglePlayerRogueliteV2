using SaveData;

public class ActionContext {
	public PlayerData PlayerData { get; set; }
	public RunData RunData { get; set; }
	public PlayerSkillData PlayerSkills { get; set; }
	public PlayerRuntimeStats PlayerStats { get; set; }
	public int? Mana { get; set; }
	public int? Stamina { get; set; }

	internal bool SpendMana(int amount) {
		if (amount <= 0)
			return true;

		if (PlayerStats != null) {
			if (PlayerStats.CurrentMana < amount)
				return false;

			PlayerStats.CurrentMana -= amount;
			Mana = PlayerStats.CurrentMana;
			return true;
		}

		if (Mana == null || Mana < amount)
			return false;

		Mana -= amount;
		return true;
	}

	internal bool SpendStamina(int amount) {
		if (amount <= 0)
			return true;

		if (PlayerStats != null) {
			if (PlayerStats.CurrentStamina < amount)
				return false;

			PlayerStats.CurrentStamina -= amount;
			Stamina = PlayerStats.CurrentStamina;
			return true;
		}

		if (Stamina == null || Stamina < amount)
			return false;

		Stamina -= amount;
		return true;
	}
}
