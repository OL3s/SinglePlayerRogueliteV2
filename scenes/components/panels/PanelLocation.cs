using Godot;
using SaveData;

public partial class PanelLocation : Control {
	private Label _labelLocation;
	private Label _labelBiome;

	public override void _Ready() {
		_labelBiome = GetNodeOrNull<Label>("Labels/Panel/LablLocation");
		_labelLocation = GetNodeOrNull<Label>("Labels/LablBiome");
		Update();
	}

	public void Update() {
		var runData = SaveNode.Get().RunData;

		if (_labelLocation != null)
			_labelLocation.Text = runData.CurrentLocation.ToString();

		if (_labelBiome != null)
			_labelBiome.Text = FormatBiomeName(runData);
	}

	private static string FormatBiomeName(RunData runData) {
		var biomeName = runData.CurrentBiome.ToString();
		if (biomeName.Length > 1)
			return biomeName[..^1];

		return biomeName;
	}
}
