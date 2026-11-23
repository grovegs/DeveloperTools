using System;
using GroveGames.DependencyInjection;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class DeveloperOptionsRegistration<TOptions> : IInitializable, IDisposable where TOptions : class, IDeveloperOptions
    {
        private readonly DeveloperOptionsRegistry _registry;
        private readonly TOptions _options;

        public DeveloperOptionsRegistration(DeveloperOptionsRegistry registry, TOptions options)
        {
            _registry = registry;
            _options = options;
        }

        public void Initialize()
        {
            _registry.Add(_options);
        }

        public void Dispose()
        {
            _registry.Remove(_options);
        }
    }
}
