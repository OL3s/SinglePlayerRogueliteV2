using Godot;

[GlobalClass]
public partial class ChangeOverlayScene : OverlayButton
{
	[Export] public PackedScene SceneToLoad { get; set; }

	protected override void HandleOverlayAction()
	{
		if (SceneToLoad == null)
		{
			GD.PushWarning($"{nameof(ChangeOverlayScene)} on '{Name}' has no scene configured.");
			return;
		}

		GlobalOverlay.Get()?.ChangeRootScene(SceneToLoad);
	}
}
