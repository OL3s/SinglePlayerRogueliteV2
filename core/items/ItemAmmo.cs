#nullable enable

using Godot;
using MyTypes;

[GlobalClass]
public partial class ItemAmmo : ItemBase {
	public ItemAmmo() {
		IsConsumable = true;
	}

	public ItemAmmo(string itemName, ItemDependency? dependencies, Texture2D icon, int maxStackSize, int cost, int useCountMax, int useCountDefault, AmmoType ammoType)
		: base(itemName, dependencies, icon, maxStackSize, cost) {
		IsConsumable = true;
		UseCountMax = useCountMax;
		UseCountDefault = useCountDefault;
		UseCountCurrent = useCountDefault;
		AmmoType = ammoType;
	}
}
