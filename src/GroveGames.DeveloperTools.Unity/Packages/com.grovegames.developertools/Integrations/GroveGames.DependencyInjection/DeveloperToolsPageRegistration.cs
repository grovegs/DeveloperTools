using System;
using GroveGames.DependencyInjection;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class DeveloperToolsPageRegistration<TPage> : IInitializable, IDisposable where TPage : class, IDeveloperToolsPage
    {
        private readonly DeveloperToolsPageRegistry _registry;
        private readonly TPage _page;
        private readonly int _order;

        public DeveloperToolsPageRegistration(DeveloperToolsPageRegistry registry, TPage page, int order)
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
