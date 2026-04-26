using Godot;

namespace SaveData {
	[GlobalClass]
	public partial class SettingsData : SaveResource {
		public override string ToString() {
			return $"SettingsData: DataVersion={DataVersion}";
		}
	}
}
