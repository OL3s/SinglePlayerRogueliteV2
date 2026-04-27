using Godot;
using SaveData;

public partial class PanelLocation : Control {
	private Label _labelBiome;
	private Label _labelLocationWave;

	public override void _Ready() {
		_labelBiome = GetNodeOrNull<Label>("Labels/Panel/LablBiome");
		_labelLocationWave = GetNodeOrNull<Label>("Labels/LablLocationWave");
		Update();
	}

	public void Update() {
		var runData = SaveNode.Get().RunData;

		if (_labelBiome != null)
			_labelBiome.Text = FormatBiomeName(runData);

		if (_labelLocationWave != null)
			_labelLocationWave.Text = $"{runData.CurrentLocation} : W{runData.ContractsCompleted + 1}";
	}

	private static string FormatBiomeName(RunData runData) {
		var biomeName = runData.CurrentBiome.ToString();
		if (biomeName.Length > 1)
			return biomeName[..^1];

		return biomeName;
	}
}
