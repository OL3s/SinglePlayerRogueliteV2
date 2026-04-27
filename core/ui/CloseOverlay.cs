using Godot;

/// <summary>
/// Button action that closes a specific node, all global overlays, or the top global overlay.
/// </summary>
[GlobalClass]
public partial class CloseOverlay : OverlayButton {
	/// <summary>
	/// Optional node to free instead of closing overlays from GlobalOverlay. Frees this node and its children.
	/// </summary>
	[Export] public Node CloseTarget { get; set; }

	/// <summary>
	/// If enabled, closes every direct child of GlobalOverlay. If disabled, closes only the top overlay.
	/// </summary>
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
