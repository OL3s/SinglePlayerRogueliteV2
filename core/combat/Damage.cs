using Godot;
using Godot.Collections;

namespace Combat {
	[GlobalClass]
	public partial class Damage : Resource {
		[Export] public Dictionary<DamageType, float> DamageValues { get; set; } = new Dictionary<DamageType, float>();
		[Export] public Dictionary<StatusEffectType, float> StatusEffectValues { get; set; } = new Dictionary<StatusEffectType, float>();

		public Damage() { }
		public Damage(Dictionary<DamageType, float> damageValues, Dictionary<StatusEffectType, float> statusEffectValues) {
			DamageValues = damageValues;
			StatusEffectValues = statusEffectValues;
		}

		public Damage Merge(Damage other) {
			var mergedDamage = new Damage();

			foreach (var damageType in DamageValues.Keys) {
				mergedDamage.DamageValues[damageType] = DamageValues[damageType] + (other.DamageValues.ContainsKey(damageType) ? other.DamageValues[damageType] : 0);
			}

			foreach (var statusEffectType in StatusEffectValues.Keys) {
				mergedDamage.StatusEffectValues[statusEffectType] = StatusEffectValues[statusEffectType] + (other.StatusEffectValues.ContainsKey(statusEffectType) ? other.StatusEffectValues[statusEffectType] : 0);
			}

			return mergedDamage;
		}

		public Damage Merge(Array<Damage> damages) {
			var mergedDamage = new Damage();

			foreach (var damage in damages) {
				mergedDamage = mergedDamage.Merge(damage);
			}

			return mergedDamage;
		}

		public Damage Scale(float multiplier) {
			var scaledDamage = new Damage();

			foreach (var damageType in DamageValues.Keys) {
				scaledDamage.DamageValues[damageType] = DamageValues[damageType] * multiplier;
			}

			foreach (var statusEffectType in StatusEffectValues.Keys) {
				scaledDamage.StatusEffectValues[statusEffectType] = StatusEffectValues[statusEffectType] * multiplier;
			}

			return scaledDamage;
		}
	}
}
