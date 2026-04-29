using Godot;
using MyTypes;

[GlobalClass]
public partial class DependencyAmmo : Dependency {
	[Export] public AmmoType AmmoType { get; set; }
	[Export] public int UseCountCost { get; set; } = 1;

	internal override bool IsMet(ItemUseContext context) {
		if (context?.CurrentAmmoType == null || context.CurrentAmmoType != AmmoType)
			return false;

		return context.AmmoItem == null || !context.AmmoItem.IsConsumable || context.AmmoItem.UseCountCurrent >= UseCountCost;
	}

	internal override bool ApplyCost(ItemUseContext context) {
		return context != null && context.ConsumeAmmo(UseCountCost);
	}
}
