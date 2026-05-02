using Godot;

public partial class StartMenu : Control {
	public override void _Ready() {
		ShowDesktopGameplayNoticeIfNeeded();
	}

	public override void _Notification(int what) {
		if (what == NotificationWMGoBackRequest)
			GetTree().Quit();
	}

	private void ShowDesktopGameplayNoticeIfNeeded() {
		var showTips = SaveNode.Get().SettingsData?.EnableFirstRunTips ?? true;
		if (!showTips || OS.HasFeature("mobile"))
			return;

		CallDeferred(MethodName.ShowDesktopGameplayNotice);
	}

	private void ShowDesktopGameplayNotice() {
		GlobalOverlay.Get()?.ShowDesktopGameplayNotice();
	}
}
