using Godot;

[GlobalClass]
public partial class ChangeOverlayScene : OverlayButton {
	[Export] public PackedScene SceneToLoad { get; set; }

	protected override void HandleOverlayAction() {
		var scene = SceneToLoad;

		if (scene == null) {
			GD.PushWarning($"{nameof(ChangeOverlayScene)} on '{Name}' has no scene configured.");
			return;
		}

		GlobalOverlay.Get()?.ChangeRootScene(scene);
	}
}
