using Godot;
using Godot.Collections;

public partial class OutpostBuildings : Node2D {
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
		var placeholderTexture = new PlaceholderTexture2D { Size = new Vector2I(64, 64) };
		var storefrontScene = GD.Load<PackedScene>("res://scenes/overlays/storefront/StorefrontOverlay.tscn");

		return new Array<BuildingData> {
			new() {
				LabelName = "Blacksmith",
				Description = "Forge, repair, and improve equipment.",
				OwnerText = "Steel remembers every strike. Bring me coin and materials, and I'll make your gear remember victory.",
				BuildingTexture = placeholderTexture,
				OwnerTexture = placeholderTexture,
				OverlayButtons = new Array<BuildingOverlayButtonData> {
					new() {
						IconPath = "",
						LabelName = "Forge"
					},
					new() {
						IconPath = "",
						LabelName = "Shop",
						PathController = storefrontScene
					}
				},
				StorefrontItems = new Array<ItemBase> {
					new ItemBase("Iron Sword", null, placeholderTexture, 1, 100),
					new ItemBase("Repair Kit", null, placeholderTexture, 5, 40),
					new ItemBase("Steel Buckler", null, placeholderTexture, 1, 140)
				}
			}
		};
	}
}
