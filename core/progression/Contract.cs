using Godot;
using MyTypes;

[GlobalClass]
public partial class Contract : Resource {
	[Export] public Biomes Biome { get; set; }
	[Export] public Locations EndLocation { get; set; }
}
