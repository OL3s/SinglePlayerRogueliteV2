using Godot;

namespace SaveData {
	[GlobalClass]
	public partial class MetaData : SaveResource {
		[ExportGroup("Gem Collection")]
		[Export] public bool GemRedCollected { get; set; } = false;
		[Export] public bool GemGreenCollected { get; set; } = false;
		[Export] public bool GemBlueCollected { get; set; } = false;
		[ExportGroup("Run Data")]
		[Export] public int RunCount { get; set; } = 0;
		[Export] public bool HasSeenOutpostIntro { get; set; } = false;
		public bool IsFirstTimePlayer => RunCount == 0;
		public override string ToString() {
			return $"MetaData: RunCount={RunCount}, IsFirstTimePlayer={IsFirstTimePlayer}, HasSeenOutpostIntro={HasSeenOutpostIntro}, CollectedGems=[Red={GemRedCollected}, Green={GemGreenCollected}, Blue={GemBlueCollected}]";
		}
	}

	public enum Gem {
		Red = 0,
		Green = 1,
		Blue = 2,
	}
}
