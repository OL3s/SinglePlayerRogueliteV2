using Godot;

[GlobalClass]
public partial class CloseOverlay : OverlayButton {
	[Export] public Node CloseTarget { get; set; }
	[Export] public bool CloseAllOverlayChildren { get; set; }

	protected override void HandleOverlayAction() {
		if (CloseTarget != null) {
			CloseTarget.QueueFree();
			return;
		}

		var overlay = GlobalOverlay.Get();
		if (overlay == null) return;

		if (CloseAllOverlayChildren) {
			overlay.CloseAllOverlays();
			return;
		}

		overlay.CloseTopOverlay();
	}
}
