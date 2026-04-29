using Godot;
using System;

[GlobalClass]
public partial class InventoryDetails : VBoxContainer {
	private Label _detailsLabel;
	private Button _equipButton;
	private Button _offhandButton;
	private Button _unequipButton;
	private ItemBase _item;

	public event Action<ItemBase> EquipPressed;
	public event Action<ItemBase> OffhandPressed;
	public event Action<ItemBase> UnequipPressed;

	public override void _Ready() {
		_detailsLabel = GetNode<Label>("DetailsLabel");
		_equipButton = GetNode<Button>("ActionButtons/EquipButton");
		_offhandButton = GetNode<Button>("ActionButtons/OffhandButton");
		_unequipButton = GetNode<Button>("ActionButtons/UnequipButton");
		_equipButton.Pressed += OnEquipPressed;
		_offhandButton.Pressed += OnOffhandPressed;
		_unequipButton.Pressed += OnUnequipPressed;
		ShowMessage("Tap an item to see details.");
	}

	public void ShowItem(ItemBase item, string details, bool canEquip, bool canEquipOffhand, bool canUnequip) {
		_item = item;
		_detailsLabel.Text = details;
		_equipButton.Visible = canEquip;
		_equipButton.Disabled = !canEquip;
		_offhandButton.Visible = canEquipOffhand;
		_offhandButton.Disabled = !canEquipOffhand;
		_unequipButton.Visible = canUnequip;
		_unequipButton.Disabled = !canUnequip;
	}

	public void ShowMessage(string message) {
		_item = null;
		_detailsLabel.Text = message;
		_equipButton.Visible = false;
		_equipButton.Disabled = true;
		_offhandButton.Visible = false;
		_offhandButton.Disabled = true;
		_unequipButton.Visible = false;
		_unequipButton.Disabled = true;
	}

	private void OnEquipPressed() {
		if (_item == null)
			return;

		EquipPressed?.Invoke(_item);
	}

	private void OnOffhandPressed() {
		if (_item == null)
			return;

		OffhandPressed?.Invoke(_item);
	}

	private void OnUnequipPressed() {
		if (_item == null)
			return;

		UnequipPressed?.Invoke(_item);
	}
}
