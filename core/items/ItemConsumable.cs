using Godot;

#nullable enable

[GlobalClass]
public partial class ItemConsumable : ItemBase {
	[Export] public int CountMax { get; set; } = 0;
	[Export] public int CountCurrent { get; set; } = 0;
	public ItemConsumable() { }
	public ItemConsumable(string itemName, DependencyLevel? useDependency, Texture2D icon, int maxStackSize, int cost, int countMax, int countCurrent)
		: base(itemName, useDependency, icon, maxStackSize, cost) {
		CountMax = countMax;
		CountCurrent = countCurrent;
	}
}
