using Godot;

public partial class SettingsDataPage : MarginContainer {
	private const string StartMenuScenePath = "res://scenes/main/main_menu/StartMenu.tscn";

	private Button _wipeAllDataButton;
	private ConfirmationDialog _wipeConfirmationDialog;

	public override void _Ready() {
		_wipeAllDataButton = GetNode<Button>("Layout/WipeAllDataButton");
		_wipeConfirmationDialog = GetNode<ConfirmationDialog>("WipeConfirmationDialog");

		_wipeAllDataButton.Pressed += OnWipeAllDataPressed;
		_wipeConfirmationDialog.Confirmed += OnWipeAllDataConfirmed;
	}

	private void OnWipeAllDataPressed() {
		_wipeConfirmationDialog.PopupCentered();
	}

	private void OnWipeAllDataConfirmed() {
		SaveNode.Get().WipeAllData();
		var startMenuScene = ResourceLoader.Load<PackedScene>(StartMenuScenePath);
		GlobalOverlay.Get()?.ChangeRootScene(startMenuScene);
	}
}
