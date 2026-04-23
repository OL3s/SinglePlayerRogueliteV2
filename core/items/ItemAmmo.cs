#nullable enable

using Godot;
using MyTypes;

[GlobalClass]
public partial class ItemAmmo : ItemBase
{
	[Export] public int CountMax { get; set; } = 0;
	[Export] public int CountCurrent { get; set; } = 0;
	[Export] public AmmoType Type { get; set; } = AmmoType.None;
	public ItemAmmo() { }

	public ItemAmmo(string itemName, DependencyLevel? useDependency, Texture2D icon, int maxStackSize, int cost, int countMax, int countCurrent, AmmoType type)
		: base(itemName, useDependency, icon, maxStackSize, cost)
	{
		CountMax = countMax;
		CountCurrent = countCurrent;
		Type = type;
	}
}
