using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CharacterNameData : Resource {
	[Export] public Dictionary<int, string> Names { get; set; } = new() {
		{ 0, "Aren" },
		{ 1, "Bryn" },
		{ 2, "Cora" },
		{ 3, "Dain" },
		{ 4, "Eira" },
		{ 5, "Finn" },
		{ 6, "Galen" },
		{ 7, "Hale" },
		{ 8, "Iris" },
		{ 9, "Jory" },
		{ 10, "Kara" },
		{ 11, "Lio" },
		{ 12, "Mira" },
		{ 13, "Nox" },
		{ 14, "Orin" },
		{ 15, "Pax" },
		{ 16, "Quinn" },
		{ 17, "Rhea" },
		{ 18, "Soren" },
		{ 19, "Talia" },
		{ 20, "Ulric" },
		{ 21, "Vera" },
		{ 22, "Wren" },
		{ 23, "Xara" },
		{ 24, "Yara" },
		{ 25, "Zane" }
	};

	public string GetRandomName(RandomNumberGenerator random) {
		if (Names.Count == 0)
			return "Player";

		var index = random.RandiRange(0, Names.Count - 1);
		return Names.TryGetValue(index, out var name) && !string.IsNullOrWhiteSpace(name) ? name : "Player";
	}
}
