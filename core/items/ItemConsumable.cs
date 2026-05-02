using Godot;

#nullable enable

[GlobalClass]
public partial class ItemConsumable : ItemUsable {
	public ItemConsumable() {
		IsConsumable = true;
	}

	public ItemConsumable(string itemName, ItemDependency? dependencies, PlayerAction? action, Texture2D icon, int maxStackSize, int cost, int useCountMax, int useCountDefault)
		: base(itemName, dependencies, action, icon, maxStackSize, cost) {
		IsConsumable = true;
		UseCountMax = useCountMax;
		UseCountDefault = useCountDefault;
		UseCountCurrent = useCountDefault;
	}
}
