#nullable enable

using Godot;
using Combat;

[GlobalClass]
public partial class ItemArmor : ItemBase {
	[Export] public Defence Defence { get; set; } = new Defence();
	public ItemArmor() { }

	public ItemArmor(string itemName, ItemDependency? dependencies, Texture2D icon, int maxStackSize, int cost, Defence defence)
		: base(itemName, dependencies, icon, maxStackSize, cost) {
		Defence = defence;
	}
}
