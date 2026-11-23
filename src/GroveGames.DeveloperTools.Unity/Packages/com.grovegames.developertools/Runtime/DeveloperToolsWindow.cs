using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class DeveloperToolsWindow : IDisposable
    {
        private const string HostName = "DeveloperTools";
        private const string Title = "Developer Tools";
        private const string VersionPrefix = "v";
        private const float CloseIconSize = 40f;
        private const float LogoSize = 96f;
        private const float TabUnderline = 6f;
        private const float MinimumHintOpacity = 0.4f;

        private readonly DeveloperToolsSettings _settings;
        private readonly DeveloperOptionsRegistry _optionsRegistry;
        private readonly DeveloperToolsPageRegistry _pageRegistry;
        private readonly List<IDeveloperToolsElementPage> _pages;
        private readonly List<Button> _tabs;
        private readonly EventCallback<GeometryChangedEvent> _geometryChangedHandler;
        private GameObject? _host;
        private PanelSettings? _panelSettings;
        private VisualElement? _root;
        private VisualElement? _panel;
        private VisualElement? _tabBar;
        private VisualElement? _content;
        private OptionsPage? _optionsPage;
        private VisualElement? _hint;
        private int _selectedPage;
        private int _pageVersion = -1;
        private float _hintHideTime;
        private Rect _hintCorner;

        private bool _isOpen;
        public bool IsOpen => _isOpen;

        public DeveloperToolsWindow(DeveloperToolsSettings settings, DeveloperOptionsRegistry optionsRegistry, DeveloperToolsPageRegistry pageRegistry)
        {
            _settings = settings;
            _optionsRegistry = optionsRegistry;
            _pageRegistry = pageRegistry;
            _pages = new List<IDeveloperToolsElementPage>();
            _tabs = new List<Button>();
            _geometryChangedHandler = HandleGeometryChanged;
        }

        public void Open()
        {
            if (IsOpen)
            {
                return;
            }

            EnsureCreated();
            _isOpen = true;
            HideHint();
            _panel!.style.display = DisplayStyle.Flex;
            ApplySafeArea();
            SyncPages();
            _pages[_selectedPage].Refresh(true);
        }

        public void Close()
        {
            if (!IsOpen)
            {
                return;
            }

            _isOpen = false;
            _panel!.style.display = DisplayStyle.None;
        }

        public void Toggle()
        {
            if (IsOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        public void ShowTapHint(Rect corner, int taps, int requiredTaps)
        {
            if (IsOpen || taps <= 0 || taps >= requiredTaps)
            {
                return;
            }

            _hintCorner = corner;
            EnsureCreated();
            ApplySafeArea();
            _hint!.style.display = DisplayStyle.Flex;
            _hint.style.opacity = Mathf.Lerp(MinimumHintOpacity, 1f, (float)taps / requiredTaps);
            _hintHideTime = Time.unscaledTime + _settings.TapInterval;
        }

        public void Update()
        {
            if (_hint != null && _hint.style.display == DisplayStyle.Flex && Time.unscaledTime >= _hintHideTime)
            {
                HideHint();
            }

            if (IsOpen)
            {
                SyncPages();
                _pages[_selectedPage].Refresh(false);
            }
        }

        public void Dispose()
        {
            if (_host != null)
            {
                Object.Destroy(_host);
            }

            if (_panelSettings != null)
            {
                Object.Destroy(_panelSettings);
            }

            _isOpen = false;
        }

        private void EnsureCreated()
        {
            if (_host != null)
            {
                return;
            }

            _panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            _panelSettings.themeStyleSheet = _settings.Theme;
            _panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            _panelSettings.referenceResolution = _settings.ReferenceResolution;
            _panelSettings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            _panelSettings.match = 0f;
            _panelSettings.sortingOrder = _settings.SortingOrder;

            _host = new GameObject(HostName);
            Object.DontDestroyOnLoad(_host);
            var document = _host.AddComponent<UIDocument>();
            document.panelSettings = _panelSettings;

            _root = document.rootVisualElement;
            _root.pickingMode = PickingMode.Ignore;
            _root.style.color = DeveloperToolsStyle.TextColor;
            _root.RegisterCallback(_geometryChangedHandler);

            if (_settings.Font != null)
            {
                _root.style.unityFontDefinition = new StyleFontDefinition(FontDefinition.FromFont(_settings.Font));
            }

            _optionsPage = new OptionsPage(_optionsRegistry, _settings.BoldFont);
            _panel = CreatePanel();
            _root.Add(_panel);
            _hint = CreateHint();
            _root.Add(_hint);
            SyncPages();
        }

        private VisualElement CreatePanel()
        {
            var panel = new VisualElement();
            panel.style.position = Position.Absolute;
            panel.style.left = 0f;
            panel.style.right = 0f;
            panel.style.top = 0f;
            panel.style.bottom = 0f;
            panel.style.backgroundColor = DeveloperToolsStyle.BackgroundColor;
            panel.style.display = DisplayStyle.None;

            panel.Add(CreateHeader());
            panel.Add(CreateTabBar());

            _content = new VisualElement();
            _content.style.flexGrow = 1f;
            _content.style.flexShrink = 1f;
            _content.style.minHeight = 0f;
            _content.style.overflow = Overflow.Hidden;
            panel.Add(_content);
            return panel;
        }

        private VisualElement CreateHeader()
        {
            var header = DeveloperToolsStyle.CreateRow();
            header.style.marginBottom = DeveloperToolsStyle.LargeSpacing;

            var logo = new Image { image = _settings.Logo };
            logo.style.width = LogoSize;
            logo.style.height = LogoSize;
            logo.style.marginRight = DeveloperToolsStyle.LargeSpacing;
            header.Add(logo);

            var titles = new VisualElement();
            var title = new Label(Title);
            title.style.fontSize = DeveloperToolsStyle.TitleFontSize;
            DeveloperToolsStyle.ApplyBold(title, _settings.BoldFont);
            titles.Add(title);
            var version = new Label(VersionPrefix + Application.version);
            version.style.fontSize = DeveloperToolsStyle.FontSize;
            version.style.color = DeveloperToolsStyle.MutedTextColor;
            titles.Add(version);
            header.Add(titles);

            header.Add(DeveloperToolsStyle.CreateSpacer());
            var close = DeveloperToolsStyle.CreateButton(string.Empty, Close);
            close.style.justifyContent = Justify.Center;
            close.style.alignItems = Align.Center;
            close.Add(new CrossIcon(CloseIconSize));
            close.style.width = DeveloperToolsStyle.ControlHeight;
            close.style.paddingLeft = 0f;
            close.style.paddingRight = 0f;
            close.style.marginRight = 0f;
            DeveloperToolsStyle.SetBorderRadius(close.style, DeveloperToolsStyle.ControlHeight * 0.5f);
            header.Add(close);
            return header;
        }

        private VisualElement CreateTabBar()
        {
            _tabBar = DeveloperToolsStyle.CreateRow();
            _tabBar.style.marginBottom = DeveloperToolsStyle.LargeSpacing;
            _tabBar.style.borderBottomWidth = 2f;
            _tabBar.style.borderBottomColor = DeveloperToolsStyle.ControlColor;
            return _tabBar;
        }

        private void SyncPages()
        {
            if (_pageRegistry.Version == _pageVersion)
            {
                return;
            }

            _pageVersion = _pageRegistry.Version;
            var selected = _selectedPage < _pages.Count ? _pages[_selectedPage] : null;
            _pages.Clear();
            _pages.Add(_optionsPage!);

            for (var i = 0; i < _pageRegistry.Count; i++)
            {
                if (_pageRegistry.Get(i) is IDeveloperToolsElementPage page)
                {
                    _pages.Add(page);
                }
            }

            RebuildTabs();
            var index = selected != null ? _pages.IndexOf(selected) : 0;
            SelectPage(index < 0 ? 0 : index);
        }

        private void RebuildTabs()
        {
            _tabBar!.Clear();
            _tabs.Clear();

            for (var i = 0; i < _pages.Count; i++)
            {
                var index = i;
                var tab = new Button(() => SelectPage(index)) { text = _pages[i].Title };
                tab.style.height = DeveloperToolsStyle.ControlHeight;
                tab.style.fontSize = DeveloperToolsStyle.ControlFontSize;
                tab.style.paddingLeft = DeveloperToolsStyle.LargeSpacing;
                tab.style.paddingRight = DeveloperToolsStyle.LargeSpacing;
                tab.style.marginLeft = 0f;
                tab.style.marginRight = DeveloperToolsStyle.Spacing;
                tab.style.marginBottom = 0f;
                tab.style.backgroundColor = Color.clear;
                DeveloperToolsStyle.SetBorderWidth(tab.style, 0f);
                DeveloperToolsStyle.SetBorderRadius(tab.style, 0f);
                tab.style.borderBottomColor = DeveloperToolsStyle.AccentColor;
                DeveloperToolsStyle.ApplyBold(tab, _settings.BoldFont);
                DeveloperToolsStyle.AddPressFeedback(tab);
                _tabs.Add(tab);
                _tabBar.Add(tab);
            }
        }

        private VisualElement CreateHint()
        {
            var hint = new VisualElement();
            hint.pickingMode = PickingMode.Ignore;
            hint.style.position = Position.Absolute;
            hint.style.display = DisplayStyle.None;
            hint.style.backgroundColor = DeveloperToolsStyle.BackgroundColor;
            DeveloperToolsStyle.SetBorderRadius(hint.style, DeveloperToolsStyle.CornerRadius);
            hint.style.paddingLeft = DeveloperToolsStyle.LargeSpacing;
            hint.style.paddingRight = DeveloperToolsStyle.LargeSpacing;
            hint.style.paddingTop = DeveloperToolsStyle.LargeSpacing;
            hint.style.paddingBottom = DeveloperToolsStyle.LargeSpacing;

            var logo = new VisualElement { pickingMode = PickingMode.Ignore };
            logo.style.flexGrow = 1f;
            logo.style.backgroundImage = new StyleBackground(_settings.Logo);
            logo.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            hint.Add(logo);
            return hint;
        }

        private void SelectPage(int index)
        {
            _selectedPage = index;
            _content!.Clear();
            _content.Add(_pages[index].Root);

            for (var i = 0; i < _tabs.Count; i++)
            {
                var isSelected = i == index;
                _tabs[i].style.color = isSelected ? DeveloperToolsStyle.TextColor : DeveloperToolsStyle.MutedTextColor;
                _tabs[i].style.borderBottomWidth = isSelected ? TabUnderline : 0f;
            }

            if (IsOpen)
            {
                _pages[index].Refresh(true);
            }
        }

        private void HideHint()
        {
            if (_hint != null)
            {
                _hint.style.display = DisplayStyle.None;
            }
        }

        private void HandleGeometryChanged(GeometryChangedEvent geometryChangedEvent)
        {
            ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            if (_panel == null || _hint == null || _root?.panel == null)
            {
                return;
            }

            var safeArea = Screen.safeArea;
            var safeTopLeft = ToPanel(safeArea.xMin, safeArea.yMax);
            var safeBottomRight = ToPanel(safeArea.xMax, safeArea.yMin);
            var screenBottomRight = ToPanel(Screen.width, 0f);
            _panel.style.paddingLeft = safeTopLeft.x + DeveloperToolsStyle.LargeSpacing;
            _panel.style.paddingTop = safeTopLeft.y + DeveloperToolsStyle.LargeSpacing;
            _panel.style.paddingRight = screenBottomRight.x - safeBottomRight.x + DeveloperToolsStyle.LargeSpacing;
            _panel.style.paddingBottom = screenBottomRight.y - safeBottomRight.y + DeveloperToolsStyle.LargeSpacing;

            var hintTopLeft = ToPanel(_hintCorner.xMin, _hintCorner.yMax);
            var hintBottomRight = ToPanel(_hintCorner.xMax, _hintCorner.yMin);
            _hint.style.left = hintTopLeft.x;
            _hint.style.top = hintTopLeft.y;
            _hint.style.width = hintBottomRight.x - hintTopLeft.x;
            _hint.style.height = hintBottomRight.y - hintTopLeft.y;
        }

        private Vector2 ToPanel(float x, float y)
        {
            return RuntimePanelUtils.ScreenToPanel(_root!.panel, new Vector2(x, Screen.height - y));
        }
    }
}
