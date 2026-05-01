using Godot;
using SaveData;

public partial class SettingsDataPage : MarginContainer {
	private const string StartMenuScenePath = "res://scenes/main/main_menu/StartMenu.tscn";

	private CheckBox _firstRunTipsToggle;
	private Button _wipeAllDataButton;
	private ConfirmationDialog _wipeConfirmationDialog;

	public override void _Ready() {
		_firstRunTipsToggle = GetNode<CheckBox>("Layout/FirstRunTipsToggle");
		_wipeAllDataButton = GetNode<Button>("Layout/WipeAllDataButton");
		_wipeConfirmationDialog = GetNode<ConfirmationDialog>("WipeConfirmationDialog");

		_firstRunTipsToggle.Toggled += OnFirstRunTipsToggled;
		_wipeAllDataButton.Pressed += OnWipeAllDataPressed;
		_wipeConfirmationDialog.Confirmed += OnWipeAllDataConfirmed;

		LoadSettings();
	}

	private void LoadSettings() {
		var settings = SaveNode.Get().SettingsData ?? new SettingsData();
		_firstRunTipsToggle.SetPressedNoSignal(settings.EnableFirstRunTips);
	}

	private void OnFirstRunTipsToggled(bool enabled) {
		var saveNode = SaveNode.Get();
		saveNode.SettingsData ??= new SettingsData();
		saveNode.SettingsData.EnableFirstRunTips = enabled;
		saveNode.SaveSettingsData();
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
