using Godot;
using Godot.Collections;

[GlobalClass]
public partial class BuildingData : Resource {
	private readonly Texture2D _fallbackIcon = new PlaceholderTexture2D { Size = new Vector2I(64, 64) };

	[Export] public string LabelName { get; set; } = "Building";
	[Export(PropertyHint.MultilineText)] public string Description { get; set; } = "";
	[Export] public Texture2D OwnerTexture { get; set; }
	[Export(PropertyHint.MultilineText)] public string OwnerText { get; set; } = "Welcome.";
	[Export] public double OwnerTextRevealSeconds { get; set; } = 1.5;
	[Export] public Texture2D BuildingTexture { get; set; }
	[Export] public Array<BuildingOverlayButtonData> OverlayButtons { get; set; } = new();

	public void Generate(BuildingOverlay overlay) {
		if (overlay == null)
			return;

		var titleLabel = overlay.GetNodeOrNull<Label>("PanelBuilding/HBoxContainer/Labels/Label");
		if (titleLabel != null)
			titleLabel.Text = string.IsNullOrWhiteSpace(LabelName) ? "Building" : LabelName;

		var descriptionLabel = overlay.GetNodeOrNull<Label>("PanelBuilding/HBoxContainer/Labels/Label2");
		if (descriptionLabel != null)
			descriptionLabel.Text = Description;

		var ownerTexture = overlay.GetNodeOrNull<TextureRect>("PanelOverlay/CenterContent/OwnerPortrait");
		if (ownerTexture != null)
			ownerTexture.Texture = OwnerTexture ?? _fallbackIcon;

		var ownerText = overlay.GetNodeOrNull<RichTextLabel>("PanelOverlay/CenterContent/DialoguePanel/MarginContainer/OwnerText");
		if (ownerText != null) {
			ownerText.Text = OwnerText;
			ownerText.VisibleRatio = 0.0f;

			var tween = overlay.CreateTween();
			tween.TweenProperty(ownerText, "visible_ratio", 1.0f, Mathf.Max(0.01, OwnerTextRevealSeconds));
		}

		var bottomUi = overlay.GetNodeOrNull<HBoxContainer>("BottomUI");
		if (bottomUi == null)
			return;

		foreach (var child in bottomUi.GetChildren())
			child.QueueFree();

		foreach (var buttonData in OverlayButtons) {
			if (buttonData == null)
				continue;

			bottomUi.AddChild(CreateButton(buttonData));
		}
	}

	private Button CreateButton(BuildingOverlayButtonData buttonData) {
		var button = new Button {
			CustomMinimumSize = new Vector2(80, 80),
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			Text = buttonData.LabelName,
			Icon = LoadIcon(buttonData.IconPath),
			IconAlignment = HorizontalAlignment.Center,
			VerticalIconAlignment = VerticalAlignment.Top,
			ExpandIcon = true,
		};

		button.Pressed += () => OpenPathController(buttonData);
		return button;
	}

	private Texture2D LoadIcon(string iconPath) {
		if (string.IsNullOrWhiteSpace(iconPath))
			return _fallbackIcon;

		return ResourceLoader.Load<Texture2D>(iconPath) ?? _fallbackIcon;
	}

	private static void OpenPathController(BuildingOverlayButtonData buttonData) {
		if (buttonData.PathController == null) {
			GD.PushWarning($"Building overlay button '{buttonData.LabelName}' has no path controller configured.");
			return;
		}

		GlobalOverlay.Get()?.AddOverlay(buttonData.PathController);
	}
}
