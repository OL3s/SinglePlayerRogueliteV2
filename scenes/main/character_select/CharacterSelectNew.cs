using Godot;

public partial class CharacterSelectNew : Control {
	public override void _Ready() {
		var startCharacters = SaveNode.Get().StartCharacters;
		SetCharacter("HBoxContainer/PanelStats/NewCharacterComponent", startCharacters, 0);
		SetCharacter("HBoxContainer/PanelGear/NewCharacterComponent", startCharacters, 1);
		SetCharacter("HBoxContainer/PanelButton/NewCharacterComponent", startCharacters, 2);
	}

	private void SetCharacter(string path, PlayerData[] startCharacters, int index) {
		if (index >= startCharacters.Length)
			return;

		var component = GetNodeOrNull<NewCharacterComponent>(path);
		component?.SetPlayerData(startCharacters[index]);
	}
}
