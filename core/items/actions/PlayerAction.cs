using Godot;

[GlobalClass]
public abstract partial class PlayerAction : Resource {
	public virtual bool CanExecute(ActionContext context) {
		return context != null;
	}

	public abstract bool Execute(ActionContext context);
}
