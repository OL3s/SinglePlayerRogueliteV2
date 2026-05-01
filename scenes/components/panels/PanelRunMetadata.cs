using Godot;
using SaveData;

public partial class PanelRunMetadata : Control {
	private Label _labelPlayerName;
	private Label _labelBiome;
	private Label _labelLocation;
	private Label _labelCurrentLevel;
	private Label _labelContractsCompleted;
	private Label _labelCurrentContract;

	public override void _Ready() {
		_labelPlayerName = GetNodeOrNull<Label>("Content/Rows/PlayerNameRow/MarginContainer/Entry/Value");
		_labelBiome = GetNodeOrNull<Label>("Content/Rows/BiomeRow/MarginContainer/Entry/Value");
		_labelLocation = GetNodeOrNull<Label>("Content/Rows/LocationRow/MarginContainer/Entry/Value");
		_labelCurrentLevel = GetNodeOrNull<Label>("Content/Rows/CurrentLevelRow/MarginContainer/Entry/Value");
		_labelContractsCompleted = GetNodeOrNull<Label>("Content/Rows/ContractsCompletedRow/MarginContainer/Entry/Value");
		_labelCurrentContract = GetNodeOrNull<Label>("Content/Rows/CurrentContractRow/MarginContainer/Entry/Value");
		Update();
	}

	public void Update() {
		var runData = SaveNode.Get()?.RunData;
		var playerName = runData?.PlayerData?.PlayerName;

		SetLabelText(_labelPlayerName, string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName);
		SetLabelText(_labelBiome, runData == null ? "Unknown" : FormatBiomeName(runData));
		SetLabelText(_labelLocation, runData == null ? "Unknown" : FormatEnumName(runData.CurrentLocation.ToString()));
		SetLabelText(_labelCurrentLevel, runData?.PlayerData?.GetCurrentRunLevel().ToString() ?? "0");
		SetLabelText(_labelContractsCompleted, runData?.ContractsCompleted.ToString() ?? "0");
		SetLabelText(_labelCurrentContract, FormatCurrentContract(runData?.CurrentContract));
	}

	private static void SetLabelText(Label label, string text) {
		if (label != null)
			label.Text = text;
	}

	private static string FormatCurrentContract(Contract contract) {
		if (contract == null)
			return "None";

		return $"{FormatBiomeName(contract.Biome.ToString())} -> {FormatEnumName(contract.EndLocation.ToString())}";
	}

	private static string FormatBiomeName(RunData runData) {
		return FormatBiomeName(runData.CurrentBiome.ToString());
	}

	private static string FormatBiomeName(string biomeName) {
		if (biomeName.Length > 1 && char.IsLetter(biomeName[^1]) && char.IsUpper(biomeName[^1]))
			biomeName = biomeName[..^1];

		return FormatEnumName(biomeName);
	}

	private static string FormatEnumName(string value) {
		if (string.IsNullOrWhiteSpace(value))
			return "Unknown";

		var result = value[0].ToString();
		for (var i = 1; i < value.Length; i++) {
			if (char.IsUpper(value[i]) && !char.IsWhiteSpace(value[i - 1]))
				result += " ";

			result += value[i];
		}

		return result;
	}
}
