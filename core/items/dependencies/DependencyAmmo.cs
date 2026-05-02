using Godot;
using MyTypes;

[GlobalClass]
public partial class DependencyAmmo : Dependency {
	[Export] public AmmoType AmmoType { get; set; }
	[Export] public int UseCountCost { get; set; } = 1;

	internal override bool IsMet(ActionContext context) {
		if (context is not ItemUseContext itemContext || itemContext.CurrentAmmoType == null || itemContext.CurrentAmmoType != AmmoType)
			return false;

		return itemContext.AmmoItem == null || !itemContext.AmmoItem.IsConsumable || itemContext.AmmoItem.UseCountCurrent >= UseCountCost;
	}

	internal override bool ApplyCost(ActionContext context) {
		return context is ItemUseContext itemContext && itemContext.ConsumeAmmo(UseCountCost);
	}
}
