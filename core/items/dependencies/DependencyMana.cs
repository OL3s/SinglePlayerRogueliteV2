using Godot;
using MyTypes;

[GlobalClass]
public partial class DependencyMana : Dependency {
	[Export] public int Cost { get; set; }

	public override bool IsMet(PlayerSkillData playerSkills = null, AmmoType? ammoType = null, int? mana = null) {
		return mana != null && mana >= Cost;
	}
}
