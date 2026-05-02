using Godot;

public partial class EquipmentPanel : PanelContainer {
	[Signal]
	public delegate void EquipmentSlotPressedEventHandler(int slotIndex, ItemBase item, string slotName);

	private bool _enableButtonPresses = true;
	private bool _skillDescriptionIsClickable;
	private bool _disableEmptySlotPresses;
	private bool _hideEmptySlotIcons;
	private bool _useSaveNodeData = true;
	private EquipedItemsData _equipedItems;
	private PanelStats _panelStats;

	/// <summary>
	/// Enables slot button interaction for this panel instance.
	/// Set this to <c>false</c> in read-only views that should only display equipment,
	/// such as run previews, so pressing equipment slots does nothing.
	/// </summary>
	[Export]
	public bool EnableButtonPresses {
		get => _enableButtonPresses;
		set {
			_enableButtonPresses = value;
			ApplyInteractionState();
		}
	}

	/// <summary>
	/// Enables the embedded stat rows to open the shared blurred info popup when clicked.
	/// Set this to <c>true</c> in scenes where the stat labels should act as help buttons,
	/// and leave it <c>false</c> in passive displays.
	/// </summary>
	[Export]
	public bool SkillDescriptionIsClickable {
		get => _skillDescriptionIsClickable;
		set {
			_skillDescriptionIsClickable = value;
			ApplyStatsInteractionState();
		}
	}

	/// <summary>
	/// Disables only the equipment slot buttons that do not currently contain an item.
	/// Use this in views like the inventory overlay where empty slots should stay visible
	/// but should not be pressable.
	/// </summary>
	[Export]
	public bool DisableEmptySlotPresses {
		get => _disableEmptySlotPresses;
		set {
			_disableEmptySlotPresses = value;
			ApplyInteractionState();
		}
	}

	/// <summary>
	/// Hides the placeholder icon for empty equipment slots while keeping icons for equipped items.
	/// Use this when a scene should show only real equipped item art and leave empty slots visually blank.
	/// </summary>
	[Export]
	public bool HideEmptySlotIcons {
		get => _hideEmptySlotIcons;
		set {
			_hideEmptySlotIcons = value;
			Render();
		}
	}

	/// <summary>
	/// Chooses whether this panel reads equipped items from the global save data or from the exported
	/// <see cref="EquipedItems"/> value below. Keep this enabled for the live player inventory/equipment view,
	/// and disable it for preview scenes that inject their own equipment data.
	/// </summary>
	[Export]
	public bool UseSaveNodeData {
		get => _useSaveNodeData;
		set {
			_useSaveNodeData = value;
			Render();
		}
	}

	/// <summary>
	/// Provides explicit equipment data for this panel instance when <see cref="UseSaveNodeData"/> is disabled.
	/// This is intended for reusable preview screens that should not read from the player's active save.
	/// </summary>
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
			_equipmentButtons[i].Disabled = DisableEmptySlotPresses && item == null;
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
		if (item == null && HideEmptySlotIcons)
			return null;

		if (item is ItemEquipable equipable && equipable.EquippedTexture != null) {
			return equipable.EquippedTexture;
		}

		return item?.Icon ?? _placeholderIcon;
	}
}
