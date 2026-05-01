using Godot;
using Godot.Collections;

namespace SaveData {
	[GlobalClass]
	public partial class OutpostData : SaveResource {
		[Export] public Array<BuildingData> Buildings { get; set; }

		public override string ToString() {
			return $"OutpostData: Buildings={Buildings?.Count ?? 0}";
		}
	}
}
