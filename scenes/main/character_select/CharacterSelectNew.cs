using Godot;

public partial class CharacterSelectNew : Control {
	private Label _chooseStartingCharacterLabel;

	public override void _Ready() {
		_chooseStartingCharacterLabel = GetNodeOrNull<Label>("ChooseStartingCharacterLabel");

		var startCharacters = SaveNode.Get().StartCharacters;
		SetCharacter("HBoxContainer/PanelStats/NewCharacterComponent", startCharacters, 0);
		SetCharacter("HBoxContainer/PanelGear/NewCharacterComponent", startCharacters, 1);
		SetCharacter("HBoxContainer/PanelButton/NewCharacterComponent", startCharacters, 2);
		UpdateFirstRunTips();
	}

	private void SetCharacter(string path, PlayerData[] startCharacters, int index) {
		if (index >= startCharacters.Length)
			return;

		var component = GetNodeOrNull<NewCharacterComponent>(path);
		component?.SetPlayerData(startCharacters[index]);
	}

	private void UpdateFirstRunTips() {
		var saveNode = SaveNode.Get();
		var showTips = saveNode.SettingsData?.EnableFirstRunTips ?? true;
		const string labelText = "Choose a starting character.";

		if (_chooseStartingCharacterLabel != null) {
			_chooseStartingCharacterLabel.Text = labelText;
			_chooseStartingCharacterLabel.Visible = true;
		}

		if (!showTips)
			return;

		CallDeferred(MethodName.ShowFirstRunTipsDialog);
	}

	private void ShowFirstRunTipsDialog() {
		GlobalOverlay.Get()?.ShowBlurredPopup("Character Select", "Choose a starting character. Green stats can be clicked to read what each stat means.");
	}
}
