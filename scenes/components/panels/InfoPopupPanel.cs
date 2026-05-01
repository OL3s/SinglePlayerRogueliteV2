using Godot;

public partial class InfoPopupPanel : Control {
	private const float PanelWidthRatio = 0.6f;
	private const int PanelMinWidth = 360;
	private const int PanelMaxWidth = 760;

	[Signal]
	public delegate void ClosedEventHandler();

	private PanelContainer _panel;
	private Label _titleLabel;
	private TextureRect _image;
	private RichTextLabel _bodyText;
	private Button _okButton;

	public override void _Ready() {
		EnsureNodes();
		UpdatePanelWidth();
	}

	private void EnsureNodes() {
		if (_panel != null)
			return;

		_panel = GetNode<PanelContainer>("CenterContainer/Panel");
		_titleLabel = GetNode<Label>("CenterContainer/Panel/MarginContainer/Layout/Title");
		_image = GetNode<TextureRect>("CenterContainer/Panel/MarginContainer/Layout/Image");
		_bodyText = GetNode<RichTextLabel>("CenterContainer/Panel/MarginContainer/Layout/BodyText");
		_okButton = GetNode<Button>("CenterContainer/Panel/MarginContainer/Layout/OkButton");

		_okButton.Pressed += OnOkPressed;
	}

	public override void _Notification(int what) {
		if (what == NotificationResized)
			UpdatePanelWidth();
	}

	public void ShowContent(string title, string richText, Texture2D image = null) {
		EnsureNodes();
		_titleLabel.Text = string.IsNullOrWhiteSpace(title) ? "Info" : title;
		_bodyText.Text = richText ?? string.Empty;
		_image.Texture = image;
		_image.Visible = image != null;
		UpdatePanelWidth();
	}

	private void UpdatePanelWidth() {
		if (_panel == null)
			return;

		var viewportWidth = GetViewportRect().Size.X;
		var width = Mathf.Clamp(Mathf.RoundToInt(viewportWidth * PanelWidthRatio), PanelMinWidth, PanelMaxWidth);
		_panel.CustomMinimumSize = new Vector2(width, 0.0f);
	}

	private void OnOkPressed() {
		EmitSignal(SignalName.Closed);
		QueueFree();
	}
}
