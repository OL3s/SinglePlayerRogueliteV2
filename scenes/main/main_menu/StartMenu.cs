using Godot;

public partial class StartMenu : Control {
	public override void _Notification(int what) {
		if (what == NotificationWMGoBackRequest)
			GetTree().Quit();
	}
}
