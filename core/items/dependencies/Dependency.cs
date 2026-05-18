using Godot;

[GlobalClass]
public abstract partial class Dependency : Resource {
	internal abstract bool IsMet(ItemUseContext context);

	internal virtual bool ApplyCost(ItemUseContext context) {
		return true;
	}
}
