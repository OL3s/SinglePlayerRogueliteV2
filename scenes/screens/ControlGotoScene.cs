using Godot;

[GlobalClass]
public partial class ControlGotoScene : Control {
	[Export] public PackedScene SceneToLoad { get; set; }
	[Export] public bool ChangeScene { get; set; }

	public override void _GuiInput(InputEvent @event) {
		if (IsPressed(@event)) {
			AcceptEvent();
			CallDeferred(MethodName.GoToScene);
		}
	}

	public void GoToScene() {
		if (SceneToLoad == null) {
			GD.PushWarning($"{nameof(ControlGotoScene)} on '{Name}' has no scene configured.");
			return;
		}

		var overlay = GlobalOverlay.Get();
		if (overlay == null) {
			GD.PushWarning($"{nameof(ControlGotoScene)} on '{Name}' could not find GlobalOverlay.");
			return;
		}

		if (ChangeScene) {
			overlay.ChangeRootScene(SceneToLoad);
			return;
		}

		overlay.AddOverlay(SceneToLoad);
	}

	private static bool IsPressed(InputEvent @event) {
		return @event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false }
			|| @event is InputEventScreenTouch { Pressed: false };
	}
}
