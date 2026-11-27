using GroveGames.DependencyInjection;
#if GROVEGAMES_INPUT
using GroveGames.Input;
#endif
using UnityEngine.UIElements;

namespace GroveGames.DeveloperTools.Unity
{
    public static class DeveloperToolsContainerBuilderExtensions
    {
        public static IContainerBuilder AddDeveloperTools(this IContainerBuilder builder)
        {
            var settings = UnityDeveloperToolsSettings.GetOrCreate();
            builder.AddSingleton(settings);
            builder.AddSingleton<IDeveloperToolsSettings>(settings);
            builder.AddSingleton<DeveloperOptionsRegistry>();
            builder.AddSingleton<IDeveloperToolsPageRegistry<VisualElement>, UnityDeveloperToolsPageRegistry>();
            builder.AddSingleton<UnityDeveloperToolsWindow>();
#if GROVEGAMES_INPUT
            builder.AddSingleton<DeveloperToolsInputBlocker>(resolver =>
            {
                resolver.TryResolve<IPointerDispatcher>(out var pointerDispatcher);
                resolver.TryResolve<IKeyboardDispatcher>(out var keyboardDispatcher);
                return new DeveloperToolsInputBlocker(resolver.Resolve<UnityDeveloperToolsWindow>(), pointerDispatcher, keyboardDispatcher);
            });
#endif
            return builder.AddSingleton<DeveloperToolsEntryPoint>();
        }

        public static IContainerBuilder AddDeveloperOptions<TOptions>(this IContainerBuilder builder) where TOptions : class, IDeveloperOptions
        {
            builder.AddSingleton<TOptions>();
            return builder.AddSingleton<DeveloperOptionsRegistration<TOptions>>();
        }

        public static IContainerBuilder AddDeveloperToolsPage<TPage>(this IContainerBuilder builder, int order = UnityDeveloperToolsPageRegistry.DefaultOrder) where TPage : class, IDeveloperToolsPage<VisualElement>
        {
            builder.AddSingleton<TPage>();
            return builder.AddSingleton<DeveloperToolsPageRegistration<TPage>>(resolver => new DeveloperToolsPageRegistration<TPage>(resolver.Resolve<IDeveloperToolsPageRegistry<VisualElement>>(), resolver.Resolve<TPage>(), order));
        }
    }
}
