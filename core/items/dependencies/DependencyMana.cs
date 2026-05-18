using Godot;
using MyTypes;

[GlobalClass]
public partial class DependencyMana : Dependency {
	[Export] public int Cost { get; set; }

	internal override bool IsMet(ItemUseContext context) {
		return context?.Mana != null && context.Mana >= Cost;
	}

	internal override bool ApplyCost(ItemUseContext context) {
		return context != null && context.SpendMana(Cost);
	}
}
