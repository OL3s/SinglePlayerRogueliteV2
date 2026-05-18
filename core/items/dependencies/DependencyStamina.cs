using Godot;

[GlobalClass]
public partial class DependencyStamina : Dependency {
	[Export] public int Cost { get; set; }

	internal override bool IsMet(ItemUseContext context) {
		return context?.Stamina != null && context.Stamina >= Cost;
	}

	internal override bool ApplyCost(ItemUseContext context) {
		return context != null && context.SpendStamina(Cost);
	}
}
