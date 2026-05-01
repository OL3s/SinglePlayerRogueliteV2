#nullable enable

using Godot;
using Godot.Collections;
using MyTypes;

[GlobalClass]
public partial class ItemOutpostCompatibilityData : Resource {
	[Export] public Array<OutpostBuildingCompatibility> CompatibleBuildings { get; set; } = new();

	public bool HasCompatibility(OutpostBuildingCompatibility compatibility) {
		return CompatibleBuildings != null && CompatibleBuildings.Contains(compatibility);
	}

	public bool IsCompatibleWith(OutpostBuildingCompatibility compatibility) {
		return CompatibleBuildings == null || CompatibleBuildings.Count == 0 || CompatibleBuildings.Contains(compatibility);
	}
}
