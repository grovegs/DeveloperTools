using UnityEngine.UIElements;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class UnityDeveloperToolsPageRegistry : IDeveloperToolsPageRegistry<VisualElement>
    {
        public const int DefaultOrder = DeveloperToolsPageRegistry<VisualElement>.DefaultOrder;

        private readonly DeveloperToolsPageRegistry<VisualElement> _registry;

        public int Count => _registry.Count;
        public int Version => _registry.Version;

        public UnityDeveloperToolsPageRegistry()
        {
            _registry = new DeveloperToolsPageRegistry<VisualElement>();
        }

        public IDeveloperToolsPage<VisualElement> Get(int index)
        {
            return _registry.Get(index);
        }

        public void Add(IDeveloperToolsPage<VisualElement> page)
        {
            _registry.Add(page);
        }

        public void Add(IDeveloperToolsPage<VisualElement> page, int order)
        {
            _registry.Add(page, order);
        }

        public void Remove(IDeveloperToolsPage<VisualElement> page)
        {
            _registry.Remove(page);
        }
    }
}
