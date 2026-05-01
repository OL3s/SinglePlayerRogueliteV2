using Godot;

public partial class BuildingTemplate : Node2D {
	private const ulong OpenDebounceMs = 250;
	private const string BuildingOverlayPath = "res://scenes/overlays/building/BuildingOverlay.tscn";

	[Export] public PackedScene BuildingOverlayScene { get; set; } = GD.Load<PackedScene>(BuildingOverlayPath);
	[Export] public BuildingData BuildingData { get; set; } = new();

	private Sprite2D _houseSprite;
	private Area2D _area;
	private ulong _lastOpenTimeMs;

	public override void _Ready() {
		_houseSprite = GetNodeOrNull<Sprite2D>("SpriteHouse");
		_area = GetNodeOrNull<Area2D>("Area2D");

		ApplyExportedValues();

		if (_area == null) {
			GD.PushWarning($"{nameof(BuildingTemplate)} on '{Name}' could not find Area2D child.");
			return;
		}

		_area.InputEvent += OnAreaInputEvent;
	}

	private void ApplyExportedValues() {
		if (_houseSprite != null && BuildingData?.BuildingTexture != null)
			_houseSprite.Texture = BuildingData.BuildingTexture;
	}

	private void OnAreaInputEvent(Node viewport, InputEvent @event, long shapeIdx) {
		if (!IsPressed(@event))
			return;

		var now = Time.GetTicksMsec();
		if (now - _lastOpenTimeMs < OpenDebounceMs)
			return;

		_lastOpenTimeMs = now;

		if (BuildingOverlayScene == null) {
			GD.PushWarning($"{nameof(BuildingTemplate)} on '{Name}' could not load BuildingOverlay.");
			return;
		}

		var overlay = GlobalOverlay.Get();
		if (overlay == null) {
			GD.PushWarning($"{nameof(BuildingTemplate)} on '{Name}' could not find GlobalOverlay.");
			return;
		}

		var overlayInstance = BuildingOverlayScene.Instantiate();
		if (overlayInstance is BuildingOverlay buildingOverlay)
			buildingOverlay.Update(BuildingData);

		overlay.AddChild(overlayInstance);
	}

	private static bool IsPressed(InputEvent @event) {
		return @event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true }
			|| @event is InputEventScreenTouch { Pressed: true };
	}
}
