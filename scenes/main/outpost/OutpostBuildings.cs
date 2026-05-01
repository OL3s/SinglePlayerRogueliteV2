using Godot;
using Godot.Collections;

public partial class OutpostBuildings : Node2D {
	private const int MinGeneratedBuildings = 0;
	private static readonly string[] PossibleBuildingPaths = {
		"res://core/buildings/data/blacksmith.tres",
		"res://core/buildings/data/jeweler.tres",
		"res://core/buildings/data/general_goods.tres"
	};

	[Export] public PackedScene BuildingTemplateScene { get; set; }

	public override void _Ready() {
		var saveNode = SaveNode.Get();
		var buildings = saveNode.RunData.OutpostBuildings;

		if (buildings == null || buildings.Count != GetChildCount()) {
			buildings = GenerateOutpostBuildings();
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

		for (var slotIndex = 0; slotIndex < buildings.Count; slotIndex++) {
			var buildingData = buildings[slotIndex];
			if (buildingData == null)
				continue;

			var slot = GetSlot(slotIndex);
			if (slot == null)
				break;

			var building = BuildingTemplateScene.Instantiate<BuildingTemplate>();
			building.BuildingData = buildingData;
			slot.AddChild(building);
		}
	}

	private Node2D GetSlot(int index) {
		if (index < 0 || index >= GetChildCount())
			return null;

		return GetChild(index) as Node2D;
	}

	private Array<BuildingData> GenerateOutpostBuildings() {
		var buildings = new Array<BuildingData>();
		var remainingPaths = new Array<string>(PossibleBuildingPaths);
		var remainingSlots = new Array<int>();
		var random = new RandomNumberGenerator();
		random.Randomize();

		for (var slotIndex = 0; slotIndex < GetChildCount(); slotIndex++) {
			buildings.Add(null);
			remainingSlots.Add(slotIndex);
		}

		var maxBuildingCount = Mathf.Min(remainingPaths.Count, remainingSlots.Count);
		var buildingCount = random.RandiRange(MinGeneratedBuildings, maxBuildingCount);

		for (var i = 0; i < buildingCount; i++) {
			var pathIndex = random.RandiRange(0, remainingPaths.Count - 1);
			var buildingPath = remainingPaths[pathIndex];
			remainingPaths.RemoveAt(pathIndex);
			var slotIndex = TakeRandomSlot(remainingSlots, random);

			var buildingData = ResourceLoader.Load<BuildingData>(buildingPath);
			if (buildingData != null)
				buildings[slotIndex] = buildingData;
		}

		return buildings;
	}

	private static int TakeRandomSlot(Array<int> slots, RandomNumberGenerator random) {
		var slotListIndex = random.RandiRange(0, slots.Count - 1);
		var slotIndex = slots[slotListIndex];
		slots.RemoveAt(slotListIndex);
		return slotIndex;
	}
}
