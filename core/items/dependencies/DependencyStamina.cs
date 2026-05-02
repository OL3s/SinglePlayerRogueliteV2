using Godot;

[GlobalClass]
public partial class DependencyStamina : Dependency {
	[Export] public int Cost { get; set; }

	internal override bool IsMet(ActionContext context) {
		return context?.Stamina != null && context.Stamina >= Cost;
	}

	internal override bool ApplyCost(ActionContext context) {
		return context != null && context.SpendStamina(Cost);
	}
}
