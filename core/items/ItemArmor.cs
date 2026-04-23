#nullable enable

using Godot;
using Combat;

[GlobalClass]
public partial class ItemArmor : ItemBase
{
	[Export] public int ConditionMax { get; set; } = 0;
	[Export] public int ConditionCurrent { get; set; } = 0;
	[Export] public Defence Defence { get; set; } = new Defence();
	public ItemArmor() { }

	public ItemArmor(string itemName, DependencyLevel? useDependency, Texture2D icon, int maxStackSize, int cost, int conditionMax, int conditionCurrent, Defence defence)
		: base(itemName, useDependency, icon, maxStackSize, cost)
	{
		ConditionMax = conditionMax;
		ConditionCurrent = conditionCurrent;
		Defence = defence;
	}
}
