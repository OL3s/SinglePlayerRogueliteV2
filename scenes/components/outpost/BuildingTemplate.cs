using Godot;

public partial class BuildingTemplate : Node2D {
	private const ulong OpenDebounceMs = 250;

	[Export] public PackedScene OverlayScene { get; set; }
	[Export] public Texture2D TextureHouse { get; set; }
	[Export] public Texture2D TextureIcon { get; set; }
	[Export] public string DisplayName { get; set; } = "Building";
	[Export] public Vector2 PromptOffset { get; set; } = new(0, -108);

	private Sprite2D _houseSprite;
	private Control _prompt;
	private TextureRect _promptIcon;
	private Label _promptLabel;
	private Area2D _area;
	private ulong _lastOpenTimeMs;

	public override void _Ready() {
		_houseSprite = GetNodeOrNull<Sprite2D>("SpriteHouse");
		_prompt = GetNodeOrNull<Control>("ClickPanel");
		_promptIcon = GetNodeOrNull<TextureRect>("ClickPanel/Icon");
		_promptLabel = GetNodeOrNull<Label>("ClickPanel/LabelPanel/Label");
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

		if (_promptIcon != null && TextureIcon != null)
			_promptIcon.Texture = TextureIcon;

		if (_promptLabel != null)
			_promptLabel.Text = DisplayName;

		UpdatePromptPosition();
	}

	public override void _Process(double delta) {
		UpdatePromptPosition();
	}

	private void UpdatePromptPosition() {
		if (_prompt == null)
			return;

		_prompt.GlobalPosition = GlobalPosition + PromptOffset - new Vector2(_prompt.Size.X * 0.5f, 0);
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
