#nullable enable

using Godot;
using Godot.Collections;
using System;

namespace Combat {
	[GlobalClass]
	public partial class Defence : Resource
	{
		[ExportGroup("Defence Percentages")]
		[Export] public Dictionary<DamageType, float> DamageReflectPercentages { get; set; } = new Dictionary<DamageType, float>();
		[Export] public Dictionary<StatusEffectType, float> StatusEffectReflectPercentages { get; set; } = new Dictionary<StatusEffectType, float>();
		[ExportGroup("Flat Defence Overrides")]
		[Export(PropertyHint.Range, "0,1,hide_slider")] public float FlatDamagePercentage { get; set; } = 0f;
		[Export(PropertyHint.Range, "0,1,hide_slider")] public float FlatEffectPercentage { get; set; } = 0f;
		public Defence() { }
		public Defence(Dictionary<DamageType, float>? damageReflectPercentages, Dictionary<StatusEffectType, float>? statusEffectReflectPercentages, float flatDamagePercentage = 0, float flatEffectPercentage = 0)
		{
			if (flatDamagePercentage < 0 || flatDamagePercentage > 1 || flatEffectPercentage < 0 || flatEffectPercentage > 1)
				throw new ArgumentException("Flat damage and effect percentages must be between 0 and 1.");

			FlatDamagePercentage = flatDamagePercentage;
			FlatEffectPercentage = flatEffectPercentage;
			DamageReflectPercentages = damageReflectPercentages ?? new Dictionary<DamageType, float>();
			StatusEffectReflectPercentages = statusEffectReflectPercentages ?? new Dictionary<StatusEffectType, float>();

			if (FlatDamagePercentage != 0)
			{
				foreach (var damageType in Enum.GetValues<DamageType>())
				{
					if (!DamageReflectPercentages.ContainsKey(damageType))
						DamageReflectPercentages[damageType] = FlatDamagePercentage;
				}
			}
			if (FlatEffectPercentage != 0)
			{
				foreach (var effectType in Enum.GetValues<StatusEffectType>())
				{
					if (!StatusEffectReflectPercentages.ContainsKey(effectType))
						StatusEffectReflectPercentages[effectType] = FlatEffectPercentage;
				}
			}
		}
	}
}
