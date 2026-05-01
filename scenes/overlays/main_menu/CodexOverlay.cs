using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class CodexOverlay : Control {
	private const int EntriesPerPage = 10;

	private enum ViewMode {
		EntryGrid,
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
				Text = GetCategoryDisplayName(category),
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
		_categoryTitle.Text = GetCategoryDisplayName(category);
		_backButton.Visible = false;
		_subcategoryButtons.Visible = true;
		_emptyState.Visible = true;

		foreach (var child in _subcategoryButtons.GetChildren()) {
			child.QueueFree();
		}

		UpdateCategoryView(category, CodexSubcategory.All);

		foreach (var subcategory in CodexData.Categories[category]) {
			var button = CreateSubcategoryButton(category, subcategory);

			button.Pressed += () => UpdateCategoryView(category, subcategory);
			_subcategoryButtons.AddChild(button);
		}
	}

	private Button CreateSubcategoryButton(CodexCategory category, CodexSubcategory subcategory) {
		return new Button {
			Text = GetSubcategoryDisplayName(subcategory),
			ClipText = true,
			Icon = GetSubcategoryIcon(category, subcategory),
			ExpandIcon = true,
			CustomMinimumSize = new Vector2(120, 48),
			SizeFlagsHorizontal = SizeFlags.ExpandFill
		};
	}

	private Texture2D GetSubcategoryIcon(CodexCategory category, CodexSubcategory subcategory) {
		return CodexData.GetEntries(category, subcategory)
			.Select(entry => entry.Icon)
			.FirstOrDefault(icon => icon != null) ?? new PlaceholderTexture2D();
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
		_subcategoryButtons.Visible = false;
		_emptyState.Visible = false;
		_pageControls.Visible = false;
		_progressLabel.Text = $"{GetCategoryDisplayName(entry.Category)} / {GetSubcategoryDisplayName(entry.Subcategory)}";

		foreach (var child in _subcategoryButtons.GetChildren()) {
			child.QueueFree();
		}

		ClearEntryGrid();
		_emptyState.Text = string.Empty;
		_entryGrid.AddChild(CreateDetailCard(entry));
	}

	private Control CreateDetailCard(CodexEntryData entry) {
		var card = new PanelContainer {
			CustomMinimumSize = new Vector2(360, 360)
		};

		var layout = new VBoxContainer {
			Alignment = BoxContainer.AlignmentMode.Center
		};

		var icon = new TextureRect {
			CustomMinimumSize = new Vector2(152, 152),
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
			Text = $"{GetCategoryDisplayName(entry.Category)} / {GetSubcategoryDisplayName(entry.Subcategory)}",
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

			SelectCategory(category);
			UpdateCategoryView(category, subcategory);
			_currentPage = page;
			RenderCurrentPage();
			return;
		}
	}

	private void ClearEntryGrid() {
		foreach (var child in _entryGrid.GetChildren()) {
			child.QueueFree();
		}
	}

	private static string GetCategoryDisplayName(CodexCategory category) {
		return category switch {
			CodexCategory.Biomes => "Regions",
			_ => category.ToString()
		};
	}

	private static string GetSubcategoryDisplayName(CodexSubcategory subcategory) {
		var name = subcategory.ToString();
		var separatorIndex = name.IndexOf('_');
		var displayName = separatorIndex >= 0 ? name[(separatorIndex + 1)..] : name;

		return displayName.Replace('_', ' ');
	}
}
