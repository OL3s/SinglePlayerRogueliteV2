using Godot;
using SaveData;

public partial class ContractSelectOverlay : Control {
	private VBoxContainer _contractList;
	private Label _emptyLabel;
	private Label _selectedTitle;
	private Label _selectedDetails;
	private Button _chooseButton;
	private readonly PlaceholderTexture2D _placeholderIcon = new() { Size = new Vector2I(64, 64) };
	private ContractChooseData _contractChooseData;
	private Contract _selectedContract;

	public override void _Ready() {
		_contractList = GetNode<VBoxContainer>("Content/ContractPanel/MarginContainer/ContractLayout/ContractList");
		_emptyLabel = GetNode<Label>("Content/ContractPanel/MarginContainer/ContractLayout/EmptyLabel");
		_selectedTitle = GetNode<Label>("Content/PreviewPanel/MarginContainer/PreviewLayout/Title");
		_selectedDetails = GetNode<Label>("Content/PreviewPanel/MarginContainer/PreviewLayout/PreviewDetails");
		_chooseButton = GetNode<Button>("Content/PreviewPanel/MarginContainer/PreviewLayout/ChooseButton");

		_chooseButton.Pressed += OnChoosePressed;
		LoadContracts();
		Render();
	}

	private void LoadContracts() {
		var saveNode = SaveNode.Get();
		if (saveNode?.RunData == null)
			return;

		saveNode.RunData.OutpostData ??= new OutpostData();
		saveNode.RunData.OutpostData.ContractChooseData ??= new ContractChooseData();
		_contractChooseData = saveNode.RunData.OutpostData.ContractChooseData;

		if (_contractChooseData.Contracts == null || _contractChooseData.Contracts.Count != 3) {
			_contractChooseData.GenerateContracts(saveNode.RunData.CurrentBiome, saveNode.RunData.CurrentBiomeContractsCompleted);
			saveNode.SaveRunData();
		}
	}

	private void Render() {
		RenderContracts();
		RenderPreview();
	}

	private void RenderContracts() {
		foreach (var child in _contractList.GetChildren())
			child.QueueFree();

		var contracts = _contractChooseData?.Contracts;
		var hasContracts = contracts != null && contracts.Count > 0;
		_contractList.Visible = hasContracts;
		_emptyLabel.Visible = !hasContracts;

		if (!hasContracts)
			return;

		foreach (var contract in contracts) {
			if (contract == null)
				continue;

			_contractList.AddChild(CreateContractButton(contract));
		}

		_selectedContract ??= contracts[0];
	}

	private Button CreateContractButton(Contract contract) {
		var button = new Button {
			CustomMinimumSize = new Vector2(0, 84),
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			Text = FormatContractTitle(contract),
			Icon = _placeholderIcon,
			TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis,
			Alignment = HorizontalAlignment.Left
		};

		button.Pressed += () => SelectContract(contract);
		return button;
	}

	private void SelectContract(Contract contract) {
		_selectedContract = contract;
		RenderPreview();
	}

	private void RenderPreview() {
		if (_selectedContract == null) {
			_selectedTitle.Text = "Choose Path";
			_selectedDetails.Text = "Select one of the available paths to continue the run.";
			_chooseButton.Disabled = true;
			return;
		}

		_selectedTitle.Text = FormatContractTitle(_selectedContract);
		_selectedDetails.Text = $"Biome: {FormatBiomeName(_selectedContract.Biome)}\n\nAccepting this path will set the next path for the current run.";
		_chooseButton.Disabled = false;
	}

	private void OnChoosePressed() {
		if (_selectedContract == null)
			return;

		var saveNode = SaveNode.Get();
		if (saveNode?.RunData == null)
			return;

		saveNode.RunData.CurrentContract = _selectedContract;
		if (saveNode.RunData.OutpostData?.ContractChooseData != null)
			saveNode.RunData.OutpostData.ContractChooseData.Contracts.Clear();

		saveNode.SaveRunData();
		QueueFree();
	}

	private static string FormatContractTitle(Contract contract) {
		return FormatBiomeName(contract.Biome);
	}

	private static string FormatBiomeName(MyTypes.Biomes biome) {
		var biomeName = biome.ToString();
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
