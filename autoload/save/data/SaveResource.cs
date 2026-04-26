using Godot;

namespace SaveData {
	public abstract partial class SaveResource : Resource {
		[Export] public int DataVersion { get; set; } = 1;
	}
}
