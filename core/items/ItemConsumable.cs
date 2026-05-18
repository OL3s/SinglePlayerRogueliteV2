using Godot;

#nullable enable

[GlobalClass]
public partial class ItemConsumable : ItemBase {
	public ItemConsumable() {
		IsConsumable = true;
	}

	public ItemConsumable(string itemName, ItemDependency? dependencies, Texture2D icon, int maxStackSize, int cost, int useCountMax, int useCountDefault)
		: base(itemName, dependencies, icon, maxStackSize, cost) {
		IsConsumable = true;
		UseCountMax = useCountMax;
		UseCountDefault = useCountDefault;
		UseCountCurrent = useCountDefault;
	}
}
