using Godot;

[GlobalClass]
public partial class PlayerSkillData : Resource {
	private const int XpPerLevel = 100;

	[Export] public int StrengthXp { get; set; } = 0;
	[Export] public int AgilityXp { get; set; } = 0;
	[Export] public int ArcanaXp { get; set; } = 0;
	[Export] public int VitalityXp { get; set; } = 0;

	public int GetLevel(int currentXp) {
		return (currentXp / XpPerLevel) + 1;
	}

	public int GetStrengthLevel() {
		return GetLevel(StrengthXp);
	}

	public int GetAgilityLevel() {
		return GetLevel(AgilityXp);
	}

	public int GetArcanaLevel() {
		return GetLevel(ArcanaXp);
	}

	public int GetVitalityLevel() {
		return GetLevel(VitalityXp);
	}

	public int GetTotalLevel() {
		return GetStrengthLevel() + GetAgilityLevel() + GetArcanaLevel() + GetVitalityLevel();
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
