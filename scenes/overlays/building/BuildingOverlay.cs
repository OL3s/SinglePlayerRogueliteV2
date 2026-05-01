using Godot;

public partial class BuildingOverlay : Control {
	public void Update(BuildingData buildingData) {
		if (buildingData == null) {
			GD.PushWarning($"{nameof(BuildingOverlay)} received no building data.");
			return;
		}

		buildingData.Generate(this);
	}
}
