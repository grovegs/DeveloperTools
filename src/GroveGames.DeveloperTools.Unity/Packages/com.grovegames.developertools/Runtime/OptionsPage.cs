using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class OptionsPage : IDeveloperToolsPage<VisualElement>
    {
        private const string AllCategory = "All";
        private const float ChevronSize = 32f;
        private const string EmptyText = "No options yet. Register them with builder.AddDeveloperOptions<T>().";
        private const string NoMatchText = "No options match.";

        private readonly DeveloperOptionsRegistry _registry;
        private readonly Font? _boldFont;
        private readonly List<string> _categories;
        private readonly List<VisualElement> _sections;
        private readonly List<ChevronIcon> _sectionChevrons;
        private readonly List<VisualElement> _sectionContents;
        private readonly List<DeveloperOptionRow> _rows;
        private readonly List<int> _rowSections;
        private readonly List<VisualElement> _subHeaders;
        private readonly List<Button> _categoryButtons;
        private readonly HashSet<string> _collapsedCategories;
        private readonly VisualElement _categoryBar;
        private readonly ScrollView _scrollView;
        private readonly Label _empty;
        private readonly Action _changedHandler;
        private int _version = -1;
        private string _selectedCategory = AllCategory;
        private string _searchText = string.Empty;
        private readonly VisualElement _root;

        public string Title => "Options";
        public VisualElement Root => _root;

        public OptionsPage(DeveloperOptionsRegistry registry, Font? boldFont)
        {
            _registry = registry;
            _boldFont = boldFont;
            _categories = new List<string>();
            _sections = new List<VisualElement>();
            _sectionChevrons = new List<ChevronIcon>();
            _sectionContents = new List<VisualElement>();
            _rows = new List<DeveloperOptionRow>();
            _rowSections = new List<int>();
            _subHeaders = new List<VisualElement>();
            _categoryButtons = new List<Button>();
            _collapsedCategories = new HashSet<string>();
            _changedHandler = ReadAll;

            _root = new VisualElement();
            Root.style.flexGrow = 1f;
            Root.style.flexShrink = 1f;
            Root.style.minHeight = 0f;

            var search = new TextField();
            search.textEdition.placeholder = "Search";
            search.style.marginBottom = DeveloperToolsStyle.Spacing;
            search.style.flexShrink = 0f;
            search.RegisterValueChangedCallback(HandleSearchChanged);
            DeveloperToolsStyle.StyleInput(search);
            Root.Add(search);

            _categoryBar = DeveloperToolsStyle.CreateRow();
            _categoryBar.style.flexWrap = Wrap.Wrap;
            _categoryBar.style.marginBottom = 0f;
            Root.Add(_categoryBar);

            _empty = new Label(EmptyText);
            _empty.style.fontSize = DeveloperToolsStyle.FontSize;
            _empty.style.color = DeveloperToolsStyle.MutedTextColor;
            _empty.style.whiteSpace = WhiteSpace.Normal;
            _empty.style.marginTop = DeveloperToolsStyle.LargeSpacing;
            Root.Add(_empty);

            _scrollView = new ScrollView(ScrollViewMode.Vertical);
            _scrollView.style.flexGrow = 1f;
            _scrollView.style.flexShrink = 1f;
            _scrollView.style.minHeight = 0f;
            _scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            Root.Add(_scrollView);
        }

        public void Refresh(bool force)
        {
            if (_registry.Version != _version)
            {
                Rebuild();
                return;
            }

            if (force)
            {
                ReadAll();
            }
        }

        private void Rebuild()
        {
            _version = _registry.Version;
            _categories.Clear();
            _sections.Clear();
            _sectionChevrons.Clear();
            _sectionContents.Clear();
            _rows.Clear();
            _rowSections.Clear();
            _subHeaders.Clear();
            _scrollView.Clear();

            for (var i = 0; i < _registry.Count; i++)
            {
                var options = _registry.Get(i);
                var section = GetOrCreateSection(options.Category);
                var content = _sectionContents[section];
                string? header = null;

                foreach (var option in DeveloperOptionsReader.Read(options.GetType()))
                {
                    if (option.Header != null && option.Header != header)
                    {
                        header = option.Header;
                        content.Add(CreateSubHeader(header));
                    }

                    var row = new DeveloperOptionRow(option, options, _changedHandler);
                    content.Add(row.Root);
                    _rows.Add(row);
                    _rowSections.Add(section);
                }
            }

            if (!_categories.Contains(_selectedCategory))
            {
                _selectedCategory = AllCategory;
            }

            RebuildCategoryBar();
            ReadAll();
            ApplyFilter();
        }

        private int GetOrCreateSection(string category)
        {
            var index = _categories.IndexOf(category);

            if (index >= 0)
            {
                return index;
            }

            var section = new VisualElement();
            section.style.backgroundColor = DeveloperToolsStyle.SurfaceColor;
            section.style.marginTop = DeveloperToolsStyle.Spacing;
            section.style.paddingLeft = DeveloperToolsStyle.LargeSpacing;
            section.style.paddingRight = DeveloperToolsStyle.LargeSpacing;
            section.style.paddingBottom = DeveloperToolsStyle.Spacing;
            DeveloperToolsStyle.SetBorderRadius(section.style, DeveloperToolsStyle.CornerRadius);

            var header = new Button(() => ToggleSection(category));
            header.style.flexDirection = FlexDirection.Row;
            header.style.alignItems = Align.Center;
            header.style.height = DeveloperToolsStyle.ControlHeight;
            header.style.color = DeveloperToolsStyle.TextColor;
            header.style.backgroundColor = Color.clear;
            header.style.marginLeft = -DeveloperToolsStyle.Spacing;
            header.style.marginRight = -DeveloperToolsStyle.Spacing;
            header.style.paddingLeft = DeveloperToolsStyle.Spacing;
            DeveloperToolsStyle.SetBorderWidth(header.style, 0f);
            DeveloperToolsStyle.AddHighlightFeedback(header);
            DeveloperToolsStyle.SetBorderRadius(header.style, DeveloperToolsStyle.CornerRadius);
            var chevron = new ChevronIcon(ChevronSize);
            chevron.style.marginRight = DeveloperToolsStyle.Spacing;
            header.Add(chevron);
            var title = new Label(category) { pickingMode = PickingMode.Ignore };
            title.style.fontSize = DeveloperToolsStyle.TitleFontSize;
            DeveloperToolsStyle.ApplyBold(title, _boldFont);
            header.Add(title);
            section.Add(header);

            var content = new VisualElement();
            section.Add(content);
            _scrollView.Add(section);

            _categories.Add(category);
            _sections.Add(section);
            _sectionChevrons.Add(chevron);
            _sectionContents.Add(content);
            return _categories.Count - 1;
        }

        private VisualElement CreateSubHeader(string text)
        {
            var label = new Label(text.ToUpperInvariant());
            label.style.fontSize = DeveloperToolsStyle.FontSize;
            label.style.color = DeveloperToolsStyle.MutedTextColor;
            label.style.marginTop = DeveloperToolsStyle.Spacing;
            DeveloperToolsStyle.ApplyBold(label, _boldFont);
            _subHeaders.Add(label);
            return label;
        }

        private void RebuildCategoryBar()
        {
            _categoryBar.Clear();
            _categoryButtons.Clear();

            if (_categories.Count < 2)
            {
                return;
            }

            AddCategoryButton(AllCategory);

            for (var i = 0; i < _categories.Count; i++)
            {
                AddCategoryButton(_categories[i]);
            }
        }

        private void AddCategoryButton(string category)
        {
            var button = DeveloperToolsStyle.CreateButton(category, () => SelectCategory(category));
            button.style.marginBottom = DeveloperToolsStyle.Spacing;
            button.userData = category;
            _categoryButtons.Add(button);
            _categoryBar.Add(button);
        }

        private void SelectCategory(string category)
        {
            _selectedCategory = category;
            ApplyFilter();
        }

        private void ToggleSection(string category)
        {
            if (!_collapsedCategories.Remove(category))
            {
                _collapsedCategories.Add(category);
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var isSearching = _searchText.Length > 0;

            for (var i = 0; i < _categoryButtons.Count; i++)
            {
                var button = _categoryButtons[i];
                var isSelected = (string)button.userData == _selectedCategory;
                button.style.backgroundColor = isSelected ? DeveloperToolsStyle.AccentColor : DeveloperToolsStyle.ControlColor;
            }

            for (var i = 0; i < _subHeaders.Count; i++)
            {
                _subHeaders[i].style.display = isSearching ? DisplayStyle.None : DisplayStyle.Flex;
            }

            var visibleSections = 0;

            for (var section = 0; section < _sections.Count; section++)
            {
                var category = _categories[section];
                var isCollapsed = !isSearching && _collapsedCategories.Contains(category);
                var hasRows = false;

                for (var i = 0; i < _rows.Count; i++)
                {
                    if (_rowSections[i] != section)
                    {
                        continue;
                    }

                    var matches = _rows[i].Matches(_searchText) || category.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    _rows[i].Root.style.display = matches ? DisplayStyle.Flex : DisplayStyle.None;
                    hasRows |= matches;
                }

                var isVisible = hasRows && (_selectedCategory == AllCategory || _selectedCategory == category);
                _sections[section].style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
                _sectionContents[section].style.display = isCollapsed ? DisplayStyle.None : DisplayStyle.Flex;
                _sectionChevrons[section].SetExpanded(!isCollapsed);

                if (isVisible)
                {
                    visibleSections++;
                }
            }

            _empty.text = _registry.Count == 0 ? EmptyText : NoMatchText;
            _empty.style.display = visibleSections == 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void ReadAll()
        {
            for (var i = 0; i < _rows.Count; i++)
            {
                _rows[i].Read();
            }
        }

        private void HandleSearchChanged(ChangeEvent<string> changeEvent)
        {
            _searchText = changeEvent.newValue ?? string.Empty;
            ApplyFilter();
        }
    }
}
