using Godot;
using MyTypes;

[GlobalClass]
public partial class DependencyAmmo : Dependency {
	[Export] public AmmoType AmmoType { get; set; }
	[Export] public int UseCountCost { get; set; } = 1;

	internal override bool IsMet(ActionContext context) {
		if (context is not ItemUseContext itemContext) {
			GD.PushError("DependencyAmmo: Ammo dependency requires an ItemUseContext.");
			return false;
		}

		var ammoItem = itemContext.GetCurrentAmmoItem();
		if (ammoItem == null) {
			GD.PushError($"DependencyAmmo: Expected ammo item of type {AmmoType}, but none was found.");
			return false;
		}

		if (ammoItem.AmmoType != AmmoType) {
			GD.PushError($"DependencyAmmo: Expected ammo type {AmmoType}, but found {ammoItem.AmmoType}.");
			return false;
		}

		return !ammoItem.IsConsumable || ammoItem.UseCountCurrent >= UseCountCost;
	}

	internal override bool ApplyCost(ActionContext context) {
		return context is ItemUseContext itemContext && itemContext.ConsumeAmmo(UseCountCost);
	}
}
