using Godot;
using MyTypes;

[GlobalClass]
public partial class DependencyMana : Dependency {
	[Export] public int Cost { get; set; }

	internal override bool IsMet(ActionContext context) {
		return context?.Mana != null && context.Mana >= Cost;
	}

	internal override bool ApplyCost(ActionContext context) {
		return context != null && context.SpendMana(Cost);
	}
}
