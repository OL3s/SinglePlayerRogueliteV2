using Godot;

public partial class BuildingTemplate : Node2D {
	private const ulong OpenDebounceMs = 250;

	[Export] public PackedScene OverlayScene { get; set; }
	[Export] public Texture2D TextureHouse { get; set; }

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
		if (_houseSprite != null && TextureHouse != null)
			_houseSprite.Texture = TextureHouse;
	}

	private void OnAreaInputEvent(Node viewport, InputEvent @event, long shapeIdx) {
		if (!IsPressed(@event))
			return;

		var now = Time.GetTicksMsec();
		if (now - _lastOpenTimeMs < OpenDebounceMs)
			return;

		_lastOpenTimeMs = now;

		if (OverlayScene == null) {
			GD.PushWarning($"{nameof(BuildingTemplate)} on '{Name}' has no overlay scene configured.");
			return;
		}

		var overlay = GlobalOverlay.Get();
		if (overlay == null) {
			GD.PushWarning($"{nameof(BuildingTemplate)} on '{Name}' could not find GlobalOverlay.");
			return;
		}

		overlay.AddOverlay(OverlayScene);
	}

	private static bool IsPressed(InputEvent @event) {
		return @event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true }
			|| @event is InputEventScreenTouch { Pressed: true };
	}
}
