using Godot;
using MyTypes;

[GlobalClass]
public abstract partial class Dependency : Resource {
	public abstract bool IsMet(PlayerSkillData playerSkills = null, AmmoType? ammoType = null, int? mana = null);
}
