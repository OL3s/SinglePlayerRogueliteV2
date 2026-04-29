#nullable enable

using Godot;

[GlobalClass]
public partial class ItemAmulet : ItemBase {
	public ItemAmulet() { }
	public ItemAmulet(string itemName, ItemDependency? dependencies, Texture2D icon, int maxStackSize, int cost)
		: base(itemName, dependencies, icon, maxStackSize, cost) {
	}
}
