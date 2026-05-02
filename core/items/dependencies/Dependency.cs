using Godot;

[GlobalClass]
public abstract partial class Dependency : Resource {
	internal abstract bool IsMet(ActionContext context);

	internal virtual bool ApplyCost(ActionContext context) {
		return true;
	}
}
