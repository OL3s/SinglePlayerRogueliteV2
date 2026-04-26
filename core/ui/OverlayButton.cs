using Godot;

[GlobalClass]
public abstract partial class OverlayButton : Button
{
	public override void _Pressed()
	{
		base._Pressed();
		CallDeferred(MethodName.HandleOverlayActionDeferred);
	}

	private void HandleOverlayActionDeferred()
	{
		HandleOverlayAction();
	}

	protected abstract void HandleOverlayAction();
}
