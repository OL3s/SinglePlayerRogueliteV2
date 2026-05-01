using Godot;
using SaveData;

public partial class SettingsSoundPage : MarginContainer {
	private const string SfxBusName = "SFX";
	private const string MusicBusName = "Music";

	private HSlider _sfxSlider;
	private Label _sfxValue;
	private CheckBox _sfxMute;
	private HSlider _musicSlider;
	private Label _musicValue;
	private CheckBox _musicMute;
	private Button _applyButton;

	public override void _Ready() {
		_sfxSlider = GetNode<HSlider>("Layout/Controls/SfxRow/Slider");
		_sfxValue = GetNode<Label>("Layout/Controls/SfxRow/Value");
		_sfxMute = GetNode<CheckBox>("Layout/Controls/SfxRow/Mute");
		_musicSlider = GetNode<HSlider>("Layout/Controls/MusicRow/Slider");
		_musicValue = GetNode<Label>("Layout/Controls/MusicRow/Value");
		_musicMute = GetNode<CheckBox>("Layout/Controls/MusicRow/Mute");
		_applyButton = GetNode<Button>("Layout/ApplyButton");

		_sfxSlider.ValueChanged += _ => OnControlsChanged();
		_sfxMute.Toggled += _ => OnControlsChanged();
		_musicSlider.ValueChanged += _ => OnControlsChanged();
		_musicMute.Toggled += _ => OnControlsChanged();
		_applyButton.Pressed += OnApplyPressed;

		LoadFromSettings();
	}

	private void LoadFromSettings() {
		var settings = SaveNode.Get().SettingsData ?? new SettingsData();

		_sfxSlider.SetValueNoSignal(settings.SfxVolumePercent);
		_sfxMute.SetPressedNoSignal(settings.SfxEnabled);
		_musicSlider.SetValueNoSignal(settings.MusicVolumePercent);
		_musicMute.SetPressedNoSignal(settings.MusicEnabled);

		RefreshUi();
		_applyButton.Disabled = true;
	}

	private void OnControlsChanged() {
		RefreshUi();
		_applyButton.Disabled = false;
	}

	private void RefreshUi() {
		_sfxValue.Text = $"{Mathf.RoundToInt((float)_sfxSlider.Value)}%";
		_musicValue.Text = $"{Mathf.RoundToInt((float)_musicSlider.Value)}%";
	}

	private void OnApplyPressed() {
		var saveNode = SaveNode.Get();
		saveNode.SettingsData ??= new SettingsData();

		saveNode.SettingsData.SfxVolumePercent = (float)_sfxSlider.Value;
		saveNode.SettingsData.SfxEnabled = _sfxMute.ButtonPressed;
		saveNode.SettingsData.MusicVolumePercent = (float)_musicSlider.Value;
		saveNode.SettingsData.MusicEnabled = _musicMute.ButtonPressed;

		saveNode.ApplySettings();
		saveNode.SaveSettingsData();
		_applyButton.Disabled = true;
	}
}
