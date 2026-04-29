using Godot;
using MyTypes;

[GlobalClass]
public partial class DependencyLevel : Dependency {
	[Export] public PlayerSkillData.PlayerSkillType SkillType { get; set; } = PlayerSkillData.PlayerSkillType.Strength;
	[Export] public int RequiredLevel { get; set; } = 1;

	public override bool IsMet(PlayerSkillData playerSkills = null, AmmoType? ammoType = null, int? mana = null) {
		return playerSkills != null && playerSkills.GetSkillLevel(SkillType) >= RequiredLevel;
	}
}
