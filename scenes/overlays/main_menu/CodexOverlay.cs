using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class CodexOverlay : Control {
	private const int EntriesPerPage = 10;

	private enum ViewMode {
		EntryGrid,
		EnemySubcategories,
		EntryDetails
	}

	private VBoxContainer _categoryButtons;
	private HBoxContainer _subcategoryButtons;
	private Label _categoryTitle;
	private Label _emptyState;
	private Label _progressLabel;
	private Button _backButton;
	private GridContainer _entryGrid;
	private HBoxContainer _pageControls;
	private Button _previousPageButton;
	private Label _pageLabel;
	private Button _nextPageButton;
	private ViewMode _viewMode;
	private CodexCategory _currentCategory = CodexCategory.Items;
	private CodexSubcategory _currentSubcategory = CodexSubcategory.All;
	private List<CodexEntryData> _currentEntries = new();
	private int _currentPage;

	public override void _Ready() {
		_categoryButtons = GetNode<VBoxContainer>("Content/CategoryPanel/MarginContainer/CategoryLayout/CategoryButtons");
		_subcategoryButtons = GetNode<HBoxContainer>("Content/RightPanel/MarginContainer/RightLayout/SubcategoryButtons");
		_categoryTitle = GetNode<Label>("Content/RightPanel/MarginContainer/RightLayout/CategoryTitle");
		_backButton = GetNode<Button>("Content/RightPanel/MarginContainer/RightLayout/BackButton");
		_emptyState = GetNode<Label>("Content/RightPanel/MarginContainer/RightLayout/EmptyState");
		_entryGrid = GetNode<GridContainer>("Content/RightPanel/MarginContainer/RightLayout/EntryGrid");
		_pageControls = GetNode<HBoxContainer>("Content/RightPanel/MarginContainer/RightLayout/PageControls");
		_previousPageButton = GetNode<Button>("Content/RightPanel/MarginContainer/RightLayout/PageControls/PreviousPageButton");
		_pageLabel = GetNode<Label>("Content/RightPanel/MarginContainer/RightLayout/PageControls/PageLabel");
		_nextPageButton = GetNode<Button>("Content/RightPanel/MarginContainer/RightLayout/PageControls/NextPageButton");
		_progressLabel = GetNode<Label>("Content/RightPanel/MarginContainer/RightLayout/ProgressLabel");
		_backButton.Pressed += OnBackPressed;
		_previousPageButton.Pressed += ShowPreviousPage;
		_nextPageButton.Pressed += ShowNextPage;

		BuildCategoryButtons();
		SelectCategory(CodexCategory.Items);
	}

	private void BuildCategoryButtons() {
		foreach (var category in CodexData.Categories.Keys) {
			var button = new Button {
				Text = category.ToString(),
				CustomMinimumSize = new Vector2(0, 64),
				SizeFlagsHorizontal = SizeFlags.ExpandFill
			};

			button.Pressed += () => SelectCategory(category);
			_categoryButtons.AddChild(button);
		}
	}

	private void SelectCategory(CodexCategory category) {
		_currentCategory = category;
		_currentSubcategory = CodexSubcategory.All;
		_categoryTitle.Text = category.ToString();
		_backButton.Visible = false;

		foreach (var child in _subcategoryButtons.GetChildren()) {
			child.QueueFree();
		}

		if (category == CodexCategory.Enemies) {
			ShowEnemySubcategories();
			return;
		}

		UpdateCategoryView(category, CodexSubcategory.All);

		foreach (var subcategory in CodexData.Categories[category]) {
			var button = new Button {
				Text = subcategory.ToString(),
				CustomMinimumSize = new Vector2(120, 48),
				SizeFlagsHorizontal = SizeFlags.ExpandFill
			};

			button.Pressed += () => UpdateCategoryView(category, subcategory);
			_subcategoryButtons.AddChild(button);
		}
	}

	private void ShowEnemySubcategories() {
		_viewMode = ViewMode.EnemySubcategories;
		_currentCategory = CodexCategory.Enemies;
		_currentSubcategory = CodexSubcategory.All;
		_categoryTitle.Text = CodexCategory.Enemies.ToString();
		_backButton.Visible = false;
		_emptyState.Text = "Select an enemy type.";
		_progressLabel.Text = "";
		_pageControls.Visible = false;
		ClearEntryGrid();

		foreach (var child in _subcategoryButtons.GetChildren()) {
			child.QueueFree();
		}

		foreach (var subcategory in CodexData.Categories[CodexCategory.Enemies]) {
			_entryGrid.AddChild(CreateSubcategoryCard(subcategory));
		}
	}

	private Button CreateSubcategoryCard(CodexSubcategory subcategory) {
		var card = CreateGridButton();
		var layout = CreateCenteredCardLayout();

		var icon = new TextureRect {
			CustomMinimumSize = new Vector2(88, 88),
			Texture = new PlaceholderTexture2D(),
			SizeFlagsHorizontal = SizeFlags.ShrinkCenter,
			ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
		};

		var title = new Label {
			Text = subcategory.ToString(),
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			HorizontalAlignment = HorizontalAlignment.Center,
			AutowrapMode = TextServer.AutowrapMode.WordSmart
		};

		layout.AddChild(icon);
		layout.AddChild(title);
		card.AddChild(layout);
		card.Pressed += () => ShowEnemyEntries(subcategory);

		return card;
	}

	private void ShowEnemyEntries(CodexSubcategory subcategory) {
		_currentCategory = CodexCategory.Enemies;
		_currentSubcategory = subcategory;
		_categoryTitle.Text = $"{CodexCategory.Enemies} / {subcategory}";
		_backButton.Visible = true;

		foreach (var child in _subcategoryButtons.GetChildren()) {
			child.QueueFree();
		}

		UpdateCategoryView(CodexCategory.Enemies, subcategory);
	}

	private void UpdateCategoryView(CodexCategory category, CodexSubcategory subcategory) {
		_viewMode = ViewMode.EntryGrid;
		_currentCategory = category;
		_currentSubcategory = subcategory;
		_currentEntries = CodexData.GetEntries(category, subcategory).ToList();
		_currentPage = 0;
		RenderCurrentPage();
	}

	private void RenderCurrentPage() {
		var pageCount = GetPageCount();
		_currentPage = Mathf.Clamp(_currentPage, 0, Mathf.Max(0, pageCount - 1));
		var entries = _currentEntries
			.Skip(_currentPage * EntriesPerPage)
			.Take(EntriesPerPage)
			.ToArray();

		_emptyState.Text = _currentEntries.Count == 0 ? "No entries." : "";
		ClearEntryGrid();

		foreach (var entry in entries) {
			_entryGrid.AddChild(CreateEntryCard(entry));
		}

		_pageControls.Visible = _currentEntries.Count > EntriesPerPage;
		_previousPageButton.Disabled = _currentPage <= 0;
		_nextPageButton.Disabled = _currentPage >= pageCount - 1;
		_pageLabel.Text = $"Page {_currentPage + 1} / {Mathf.Max(1, pageCount)}";
		_progressLabel.Text = $"Entries: {_currentEntries.Count}";
	}

	private int GetPageCount() {
		return Mathf.CeilToInt(_currentEntries.Count / (float)EntriesPerPage);
	}

	private void ShowPreviousPage() {
		_currentPage--;
		RenderCurrentPage();
	}

	private void ShowNextPage() {
		_currentPage++;
		RenderCurrentPage();
	}

	private Button CreateEntryCard(CodexEntryData entry) {
		var card = CreateGridButton();
		var layout = CreateCenteredCardLayout();

		var icon = new TextureRect {
			CustomMinimumSize = new Vector2(88, 88),
			Texture = entry.Icon ?? new PlaceholderTexture2D(),
			SizeFlagsHorizontal = SizeFlags.ShrinkCenter,
			ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
		};

		var title = new Label {
			Text = entry.Title,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			HorizontalAlignment = HorizontalAlignment.Center,
			AutowrapMode = TextServer.AutowrapMode.WordSmart
		};

		layout.AddChild(icon);
		layout.AddChild(title);
		card.AddChild(layout);
		card.Pressed += () => ShowEntryDetails(entry);

		return card;
	}

	private Button CreateGridButton() {
		return new Button {
			CustomMinimumSize = new Vector2(112, 156),
			Text = string.Empty,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill
		};
	}

	private VBoxContainer CreateCenteredCardLayout() {
		var layout = new VBoxContainer {
			Alignment = BoxContainer.AlignmentMode.Center,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill
		};

		layout.SetAnchorsPreset(LayoutPreset.FullRect);
		layout.SetOffsetsPreset(LayoutPreset.FullRect);
		return layout;
	}

	private void ShowEntryDetails(CodexEntryData entry) {
		_viewMode = ViewMode.EntryDetails;
		_categoryTitle.Text = entry.Title;
		_backButton.Visible = true;
		_pageControls.Visible = false;
		_progressLabel.Text = $"{entry.Category} / {entry.Subcategory}";

		foreach (var child in _subcategoryButtons.GetChildren()) {
			child.QueueFree();
		}

		ClearEntryGrid();
		_emptyState.Text = string.Empty;
		_entryGrid.AddChild(CreateDetailCard(entry));
	}

	private Control CreateDetailCard(CodexEntryData entry) {
		var card = new PanelContainer {
			CustomMinimumSize = new Vector2(360, 420)
		};

		var layout = new VBoxContainer {
			Alignment = BoxContainer.AlignmentMode.Center
		};

		var icon = new TextureRect {
			CustomMinimumSize = new Vector2(180, 180),
			Texture = entry.Icon ?? new PlaceholderTexture2D(),
			ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
		};

		var title = new Label {
			Text = entry.Title,
			HorizontalAlignment = HorizontalAlignment.Center
		};
		title.AddThemeFontSizeOverride("font_size", 32);

		var meta = new Label {
			Text = $"{entry.Category} / {entry.Subcategory}",
			HorizontalAlignment = HorizontalAlignment.Center
		};

		var description = new Label {
			Text = string.IsNullOrWhiteSpace(entry.Description) ? "No description yet." : entry.Description,
			HorizontalAlignment = HorizontalAlignment.Center,
			AutowrapMode = TextServer.AutowrapMode.WordSmart
		};

		layout.AddChild(icon);
		layout.AddChild(title);
		layout.AddChild(meta);
		layout.AddChild(description);
		card.AddChild(layout);

		return card;
	}

	private void OnBackPressed() {
		if (_viewMode == ViewMode.EntryDetails) {
			var category = _currentCategory;
			var subcategory = _currentSubcategory;
			var page = _currentPage;

			if (_currentCategory == CodexCategory.Enemies) {
				ShowEnemyEntries(subcategory);
				_currentPage = page;
				RenderCurrentPage();
				return;
			}

			SelectCategory(category);
			UpdateCategoryView(category, subcategory);
			_currentPage = page;
			RenderCurrentPage();
			return;
		}

		if (_currentCategory == CodexCategory.Enemies) {
			ShowEnemySubcategories();
		}
	}

	private void ClearEntryGrid() {
		foreach (var child in _entryGrid.GetChildren()) {
			child.QueueFree();
		}
	}
}
