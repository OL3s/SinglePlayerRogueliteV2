#nullable enable

using Godot;

[GlobalClass]
public partial class ItemEquipable : ItemBase {
	[Export] public Texture2D EquippedTexture { get; set; } = new PlaceholderTexture2D();
	[Export] public int ConditionMax { get; set; } = 0;
	[Export] public int ConditionCurrent { get; set; } = 0;
	[Export] public ItemDepencency? Dependency { get; set; } = null;

	public ItemEquipable() { }

	public ItemEquipable(string itemName, DependencyLevel? useDependency, Texture2D icon, int maxStackSize, int cost, Texture2D equippedTexture, int conditionMax, int conditionCurrent, ItemDepencency? dependency)
		: base(itemName, useDependency, icon, maxStackSize, cost) {
		EquippedTexture = equippedTexture;
		ConditionMax = conditionMax;
		ConditionCurrent = conditionCurrent;
		Dependency = dependency;
	}
}
