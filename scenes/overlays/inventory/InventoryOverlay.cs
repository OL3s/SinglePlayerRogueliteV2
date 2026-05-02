using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class InventoryOverlay : Control {
	private const int ItemsPerPage = 8;
	private static readonly Type[] ItemFilterTypes = {
		typeof(ItemEquipable),
		typeof(ItemArmor),
		typeof(ItemAmulet),
		typeof(ItemAmmo),
		typeof(ItemConsumable)
	};

	private EquipmentPanel _equipmentPanel;
	private ItemInventory[] _inventoryButtons;
	private HBoxContainer _filterButtons;
	private GridContainer _inventoryGrid;
	private Button _previousPageButton;
	private Button _nextPageButton;
	private Label _pageLabel;
	private InventoryDetails _details;
	private Label _emptyInventoryLabel;
	private PlaceholderTexture2D _placeholderIcon = new() { Size = new Vector2I(64, 64) };
	private List<ItemBase> _items = new();
	private Type _filterType;
	private int _currentPage;

	public override void _Ready() {
		_equipmentPanel = GetNode<EquipmentPanel>("Content/EquipmentPanel");
		_inventoryButtons = Enumerable.Range(1, 8)
			.Select(index => GetNode<ItemInventory>($"Content/InventoryPanel/MarginContainer/InventoryLayout/InventoryGrid/Item{index}"))
			.ToArray();

		_filterButtons = GetNode<HBoxContainer>("Content/InventoryPanel/MarginContainer/InventoryLayout/FilterButtons");
		_inventoryGrid = GetNode<GridContainer>("Content/InventoryPanel/MarginContainer/InventoryLayout/InventoryGrid");
		_previousPageButton = GetNode<Button>("Content/InventoryPanel/MarginContainer/InventoryLayout/PageControls/PreviousPageButton");
		_nextPageButton = GetNode<Button>("Content/InventoryPanel/MarginContainer/InventoryLayout/PageControls/NextPageButton");
		_pageLabel = GetNode<Label>("Content/InventoryPanel/MarginContainer/InventoryLayout/PageControls/PageLabel");
		_details = GetNode<InventoryDetails>("Content/InventoryPanel/MarginContainer/InventoryLayout/Details");
		_emptyInventoryLabel = GetNode<Label>("Content/InventoryPanel/MarginContainer/InventoryLayout/EmptyInventoryLabel");

		_previousPageButton.Pressed += ShowPreviousPage;
		_nextPageButton.Pressed += ShowNextPage;
		_details.EquipPressed += OnEquipPressed;
		_details.OffhandPressed += OnOffhandPressed;
		_details.UnequipPressed += OnUnequipPressed;
		_equipmentPanel.EquipmentSlotPressed += OnEquipmentSlotPressed;

		foreach (var button in _inventoryButtons) {
			button.Pressed += () => OnInventoryBoxPressed(button);
		}

		LoadItems();
		BuildFilterButtons();
		_equipmentPanel.Render();
		RenderInventoryPage();
	}

	private void LoadItems() {
		_items = SaveNode.Get()?.InventoryData?.Items?.Where(item => item != null).ToList() ?? new List<ItemBase>();
	}

	private void BuildFilterButtons() {
		foreach (var child in _filterButtons.GetChildren()) {
			child.QueueFree();
		}

		_filterButtons.AddChild(CreateFilterButton("All", null));

		foreach (var itemType in ItemFilterTypes) {
			_filterButtons.AddChild(CreateFilterButton(GetFilterName(itemType), itemType));
		}
	}

	private Button CreateFilterButton(string label, Type itemType) {
		var button = new Button {
			Text = label,
			CustomMinimumSize = new Vector2(92, 56),
			SizeFlagsHorizontal = SizeFlags.ShrinkBegin,
			Icon = _placeholderIcon,
			IconAlignment = HorizontalAlignment.Center,
			VerticalIconAlignment = VerticalAlignment.Top,
			ExpandIcon = true,
			ClipText = true
		};

		button.Pressed += () => SetFilter(itemType);
		return button;
	}

	private void OnEquipmentSlotPressed(int slotIndex, ItemBase item, string slotName) {
		if (item == null) {
			ShowMessage($"{slotName}: Empty");
			return;
		}

		_details.ShowItem(item, $"{slotName}\n{FormatItemDetails(item)}", false, false, true);
	}

	private void SetFilter(Type itemType) {
		_filterType = itemType;
		_currentPage = 0;
		RenderInventoryPage();
	}

	private void RenderInventoryPage() {
		LoadItems();
		var visibleItems = GetFilteredItems();
		var hasItems = visibleItems.Count > 0;
		var pageCount = Mathf.Max(1, Mathf.CeilToInt(visibleItems.Count / (float)ItemsPerPage));
		_currentPage = Mathf.Clamp(_currentPage, 0, pageCount - 1);
		_inventoryGrid.Visible = hasItems;
		_emptyInventoryLabel.Visible = !hasItems;

		for (var i = 0; i < _inventoryButtons.Length; i++) {
			var itemIndex = _currentPage * ItemsPerPage + i;
			var button = _inventoryButtons[i];

			if (itemIndex >= visibleItems.Count) {
				button.Clear(_placeholderIcon);
				button.Visible = false;
				button.Disabled = true;
				continue;
			}

			button.Visible = true;
			button.Disabled = false;
			button.SetItem(visibleItems[itemIndex], _placeholderIcon);
		}

		_pageLabel.Visible = hasItems;
		_previousPageButton.Visible = hasItems;
		_nextPageButton.Visible = hasItems;
		_previousPageButton.Disabled = _currentPage <= 0;
		_nextPageButton.Disabled = _currentPage >= pageCount - 1;
		_pageLabel.Text = $"Page {_currentPage + 1} / {pageCount}";
	}

	private List<ItemBase> GetFilteredItems() {
		if (_filterType == null) return _items;

		return _items.Where(item => item.GetType() == _filterType).ToList();
	}

	private static string GetFilterName(Type itemType) {
		var name = itemType.Name;
		return name.StartsWith("Item") ? name[4..] : name;
	}

	private void ShowPreviousPage() {
		_currentPage--;
		RenderInventoryPage();
	}

	private void ShowNextPage() {
		_currentPage++;
		RenderInventoryPage();
	}

	private void OnInventoryBoxPressed(ItemInventory button) {
		ShowDetails(button.Item);
	}

	private void ShowDetails(ItemBase item) {
		_details.ShowItem(item, FormatItemDetails(item), IsEquipable(item), item is ItemEquipable, false);
	}

	private void ShowMessage(string text) {
		_details.ShowMessage(text);
	}

	private void OnEquipPressed(ItemBase item) {
		EquipItem(item, false);
	}

	private void OnOffhandPressed(ItemBase item) {
		EquipItem(item, true);
	}

	private void OnUnequipPressed(ItemBase item) {
		var saveNode = SaveNode.Get();
		if (saveNode?.InventoryData == null || saveNode.EquipedItemsData == null)
			return;

		if (!saveNode.InventoryData.TryUnequipItem(item, saveNode.EquipedItemsData)) {
			ShowMessage($"Cannot unequip {item.ItemName}.");
			return;
		}

		_equipmentPanel.Render();
		RenderInventoryPage();
		ShowMessage($"Unequipped {item.ItemName}.");
	}

	private void EquipItem(ItemBase item, bool offhand) {
		var saveNode = SaveNode.Get();
		if (saveNode?.InventoryData == null || saveNode.EquipedItemsData == null)
			return;

		var equipped = offhand
			? saveNode.InventoryData.TryEquipOffhandItem(item, saveNode.EquipedItemsData)
			: saveNode.InventoryData.TryEquipItem(item, saveNode.EquipedItemsData);

		if (!equipped) {
			ShowMessage($"Cannot equip {item.ItemName}.");
			return;
		}

		SignalHandler.EmitSignalItemEquippedStatic(item);
		_equipmentPanel.Render();
		RenderInventoryPage();
		ShowMessage($"Equipped {item.ItemName}{(offhand ? " to off hand" : "")}." );
	}

	private static string FormatItemDetails(ItemBase item) {
		return $"{item.ItemName}\nCost: {item.Cost}\nStack: {item.MaxStackSize}";
	}

	private static bool IsEquipable(ItemBase item) {
		return item is ItemEquipable or ItemArmor or ItemAmulet or ItemAmmo or ItemConsumable;
	}
}
