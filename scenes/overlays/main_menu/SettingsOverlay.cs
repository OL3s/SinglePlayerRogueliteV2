using Godot;

public partial class SettingsOverlay : Control {
	private Button _soundButton;
	private Button _videoButton;
	private Button _dataButton;
	private Label _categoryTitle;
	private Control _soundPage;
	private Control _videoPage;
	private Control _dataPage;

	public override void _Ready() {
		_soundButton = GetNode<Button>("Content/CategoryPanel/MarginContainer/CategoryLayout/CategoryButtons/SoundButton");
		_videoButton = GetNode<Button>("Content/CategoryPanel/MarginContainer/CategoryLayout/CategoryButtons/VideoButton");
		_dataButton = GetNode<Button>("Content/CategoryPanel/MarginContainer/CategoryLayout/CategoryButtons/DataButton");
		_categoryTitle = GetNode<Label>("Content/RightPanel/MarginContainer/RightLayout/CategoryTitle");
		_soundPage = GetNode<Control>("Content/RightPanel/MarginContainer/RightLayout/Pages/SoundPage");
		_videoPage = GetNode<Control>("Content/RightPanel/MarginContainer/RightLayout/Pages/VideoPage");
		_dataPage = GetNode<Control>("Content/RightPanel/MarginContainer/RightLayout/Pages/DataPage");

		_soundButton.Pressed += () => SelectPage("Sound");
		_videoButton.Pressed += () => SelectPage("Video");
		_dataButton.Pressed += () => SelectPage("Data");

		SelectPage("Sound");
	}

	private void SelectPage(string pageName) {
		_categoryTitle.Text = pageName;
		_soundPage.Visible = pageName == "Sound";
		_videoPage.Visible = pageName == "Video";
		_dataPage.Visible = pageName == "Data";
	}
}
