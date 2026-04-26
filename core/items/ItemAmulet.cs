#nullable enable

using Godot;

[GlobalClass]
public partial class ItemAmulet : ItemBase {
	public ItemAmulet() { }
	public ItemAmulet(string itemName, DependencyLevel? useDependency, Texture2D icon, int maxStackSize, int cost)
		: base(itemName, useDependency, icon, maxStackSize, cost) {
	}
}
