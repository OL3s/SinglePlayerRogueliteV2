using Godot;

[GlobalClass]
public abstract partial class OverlayButton : Button
{
	public override void _Pressed()
	{
		base._Pressed();
		HandleOverlayAction();
	}

	protected abstract void HandleOverlayAction();
}
