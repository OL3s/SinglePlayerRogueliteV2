using Godot;

[GlobalClass]
public partial class ChangeOverlayScene : OverlayButton {
	[Export] public PackedScene SceneToLoad { get; set; }
	[Export(PropertyHint.File, "*.tscn")] public string ScenePath { get; set; }

	protected override void HandleOverlayAction() {
		var scene = SceneToLoad;
		if (scene == null && !string.IsNullOrWhiteSpace(ScenePath)) scene = ResourceLoader.Load<PackedScene>(ScenePath);

		if (scene == null) {
			GD.PushWarning($"{nameof(ChangeOverlayScene)} on '{Name}' has no scene configured.");
			return;
		}

		GlobalOverlay.Get()?.ChangeRootScene(scene);
	}
}
