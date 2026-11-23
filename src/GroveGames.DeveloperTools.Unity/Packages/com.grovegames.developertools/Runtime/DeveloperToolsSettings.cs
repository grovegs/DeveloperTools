using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class DeveloperToolsSettings : ScriptableObject
    {
        public const string ConfigName = "com.grovegames.developertools.settings";

        private static DeveloperToolsSettings? s_instance;

        [Header("Open")]
        [SerializeField, Range(64, 400)] private int _cornerSize = 160;
        [SerializeField, Min(1)] private int _tapCount = 3;
        [SerializeField, Range(0.1f, 2f)] private float _tapInterval = 0.5f;
        [SerializeField] private Key _toggleKey = Key.Backquote;

        [Header("Appearance")]
        [SerializeField] private ThemeStyleSheet? _theme;
        [SerializeField] private Font? _font;
        [SerializeField] private Font? _boldFont;
        [SerializeField, HideInInspector] private Texture2D? _logo;
        [SerializeField] private Vector2Int _referenceResolution = new(1080, 1920);
        [SerializeField, Range(0, 1000)] private int _sortingOrder = 1000;

        public int CornerSize => _cornerSize;
        public int TapCount => _tapCount;
        public float TapInterval => _tapInterval;
        public Key ToggleKey => _toggleKey;
        public ThemeStyleSheet? Theme => _theme;
        public Font? Font => _font;
        public Font? BoldFont => _boldFont;
        public Texture2D? Logo => _logo;
        public Vector2Int ReferenceResolution => _referenceResolution;
        public int SortingOrder => _sortingOrder;

        public static DeveloperToolsSettings GetOrCreate()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorBuildSettings.TryGetConfigObject(ConfigName, out DeveloperToolsSettings settings) && settings != null)
            {
                return settings;
            }
#else
            if (s_instance == null)
            {
                var preloaded = Resources.FindObjectsOfTypeAll<DeveloperToolsSettings>();
                s_instance = preloaded.Length > 0 ? preloaded[0] : null;
            }
#endif
            if (s_instance == null)
            {
                s_instance = CreateInstance<DeveloperToolsSettings>();
                s_instance.hideFlags = HideFlags.DontSave;
            }

            return s_instance;
        }
    }
}
