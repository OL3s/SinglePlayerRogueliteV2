using Godot;

public partial class PanelGold : Control {
	private Label _label;

	public override void _Ready() {
		_label = GetNodeOrNull<Label>("Label");
		SignalHandler.SubscribeGoldAmountChanged(OnGoldAmountChanged);
		Update();
	}

	public override void _ExitTree() {
		SignalHandler.UnsubscribeGoldAmountChanged(OnGoldAmountChanged);
	}

	public void Update() {
		if (_label == null)
			return;

		UpdateGold(SaveNode.Get().RunData.Gold);
	}

	private void OnGoldAmountChanged(int goldAmount) {
		UpdateGold(goldAmount);
	}

	private void UpdateGold(int goldAmount) {
		if (_label == null)
			return;

		_label.Text = goldAmount.ToString();
	}
}
