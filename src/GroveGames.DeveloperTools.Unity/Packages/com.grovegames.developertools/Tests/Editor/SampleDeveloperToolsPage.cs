using UnityEngine.UIElements;

namespace GroveGames.DeveloperTools.Unity.Editor.Tests
{
    public sealed class SampleDeveloperToolsPage : IDeveloperToolsPage
    {
        private readonly VisualElement _root;

        public string Title => "Sample";
        public VisualElement Root => _root;

        public SampleDeveloperToolsPage()
        {
            _root = new VisualElement();
        }

        public void Refresh(bool force)
        {
        }
    }
}
