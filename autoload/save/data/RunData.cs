using Godot;
using MyTypes;

namespace SaveData
{
	[GlobalClass]
	public partial class RunData : SaveResource
	{
		[Export] public Biomes CurrentBiome { get; set; } = Biomes.GrasslandsA;
		[Export] public Locations CurrentLocation { get; set; } = Locations.Village;
		[Export] public PlayerData PlayerData { get; set; } = null;
		[Export] public StoreItemData StoreData { get; set; } = null;
		[Export] public Contract CurrentContract { get; set; } = null;
		[Export] public InventoryData InventoryData { get; set; } = new InventoryData();
		[Export] public int Gold { get; set; } = 100;
		public override string ToString()
		{
			return $"RunData: CurrentBiome={CurrentBiome} PlayerData={PlayerData}, StoreData={StoreData}, CurrentContract={CurrentContract}, InventoryData={InventoryData}, Gold={Gold}";
		}
	}
}
