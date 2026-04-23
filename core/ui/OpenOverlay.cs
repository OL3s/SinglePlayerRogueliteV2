using Godot;

[GlobalClass]
public partial class OpenOverlay : OverlayButton
{
	[Export] public PackedScene OverlayScene { get; set; }

	protected override void HandleOverlayAction()
	{
		if (OverlayScene != null)
		{
			GlobalOverlay.Get()?.AddOverlay(OverlayScene);
			return;
		}

		GD.PushWarning($"{nameof(OpenOverlay)} on '{Name}' has no overlay configured.");
	}
}
