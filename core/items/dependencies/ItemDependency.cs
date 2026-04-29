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

	public static bool CanExecute(Array<Dependency> dependencies, PlayerSkillData playerSkills = null, AmmoType? ammoType = null, int? mana = null) {
		foreach (var dependency in dependencies) {
			if (dependency != null && !dependency.IsMet(playerSkills, ammoType, mana))
				return false;
		}

		return true;
	}

	public bool CanExecute(PlayerSkillData playerSkills = null, AmmoType? ammoType = null, int? mana = null) {
		return CanExecute(Dependencies, playerSkills, ammoType, mana);
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

	public int? GetRequiredSkillLevel(PlayerSkillData.PlayerSkillType skillType) {
		foreach (var dependency in Dependencies) {
			if (dependency is DependencyLevel levelDependency && levelDependency.SkillType == skillType)
				return levelDependency.RequiredLevel;
		}

		return null;
	}
}
