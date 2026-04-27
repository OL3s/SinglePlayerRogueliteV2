using Godot;

public partial class CharacterSelection : Control {
	private const string OutpostScenePath = "res://scenes/outpost/Outpost.tscn";
	private const string NewCharacterScenePath = "res://scenes/characterSelect/characterSelectNew.tscn";
	private const string MainMenuScenePath = "res://scenes/screens/StartMenu.tscn";

	public override void _Ready() {
		SetButtonScene("HBoxContainer/PanelButton/VBoxContainer/ContinueButton", OutpostScenePath);
		SetButtonScene("HBoxContainer/PanelButton/VBoxContainer/NewRunButton", NewCharacterScenePath);
		SetButtonScene("BackButton", MainMenuScenePath);
	}

	private void SetButtonScene(string buttonPath, string scenePath) {
		var button = GetNodeOrNull<ChangeOverlayScene>(buttonPath);
		if (button == null)
			return;

		button.SceneToLoad = ResourceLoader.Load<PackedScene>(scenePath);
	}
}
