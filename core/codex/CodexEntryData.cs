using Godot;

[GlobalClass]
public partial class CodexEntryData : Resource {
	[Export] public string Id { get; set; } = string.Empty;
	[Export] public CodexCategory Category { get; set; }
	[Export] public CodexSubcategory Subcategory { get; set; }
	[Export] public string Title { get; set; } = string.Empty;
	[Export(PropertyHint.MultilineText)] public string Description { get; set; } = string.Empty;
	[Export] public Texture2D Icon { get; set; } = new PlaceholderTexture2D();
}
