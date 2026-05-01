using Godot;
using System.Linq;

public partial class SmithOverlay : Control {
	private const int ItemsPerPage = 8;

	private GridContainer _itemGrid;
	private Label _emptyLabel;
	private TextureRect _previewIcon;
	private Label _previewDetails;
	private Button _selectButton;
	private Button _previousPageButton;
	private Button _nextPageButton;
	private Label _pageLabel;
	private readonly PlaceholderTexture2D _placeholderIcon = new() { Size = new Vector2I(64, 64) };
	private ItemBase[] _items = System.Array.Empty<ItemBase>();
	private ItemBase _selectedItem;
	private int _currentPage;

	public override void _Ready() {
		_itemGrid = GetNode<GridContainer>("Content/ItemPanel/MarginContainer/ItemLayout/ItemGrid");
		_emptyLabel = GetNode<Label>("Content/ItemPanel/MarginContainer/ItemLayout/EmptyLabel");
		_previewIcon = GetNode<TextureRect>("Content/PreviewPanel/MarginContainer/PreviewLayout/PreviewIcon");
		_previewDetails = GetNode<Label>("Content/PreviewPanel/MarginContainer/PreviewLayout/PreviewDetails");
		_selectButton = GetNode<Button>("Content/PreviewPanel/MarginContainer/PreviewLayout/SelectButton");
		_previousPageButton = GetNode<Button>("Content/PreviewPanel/MarginContainer/PreviewLayout/PageControls/PreviousPageButton");
		_nextPageButton = GetNode<Button>("Content/PreviewPanel/MarginContainer/PreviewLayout/PageControls/NextPageButton");
		_pageLabel = GetNode<Label>("Content/PreviewPanel/MarginContainer/PreviewLayout/PageControls/PageLabel");

		_selectButton.Pressed += OnSelectPressed;
		_previousPageButton.Pressed += ShowPreviousPage;
		_nextPageButton.Pressed += ShowNextPage;

		LoadItems();
		Render();
	}

	private void LoadItems() {
		_currentPage = 0;
		_selectedItem = null;
		_items = SaveNode.Get()?.InventoryData?.Items?
			.Where(item => item != null && item.HasCompatibility(MyTypes.OutpostBuildingCompatibility.Smith))
			.ToArray() ?? System.Array.Empty<ItemBase>();
	}

	private void Render() {
		RenderItems();
		RenderPreview();
	}

	private void RenderItems() {
		foreach (var child in _itemGrid.GetChildren())
			child.QueueFree();

		var hasItems = _items.Length > 0;
		_itemGrid.Visible = hasItems;
		_emptyLabel.Visible = !hasItems;

		if (!hasItems) {
			_pageLabel.Visible = false;
			_previousPageButton.Visible = false;
			_nextPageButton.Visible = false;
			return;
		}

		var pageCount = Mathf.Max(1, Mathf.CeilToInt(_items.Length / (float)ItemsPerPage));
		_currentPage = Mathf.Clamp(_currentPage, 0, pageCount - 1);

		foreach (var item in _items.Skip(_currentPage * ItemsPerPage).Take(ItemsPerPage)) {
			_itemGrid.AddChild(CreateItemButton(item));
		}

		_pageLabel.Visible = true;
		_previousPageButton.Visible = true;
		_nextPageButton.Visible = true;
		_previousPageButton.Disabled = _currentPage <= 0;
		_nextPageButton.Disabled = _currentPage >= pageCount - 1;
		_pageLabel.Text = $"Page {_currentPage + 1} / {pageCount}";
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
			_previewDetails.Text = _items.Length == 0
				? "No items are explicitly marked for smith upgrades."
				: "Select an item that is explicitly marked for smith upgrades.";
			_selectButton.Disabled = true;
			return;
		}

		_previewIcon.Texture = _selectedItem.Icon ?? _placeholderIcon;
		_previewDetails.Text = $"{_selectedItem.ItemName}\nExplicitly marked for smith upgrades. Upgrade logic comes later.";
		_selectButton.Disabled = false;
	}

	private void OnSelectPressed() {
		if (_selectedItem == null)
			return;

		_previewDetails.Text = $"Selected {_selectedItem.ItemName}. Placeholder only.";
	}

	private void ShowPreviousPage() {
		_currentPage--;
		RenderItems();
	}

	private void ShowNextPage() {
		_currentPage++;
		RenderItems();
	}
}
