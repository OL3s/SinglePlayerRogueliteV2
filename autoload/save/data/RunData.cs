using Godot;
using MyTypes;

namespace SaveData {
	[GlobalClass]
	public partial class RunData : SaveResource {
		[Export] public Biomes CurrentBiome { get; set; } = Biomes.GrasslandsA;
		[Export] public Locations CurrentLocation { get; set; } = Locations.Village;
		[Export] public PlayerData PlayerData { get; set; } = new PlayerData();
		[Export] public StoreItemData StoreData { get; set; } = null;
		[Export] public Contract CurrentContract { get; set; } = null;
		[Export] public InventoryData InventoryData { get; set; } = new InventoryData();
		[Export] public int ContractsCompleted { get; set; } = 0;
		[Export] public int Gold { get; set; } = 100;
		public override string ToString() {
			return $"RunData:\n"
				+ $"  CurrentBiome={CurrentBiome}\n"
				+ $"  CurrentLocation={CurrentLocation}\n"
				+ $"  PlayerData={FormatResource(PlayerData)}\n"
				+ $"  StoreData={FormatResource(StoreData)}\n"
				+ $"  CurrentContract={FormatResource(CurrentContract)}\n"
				+ $"  InventoryData={FormatResource(InventoryData)}\n"
				+ $"  ContractsCompleted={ContractsCompleted}\n"
				+ $"  Gold={Gold}";
		}

		private static string FormatResource(Resource resource) {
			return resource?.ToString() ?? "None";
		}
	}
}
