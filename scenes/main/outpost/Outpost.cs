using Godot;

public partial class Outpost : Node {
	public override void _Ready() {
		UpdateFirstRunGuidance();
	}

	private void UpdateFirstRunGuidance() {
		var saveNode = SaveNode.Get();
		var showTips = saveNode.SettingsData?.EnableFirstRunTips ?? true;

		if (!showTips)
			return;

		CallDeferred(MethodName.ShowFirstRunOutpostDialog);
	}

	private void ShowFirstRunOutpostDialog() {
		GlobalOverlay.Get()?.ShowBlurredPopup("Village", "You are in the village. Tap buildings to enter them and see what they offer. The buttons at the bottom open your inventory, character, and codex. Press the contract building to start your adventure.");
	}
}
