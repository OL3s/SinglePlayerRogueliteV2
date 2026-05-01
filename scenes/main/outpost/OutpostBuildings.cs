using Godot;
using Godot.Collections;

public partial class OutpostBuildings : Node2D {
	private const int MinGeneratedBuildings = 1;
	private const int MaxGeneratedBuildings = 3;
	private static readonly string[] PossibleBuildingPaths = {
		"res://core/buildings/data/blacksmith.tres",
		"res://core/buildings/data/jeweler.tres",
		"res://core/buildings/data/general_goods.tres"
	};

	[Export] public PackedScene BuildingTemplateScene { get; set; }

	public override void _Ready() {
		var saveNode = SaveNode.Get();
		var buildings = saveNode.RunData.OutpostBuildings;

		if (buildings == null) {
			buildings = GenerateMockBuildings();
			saveNode.RunData.OutpostBuildings = buildings;
			saveNode.SaveRunData();
		}

		GenerateBuildings(buildings);
	}

	private void GenerateBuildings(Array<BuildingData> buildings) {
		if (BuildingTemplateScene == null) {
			GD.PushWarning($"{nameof(OutpostBuildings)} on '{Name}' has no building template configured.");
			return;
		}

		var slotIndex = 0;
		foreach (var buildingData in buildings) {
			if (buildingData == null)
				continue;

			var slot = GetSlot(slotIndex);
			if (slot == null)
				break;

			var building = BuildingTemplateScene.Instantiate<BuildingTemplate>();
			building.BuildingData = buildingData;
			slot.AddChild(building);
			slotIndex++;
		}
	}

	private Node2D GetSlot(int index) {
		if (index < 0 || index >= GetChildCount())
			return null;

		return GetChild(index) as Node2D;
	}

	private static Array<BuildingData> GenerateMockBuildings() {
		var buildings = new Array<BuildingData>();
		var remainingPaths = new Array<string>(PossibleBuildingPaths);
		var random = new RandomNumberGenerator();
		random.Randomize();
		var buildingCount = random.RandiRange(MinGeneratedBuildings, Mathf.Min(MaxGeneratedBuildings, remainingPaths.Count));

		for (var i = 0; i < buildingCount; i++) {
			var pathIndex = random.RandiRange(0, remainingPaths.Count - 1);
			var buildingPath = remainingPaths[pathIndex];
			remainingPaths.RemoveAt(pathIndex);

			var buildingData = ResourceLoader.Load<BuildingData>(buildingPath);
			if (buildingData != null)
				buildings.Add(buildingData);
		}

		return buildings;
	}
}
