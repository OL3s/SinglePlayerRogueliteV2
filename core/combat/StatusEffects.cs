using Godot.Collections;
using Combat;
using Godot;

[Tool]
public partial class StatusEffects : Resource
{
	[Export] public Dictionary<StatusEffectType, int> ActiveStatusEffects { get; set; } = new Dictionary<StatusEffectType, int>();
	public StatusEffects() { }
	public StatusEffects(Dictionary<StatusEffectType, int> statusEffects)
	{
		foreach (var statusEffect in statusEffects.Keys)
		{
			ActiveStatusEffects[statusEffect] = statusEffects[statusEffect];
		}
	}

	public void Tick(CombatContainer combatContainer)
	{
		foreach (var statusEffect in ActiveStatusEffects.Keys)
		{
			int strength = ActiveStatusEffects[statusEffect];
			switch (statusEffect)
			{
				case StatusEffectType.Burn:
					combatContainer.CurrentHealth -= (int)Mathf.Floor(strength / 10f);
					break;
				case StatusEffectType.Freeze:
					break;
				case StatusEffectType.Shock:
					break;
				case StatusEffectType.Poison:
					combatContainer.CurrentHealth -= (int)Mathf.Floor(combatContainer.MaxHealth * (strength / 100f));
					break;
			}

			ActiveStatusEffects[statusEffect] -= 1;
			if (ActiveStatusEffects[statusEffect] <= 0)
			{
				ActiveStatusEffects.Remove(statusEffect);
			}
		}
	}
}
