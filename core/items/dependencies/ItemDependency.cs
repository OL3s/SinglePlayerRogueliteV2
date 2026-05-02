using Godot;
using Godot.Collections;
using MyTypes;

[GlobalClass]
public partial class ItemDependency : Resource {
	[Export] public Array<Dependency> Dependencies { get; set; } = new();

	public void AddDependency(Dependency dependency) {
		if (dependency == null)
			return;

		Dependencies.Add(dependency);
	}

	internal static bool CanExecute(Array<Dependency> dependencies, ActionContext context) {
		foreach (var dependency in dependencies) {
			if (dependency != null && !dependency.IsMet(context))
				return false;
		}

		return true;
	}

	internal static bool CanExecute(Array<Dependency> dependencies, PlayerSkillData playerSkills = null, AmmoType? ammoType = null, int? mana = null, int? stamina = null) {
		return CanExecute(dependencies, new ActionContext {
			PlayerSkills = playerSkills,
			Mana = mana,
			Stamina = stamina
		});
	}

	internal static bool CanExecute(Array<Dependency> dependencies, PlayerSkillData playerSkills = null, AmmoType? ammoType = null, ItemAmmo ammoItem = null, int? mana = null, int? stamina = null) {
		return CanExecute(dependencies, new ItemUseContext {
			PlayerSkills = playerSkills,
			AmmoType = ammoType,
			AmmoItem = ammoItem,
			Mana = mana,
			Stamina = stamina
		});
	}

	internal bool CanExecute(ActionContext context) {
		return CanExecute(Dependencies, context);
	}

	internal bool CanExecute(PlayerSkillData playerSkills = null, AmmoType? ammoType = null, int? mana = null, int? stamina = null) {
		return CanExecute(Dependencies, playerSkills, ammoType, mana, stamina);
	}

	internal bool ApplyCosts(ActionContext context) {
		foreach (var dependency in Dependencies) {
			if (dependency != null && !dependency.ApplyCost(context))
				return false;
		}

		return true;
	}

	public AmmoType? GetRequiredAmmoType() {
		foreach (var dependency in Dependencies) {
			if (dependency is DependencyAmmo ammoDependency) return ammoDependency.AmmoType;
		}

		return null;
	}

	public int? GetManaCost() {
		foreach (var dependency in Dependencies) {
			if (dependency is DependencyMana manaDependency) return manaDependency.Cost;
		}

		return null;
	}

	public int? GetStaminaCost() {
		foreach (var dependency in Dependencies) {
			if (dependency is DependencyStamina staminaDependency) return staminaDependency.Cost;
		}

		return null;
	}

	public int? GetRequiredSkillLevel(PlayerSkillData.PlayerSkillType skillType) {
		foreach (var dependency in Dependencies) {
			if (dependency is DependencyLevel levelDependency && levelDependency.SkillType == skillType)
				return levelDependency.RequiredLevel;
		}

		return null;
	}
}
