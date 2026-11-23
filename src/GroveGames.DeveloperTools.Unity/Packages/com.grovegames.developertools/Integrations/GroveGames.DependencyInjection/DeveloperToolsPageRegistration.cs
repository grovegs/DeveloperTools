using System;
using GroveGames.DependencyInjection;
using UnityEngine.UIElements;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class DeveloperToolsPageRegistration<TPage> : IInitializable, IDisposable where TPage : class, IDeveloperToolsPage<VisualElement>
    {
        private readonly DeveloperToolsPageRegistry<VisualElement> _registry;
        private readonly TPage _page;
        private readonly int _order;

        public DeveloperToolsPageRegistration(DeveloperToolsPageRegistry<VisualElement> registry, TPage page, int order)
        {
            _registry = registry;
            _page = page;
            _order = order;
        }

        public void Initialize()
        {
            _registry.Add(_page, _order);
        }

        public void Dispose()
        {
            _registry.Remove(_page);
        }
    }
}
