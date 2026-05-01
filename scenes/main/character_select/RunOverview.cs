using Godot;

public partial class RunOverview : Control {
	private const string OutpostScenePath = "res://scenes/main/outpost/Outpost.tscn";
	private const string NewCharacterScenePath = "res://scenes/main/character_select/characterSelectNew.tscn";
	private const string MainMenuScenePath = "res://scenes/main/main_menu/StartMenu.tscn";

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
