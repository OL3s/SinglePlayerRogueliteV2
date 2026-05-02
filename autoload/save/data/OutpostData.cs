using Godot;
using Godot.Collections;


	namespace SaveData {
		[GlobalClass]
		public partial class OutpostData : SaveResource {
			[Export] public Array<BuildingData> Buildings { get; set; }
			[Export] public ContractChooseData ContractChooseData { get; set; }

			public override string ToString() {
				return $"OutpostData:\n  Buildings={FormatBuildings(Buildings)}\n  ContractChooseData={ContractChooseData?.ToString() ?? "None"}";
			}

			private static string FormatBuildings(Array<BuildingData> buildings) {
			if (buildings == null || buildings.Count == 0)
				return "[]";

			var result = "[\n";
			for (var i = 0; i < buildings.Count; i++) {
				if (i > 0)
					result += ",\n";

				result += $"    Slot {i}: {buildings[i]?.ToString() ?? "None"}";
			}

			return result + "\n  ]";
		}
	}
}
