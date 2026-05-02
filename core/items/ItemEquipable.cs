#nullable enable

using Godot;

[GlobalClass]
public partial class ItemEquipable : ItemUsable {
	[Export] public Texture2D EquippedTexture { get; set; } = new PlaceholderTexture2D();

	public ItemEquipable() { }

	public ItemEquipable(string itemName, ItemDependency? dependencies, PlayerAction? action, Texture2D icon, int maxStackSize, int cost, Texture2D equippedTexture)
		: base(itemName, dependencies, action, icon, maxStackSize, cost) {
		EquippedTexture = equippedTexture;
	}
}
