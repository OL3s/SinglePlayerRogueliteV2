using Godot;

[GlobalClass]
public partial class PlayerSkillData : Resource {
	public enum PlayerSkillType {
		Strength,
		Agility,
		Arcana,
		Vitality
	}

	private const int XpPerLevel = 100;

	[Export] public int StrengthXp { get; set; } = 0;
	[Export] public int AgilityXp { get; set; } = 0;
	[Export] public int ArcanaXp { get; set; } = 0;
	[Export] public int VitalityXp { get; set; } = 0;

	public int GetLevel(int currentXp) {
		return GetLevelFromXp(currentXp);
	}

	public static int GetLevelFromXp(int currentXp) {
		return (Mathf.Max(0, currentXp) / XpPerLevel) + 1;
	}

	public int GetXp(PlayerSkillType skillType) {
		return skillType switch {
			PlayerSkillType.Strength => StrengthXp,
			PlayerSkillType.Agility => AgilityXp,
			PlayerSkillType.Arcana => ArcanaXp,
			PlayerSkillType.Vitality => VitalityXp,
			_ => 0
		};
	}

	public int GetSkillLevel(PlayerSkillType skillType) {
		return GetLevel(GetXp(skillType));
	}

	public int GetStrengthLevel() {
		return GetSkillLevel(PlayerSkillType.Strength);
	}

	public int GetAgilityLevel() {
		return GetSkillLevel(PlayerSkillType.Agility);
	}

	public int GetArcanaLevel() {
		return GetSkillLevel(PlayerSkillType.Arcana);
	}

	public int GetVitalityLevel() {
		return GetSkillLevel(PlayerSkillType.Vitality);
	}

	public int GetTotalLevel() {
		return GetStrengthLevel() + GetAgilityLevel() + GetArcanaLevel() + GetVitalityLevel();
	}

	public int GetTotalXp() {
		return StrengthXp + AgilityXp + ArcanaXp + VitalityXp;
	}

	public override string ToString() {
		return $"PlayerSkillData:\n"
			+ $"    StrengthXp={StrengthXp}, StrengthLevel={GetStrengthLevel()}\n"
			+ $"    AgilityXp={AgilityXp}, AgilityLevel={GetAgilityLevel()}\n"
			+ $"    ArcanaXp={ArcanaXp}, ArcanaLevel={GetArcanaLevel()}\n"
			+ $"    VitalityXp={VitalityXp}, VitalityLevel={GetVitalityLevel()}\n"
			+ $"    TotalLevel={GetTotalLevel()}";
	}
}
