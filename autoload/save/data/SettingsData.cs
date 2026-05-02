using Godot;

namespace SaveData {
	[GlobalClass]
	public partial class SettingsData : SaveResource {
		[ExportGroup("Sound")]
		[Export(PropertyHint.Range, "0,100,1")]
		public float SfxVolumePercent { get; set; } = 72.0f;

		[Export]
		public bool SfxEnabled { get; set; } = true;

		[Export(PropertyHint.Range, "0,100,1")]
		public float MusicVolumePercent { get; set; } = 84.0f;

		[Export]
		public bool MusicEnabled { get; set; } = true;

		[ExportGroup("Gameplay")]
		[Export]
		public bool EnableFirstRunTips { get; set; } = true;

		public override string ToString() {
			return $"SettingsData: DataVersion={DataVersion}, SfxVolumePercent={SfxVolumePercent}, SfxEnabled={SfxEnabled}, MusicVolumePercent={MusicVolumePercent}, MusicEnabled={MusicEnabled}, EnableFirstRunTips={EnableFirstRunTips}";
		}
	}
}
