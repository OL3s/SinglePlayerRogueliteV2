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
	[Export] public Array<ItemBase> StorefrontItems { get; set; } = new();

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

			bottomUi.AddChild(CreateButton(buttonData, overlay));
		}
	}

	private Button CreateButton(BuildingOverlayButtonData buttonData, BuildingOverlay buildingOverlay) {
		var button = buttonData.SceneToLoad == null
			? new Button()
			: new ChangeOverlayScene { SceneToLoad = buttonData.SceneToLoad };

		button.CustomMinimumSize = new Vector2(80, 80);
		button.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		button.Text = buttonData.LabelName;
		button.Icon = LoadIcon(buttonData.IconPath);
		button.IconAlignment = HorizontalAlignment.Center;
		button.VerticalIconAlignment = VerticalAlignment.Top;
		button.ExpandIcon = true;
		button.Disabled = buttonData.RequiresCurrentContract && SaveNode.Get()?.RunData?.CurrentContract == null;

		if (buttonData.PathController != null)
			button.Pressed += () => OpenPathController(buttonData, buildingOverlay);

		return button;
	}

	private Texture2D LoadIcon(string iconPath) {
		if (string.IsNullOrWhiteSpace(iconPath))
			return _fallbackIcon;

		return ResourceLoader.Load<Texture2D>(iconPath) ?? _fallbackIcon;
	}

	private void OpenPathController(BuildingOverlayButtonData buttonData, BuildingOverlay buildingOverlay) {
		if (buttonData.PathController == null) {
			GD.PushWarning($"Building overlay button '{buttonData.LabelName}' has no path controller configured.");
			return;
		}

		var overlay = buttonData.PathController.Instantiate();
		if (overlay is StorefrontOverlay storefrontOverlay)
			storefrontOverlay.Update(this);

		overlay.TreeExited += () => Generate(buildingOverlay);

		GlobalOverlay.Get()?.AddChild(overlay);
	}

	public override string ToString() {
		return $"BuildingData(Name={LabelName}, Description={Description}, Buttons={FormatButtons(OverlayButtons)}, StorefrontItems={FormatItems(StorefrontItems)})";
	}

	private static string FormatButtons(Array<BuildingOverlayButtonData> buttons) {
		if (buttons == null || buttons.Count == 0)
			return "[]";

		var result = "[";
		for (var i = 0; i < buttons.Count; i++) {
			if (i > 0)
				result += ", ";

			result += buttons[i]?.LabelName ?? "None";
		}

		return result + "]";
	}

	private static string FormatItems(Array<ItemBase> items) {
		if (items == null || items.Count == 0)
			return "[]";

		var result = "[";
		for (var i = 0; i < items.Count; i++) {
			if (i > 0)
				result += ", ";

			result += items[i]?.ItemName ?? "None";
		}

		return result + "]";
	}
}
