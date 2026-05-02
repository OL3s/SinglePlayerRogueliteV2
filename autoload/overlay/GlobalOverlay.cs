using Godot;

public partial class GlobalOverlay : CanvasLayer {
	private const string PopupBlurBackdropName = "PopupBlurBackdrop";
	private const string BlurShaderPath = "res://assets/shaders/PopupBlurBackdrop.gdshader";
	private const string InfoPopupPanelScenePath = "res://scenes/components/panels/InfoPopupPanel.tscn";
	private static readonly Shader BlurShader = ResourceLoader.Load<Shader>(BlurShaderPath);
	private static readonly PackedScene InfoPopupPanelScene = ResourceLoader.Load<PackedScene>(InfoPopupPanelScenePath);
	private ColorRect _popupBlurBackdrop;
	private InfoPopupPanel _activeInfoPopup;

	public int OverlayCount {
		get {
			var count = 0;
			foreach (Node child in GetChildren()) {
				if (child == _popupBlurBackdrop || child == _activeInfoPopup)
					continue;

				count++;
			}

			return count;
		}
	}

	public static GlobalOverlay Get() {
		var sceneTree = Engine.GetMainLoop() as SceneTree;
		return sceneTree?.Root?.GetNodeOrNull<GlobalOverlay>("/root/GlobalOverlay");
	}

	public override void _Ready() {
		EnsurePopupBlurBackdrop();
	}

	public void AddOverlay(PackedScene overlayScene) {
		if (overlayScene == null) {
			GD.PushError("Overlay scene is null, cannot add.");
			return;
		}

		AddChild(overlayScene.Instantiate());
	}

	public void ShowBlurredPopup(string title, string richText, Texture2D image = null) {
		if (InfoPopupPanelScene == null)
			return;

		EnsurePopupBlurBackdrop();
		CloseInfoPopup();
		MoveChild(_popupBlurBackdrop, GetChildCount() - 1);

		_activeInfoPopup = InfoPopupPanelScene.Instantiate<InfoPopupPanel>();
		_popupBlurBackdrop.Visible = true;
		AddChild(_activeInfoPopup);
		_activeInfoPopup.ShowContent(title, richText, image);
		_activeInfoPopup.Closed += OnInfoPopupClosed;
	}

	public void ShowDesktopGameplayNotice() {
		ShowBlurredPopup(
			"Mobile-first gameplay",
			"This game is mainly built for phone gameplay. It still runs on desktop, but the controls and interface are designed around touch and portrait-style play."
		);
	}

	public void CloseTopOverlay() {
		if (GodotObject.IsInstanceValid(_activeInfoPopup)) {
			CloseInfoPopup();
			return;
		}

		for (var i = GetChildCount() - 1; i >= 0; i--) {
			var child = GetChild(i);
			if (child == _popupBlurBackdrop)
				continue;

			child.QueueFree();
			return;
		}
	}

	public void CloseAllOverlays() {
		CloseInfoPopup();

		foreach (Node child in GetChildren()) {
			if (child == _popupBlurBackdrop)
				continue;

			child.QueueFree();
		}
	}

	public Error ChangeRootScene(PackedScene scene) {
		if (scene == null) {
			GD.PushError("Scene is null, cannot change root scene.");
			return Error.InvalidParameter;
		}

		CloseAllOverlays();

		var sceneTree = Engine.GetMainLoop() as SceneTree;
		if (sceneTree == null) {
			GD.PushError("SceneTree is unavailable, cannot change root scene.");
			return Error.Unavailable;
		}

		return sceneTree.ChangeSceneToPacked(scene);
	}

	private void EnsurePopupBlurBackdrop() {
		if (GodotObject.IsInstanceValid(_popupBlurBackdrop))
			return;

		_popupBlurBackdrop = new ColorRect {
			Name = PopupBlurBackdropName,
			MouseFilter = Control.MouseFilterEnum.Stop,
			Color = Colors.White,
			Visible = false
		};
		_popupBlurBackdrop.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		_popupBlurBackdrop.OffsetLeft = 0.0f;
		_popupBlurBackdrop.OffsetTop = 0.0f;
		_popupBlurBackdrop.OffsetRight = 0.0f;
		_popupBlurBackdrop.OffsetBottom = 0.0f;

		if (BlurShader != null) {
			_popupBlurBackdrop.Material = new ShaderMaterial {
				Shader = BlurShader
			};
		}

		AddChild(_popupBlurBackdrop);
		MoveChild(_popupBlurBackdrop, 0);
	}

	private void OnInfoPopupClosed() {
		CloseInfoPopup();
	}

	private void CloseInfoPopup() {
		if (GodotObject.IsInstanceValid(_activeInfoPopup)) {
			_activeInfoPopup.Closed -= OnInfoPopupClosed;
			_activeInfoPopup.QueueFree();
			_activeInfoPopup = null;
		}

		if (GodotObject.IsInstanceValid(_popupBlurBackdrop))
			_popupBlurBackdrop.Visible = false;
	}
}
