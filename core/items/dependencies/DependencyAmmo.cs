using Godot;
using MyTypes;

[GlobalClass]
public partial class DependencyAmmo : Dependency {
	[Export] public AmmoType AmmoType { get; set; }

	public override bool IsMet(PlayerSkillData playerSkills = null, AmmoType? ammoType = null, int? mana = null) {
		return ammoType != null && ammoType == AmmoType;
	}
}
