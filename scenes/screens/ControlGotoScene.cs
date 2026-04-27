using Godot;

[GlobalClass]
public partial class ControlGotoScene : Control {
	private const string NewCharacterScenePath = "res://scenes/characterSelect/characterSelectNew.tscn";

	[Export] public PackedScene SceneToLoad { get; set; }
	[Export] public bool ChangeScene { get; set; }

	public override void _GuiInput(InputEvent @event) {
		if (IsPressed(@event)) {
			AcceptEvent();
			CallDeferred(MethodName.GoToScene);
		}
	}

	public void GoToScene() {
		var sceneToLoad = GetSceneToLoad();

		if (sceneToLoad == null) {
			GD.PushWarning($"{nameof(ControlGotoScene)} on '{Name}' has no scene configured.");
			return;
		}

		var overlay = GlobalOverlay.Get();
		if (overlay == null) {
			GD.PushWarning($"{nameof(ControlGotoScene)} on '{Name}' could not find GlobalOverlay.");
			return;
		}

		if (ChangeScene) {
			overlay.ChangeRootScene(sceneToLoad);
			return;
		}

		overlay.AddOverlay(sceneToLoad);
	}

	private PackedScene GetSceneToLoad() {
		var saveNode = SaveNode.Get();
		if (!saveNode.HadPlayerDataOnLoad)
			return ResourceLoader.Load<PackedScene>(NewCharacterScenePath);

		return SceneToLoad;
	}

	private static bool IsPressed(InputEvent @event) {
		return @event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false }
			|| @event is InputEventScreenTouch { Pressed: false };
	}
}
