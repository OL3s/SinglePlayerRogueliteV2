using Godot;

[GlobalClass]
public partial class BuildingOverlayButtonData : Resource {
	[Export(PropertyHint.File, "*.png,*.svg,*.jpg,*.jpeg,*.webp,*.tres,*.res")]
	public string IconPath { get; set; }

	[Export] public string LabelName { get; set; }
	[Export] public PackedScene PathController { get; set; }
	[Export] public PackedScene SceneToLoad { get; set; }
	[Export] public bool RequiresCurrentContract { get; set; }
}
