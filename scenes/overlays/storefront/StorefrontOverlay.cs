using Godot;

public partial class StorefrontOverlay : Control {
	private GridContainer _itemGrid;
	private Label _emptyLabel;
	private TextureRect _previewIcon;
	private Label _previewDetails;
	private Label _priceLabel;
	private Button _purchaseButton;
	private readonly PlaceholderTexture2D _placeholderIcon = new() { Size = new Vector2I(64, 64) };
	private BuildingData _buildingData;
	private ItemBase _selectedItem;

	public override void _Ready() {
		_itemGrid = GetNode<GridContainer>("Content/StorePanel/MarginContainer/StoreLayout/ItemGrid");
		_emptyLabel = GetNode<Label>("Content/StorePanel/MarginContainer/StoreLayout/EmptyLabel");
		_previewIcon = GetNode<TextureRect>("Content/PreviewPanel/MarginContainer/PreviewLayout/PreviewIcon");
		_previewDetails = GetNode<Label>("Content/PreviewPanel/MarginContainer/PreviewLayout/PreviewDetails");
		_priceLabel = GetNode<Label>("Content/PreviewPanel/MarginContainer/PreviewLayout/PriceLabel");
		_purchaseButton = GetNode<Button>("Content/PreviewPanel/MarginContainer/PreviewLayout/PurchaseButton");

		_purchaseButton.Pressed += PurchaseSelectedItem;
		SignalHandler.SubscribeGoldAmountChanged(OnGoldAmountChanged);
		Render();
	}

	public override void _ExitTree() {
		SignalHandler.UnsubscribeGoldAmountChanged(OnGoldAmountChanged);
	}

	public void Update(BuildingData buildingData) {
		_buildingData = buildingData;
		_selectedItem = null;

		if (IsNodeReady())
			Render();
	}

	private void Render() {
		RenderItems();
		RenderPreview();
	}

	private void RenderItems() {
		foreach (var child in _itemGrid.GetChildren())
			child.QueueFree();

		var items = _buildingData?.StorefrontItems;
		var hasItems = items != null && items.Count > 0;
		_itemGrid.Visible = hasItems;
		_emptyLabel.Visible = !hasItems;

		if (!hasItems)
			return;

		foreach (var item in items) {
			if (item == null)
				continue;

			_itemGrid.AddChild(CreateItemButton(item));
		}
	}

	private Button CreateItemButton(ItemBase item) {
		var button = new Button {
			CustomMinimumSize = new Vector2(132, 92),
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill,
			Text = item.ItemName,
			Icon = item.Icon ?? _placeholderIcon,
			IconAlignment = HorizontalAlignment.Center,
			VerticalIconAlignment = VerticalAlignment.Top,
			ExpandIcon = true,
			ClipText = true,
			AutowrapMode = TextServer.AutowrapMode.WordSmart
		};

		button.Pressed += () => SelectItem(item);
		return button;
	}

	private void SelectItem(ItemBase item) {
		_selectedItem = item;
		RenderPreview();
	}

	private void RenderPreview() {
		if (_selectedItem == null) {
			_previewIcon.Texture = _placeholderIcon;
			_previewDetails.Text = "Select an item to preview it.";
			_priceLabel.Text = "Price: -";
			_purchaseButton.Disabled = true;
			return;
		}

		_previewIcon.Texture = _selectedItem.Icon ?? _placeholderIcon;
		_previewDetails.Text = FormatItemDetails(_selectedItem);
		_priceLabel.Text = $"Price: {_selectedItem.Cost}";
		_purchaseButton.Disabled = !CanPurchase(_selectedItem);
	}

	private bool CanPurchase(ItemBase item) {
		var saveNode = SaveNode.Get();
		return item != null && saveNode?.RunData != null && saveNode.RunData.Gold >= item.Cost;
	}

	private void PurchaseSelectedItem() {
		if (!CanPurchase(_selectedItem)) {
			RenderPreview();
			return;
		}

		var saveNode = SaveNode.Get();
		saveNode.RunData.Gold -= _selectedItem.Cost;
		saveNode.InventoryData.AddItem(_selectedItem);
		_buildingData.StorefrontItems.Remove(_selectedItem);
		saveNode.SaveRunData();

		SignalHandler.EmitSignalPurchaseItemStatic(_selectedItem);
		SignalHandler.EmitSignalGoldAmountChangedStatic(saveNode.RunData.Gold);

		var purchasedItemName = _selectedItem.ItemName;
		_selectedItem = null;
		Render();
		_previewDetails.Text = $"Purchased {purchasedItemName}.";
	}

	private void OnGoldAmountChanged(int goldAmount) {
		RenderPreview();
	}

	private static string FormatItemDetails(ItemBase item) {
		return $"{item.ItemName}\nStack: {item.MaxStackSize}";
	}
}
