using Godot;
using MyTypes;

[GlobalClass]
public partial class DependencyLevel : Dependency {
	[Export] public PlayerSkillData.PlayerSkillType SkillType { get; set; } = PlayerSkillData.PlayerSkillType.Strength;
	[Export] public int RequiredLevel { get; set; } = 1;

	internal override bool IsMet(ActionContext context) {
		return context?.PlayerSkills != null && GetCurrentLevel(context.PlayerSkills) >= RequiredLevel;
	}

	private int GetCurrentLevel(PlayerSkillData playerSkills) {
		return SkillType switch {
			PlayerSkillData.PlayerSkillType.Strength => playerSkills.GetStrengthLevel(),
			PlayerSkillData.PlayerSkillType.Agility => playerSkills.GetAgilityLevel(),
			PlayerSkillData.PlayerSkillType.Arcana => playerSkills.GetArcanaLevel(),
			PlayerSkillData.PlayerSkillType.Vitality => playerSkills.GetVitalityLevel(),
			_ => 0
		};
	}
}
