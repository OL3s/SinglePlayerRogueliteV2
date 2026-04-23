using Combat;
using Godot;

[GlobalClass]
public partial class CombatContainer : Resource
{
	[Export] public int MaxHealth { get; set; } = 100;
	[Export] public int CurrentHealth { get; set; } = 100;
	[Export] public StatusEffects ActiveStatusEffects { get; set; } = new StatusEffects();
	[Export] public Defence Defence { get; set; } = new Defence();
	public CombatContainer() { }
	public CombatContainer(int maxHealth, int currentHealth, Defence defence)
	{
		MaxHealth = maxHealth;
		CurrentHealth = currentHealth;
		Defence = defence;
	}
	public void ApplyDamage(Damage damage)
	{
		float totalDamage = 0;
		foreach (var damageType in damage.DamageValues.Keys)
		{
			float damageValue = damage.DamageValues[damageType];
			float reflectPercentage = Defence.DamageReflectPercentages.ContainsKey(damageType) ? Defence.DamageReflectPercentages[damageType] : 0;
			totalDamage += damageValue * (1 - reflectPercentage);
		}
		foreach (var statusEffectType in damage.StatusEffectValues.Keys)
		{
			float statusEffectValue = damage.StatusEffectValues[statusEffectType];
			float reflectPercentage = Defence.StatusEffectReflectPercentages.ContainsKey(statusEffectType) ? Defence.StatusEffectReflectPercentages[statusEffectType] : 0;
			totalDamage += statusEffectValue * (1 - reflectPercentage);
		}
		CurrentHealth -= (int)totalDamage;
	}

	public void ApplyTick()
	{
		ActiveStatusEffects.Tick(this);
	}

	public bool CheckDeath()
	{
		return CurrentHealth <= 0;
	}
}
