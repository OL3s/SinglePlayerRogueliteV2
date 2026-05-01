using Godot;

public partial class EquipmentPanel : PanelContainer {
	[Signal]
	public delegate void EquipmentSlotPressedEventHandler(int slotIndex, ItemBase item, string slotName);

	private bool _enableButtonPresses = true;
	private bool _skillDescriptionIsClickable;
	private bool _useSaveNodeData = true;
	private EquipedItemsData _equipedItems;
	private PanelStats _panelStats;

	[Export]
	public bool EnableButtonPresses {
		get => _enableButtonPresses;
		set {
			_enableButtonPresses = value;
			ApplyInteractionState();
		}
	}

	[Export]
	public bool SkillDescriptionIsClickable {
		get => _skillDescriptionIsClickable;
		set {
			_skillDescriptionIsClickable = value;
			ApplyStatsInteractionState();
		}
	}

	[Export]
	public bool UseSaveNodeData {
		get => _useSaveNodeData;
		set {
			_useSaveNodeData = value;
			Render();
		}
	}

	[Export]
	public EquipedItemsData EquipedItems {
		get => _equipedItems;
		set {
			_equipedItems = value;
			Render();
		}
	}

	private static readonly string[] EquipmentSlotNames = {
		"Main Hand",
		"Off Hand",
		"Armor",
		"Amulet",
		"Ammo",
		"Consumable"
	};

	private Button[] _equipmentButtons;
	private PlaceholderTexture2D _placeholderIcon = new() { Size = new Vector2I(64, 64) };

	public override void _Ready() {
		_panelStats = GetNodeOrNull<PanelStats>("MarginContainer/EquipmentLayout/PanelStats");
		_equipmentButtons = new[] {
			GetNode<Button>("MarginContainer/EquipmentLayout/EquipmentBody/LeftSlots/Slot1"),
			GetNode<Button>("MarginContainer/EquipmentLayout/EquipmentBody/LeftSlots/Slot2"),
			GetNode<Button>("MarginContainer/EquipmentLayout/EquipmentBody/LeftSlots/Slot3"),
			GetNode<Button>("MarginContainer/EquipmentLayout/EquipmentBody/RightSlots/Slot4"),
			GetNode<Button>("MarginContainer/EquipmentLayout/EquipmentBody/RightSlots/Slot5"),
			GetNode<Button>("MarginContainer/EquipmentLayout/EquipmentBody/RightSlots/Slot6")
		};

		for (var i = 0; i < _equipmentButtons.Length; i++) {
			var slotIndex = i;
			_equipmentButtons[i].Pressed += () => OnEquipmentSlotPressed(slotIndex);
		}

		ApplyInteractionState();
		ApplyStatsInteractionState();
		Render();
	}

	public void Render() {
		if (_equipmentButtons == null)
			return;

		var equippedItems = GetEquippedItems();

		for (var i = 0; i < _equipmentButtons.Length; i++) {
			var slotName = EquipmentSlotNames[i];
			var item = equippedItems[i];
			var text = item?.ItemName ?? "Empty";
			_equipmentButtons[i].Text = $"{slotName}\n{text}";
			_equipmentButtons[i].Icon = GetItemIcon(item);
		}
	}

	private void ApplyInteractionState() {
		if (_equipmentButtons == null)
			return;

		foreach (var button in _equipmentButtons) {
			button.FocusMode = EnableButtonPresses ? FocusModeEnum.All : FocusModeEnum.None;
			button.MouseFilter = EnableButtonPresses ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
		}
	}

	private void ApplyStatsInteractionState() {
		if (_panelStats == null)
			return;

		_panelStats.SkillDescriptionIsClickable = SkillDescriptionIsClickable;
	}

	private void OnEquipmentSlotPressed(int slotIndex) {
		if (!EnableButtonPresses)
			return;

		EmitSignal(SignalName.EquipmentSlotPressed, slotIndex, GetEquippedItems()[slotIndex], EquipmentSlotNames[slotIndex]);
	}

	private ItemBase[] GetEquippedItems() {
		var equipped = UseSaveNodeData ? SaveNode.Get()?.EquipedItemsData : _equipedItems;
		return new ItemBase[] {
			equipped?.MainHandItem,
			equipped?.OffHandItem,
			equipped?.ArmorItem,
			equipped?.AmuletItem,
			equipped?.AmmoItem,
			equipped?.ConsumableItem
		};
	}

	private Texture2D GetItemIcon(ItemBase item) {
		if (item is ItemEquipable equipable && equipable.EquippedTexture != null) {
			return equipable.EquippedTexture;
		}

		return item?.Icon ?? _placeholderIcon;
	}
}
