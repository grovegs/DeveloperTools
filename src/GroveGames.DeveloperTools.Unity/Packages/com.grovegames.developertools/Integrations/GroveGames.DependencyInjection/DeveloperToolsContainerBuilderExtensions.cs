using GroveGames.DependencyInjection;
using UnityEngine.UIElements;

namespace GroveGames.DeveloperTools.Unity
{
    public static class DeveloperToolsContainerBuilderExtensions
    {
        public static IContainerBuilder AddDeveloperTools(this IContainerBuilder builder)
        {
            builder.AddSingleton(DeveloperToolsSettings.GetOrCreate());
            builder.AddSingleton<DeveloperOptionsRegistry>();
            builder.AddSingleton<DeveloperToolsPageRegistry<VisualElement>>();
            builder.AddSingleton<DeveloperToolsWindow>();
#if GROVEGAMES_INPUT
            builder.AddSingleton<DeveloperToolsInputBlocker>();
#endif
            return builder.AddSingleton<DeveloperToolsEntryPoint>();
        }

        public static IContainerBuilder AddDeveloperOptions<TOptions>(this IContainerBuilder builder) where TOptions : class, IDeveloperOptions
        {
            builder.AddSingleton<TOptions>();
            return builder.AddSingleton<DeveloperOptionsRegistration<TOptions>>();
        }

        public static IContainerBuilder AddDeveloperToolsPage<TPage>(this IContainerBuilder builder, int order = DeveloperToolsPageRegistry<VisualElement>.DefaultOrder) where TPage : class, IDeveloperToolsPage<VisualElement>
        {
            builder.AddSingleton<TPage>();
            return builder.AddSingleton<DeveloperToolsPageRegistration<TPage>>(resolver => new DeveloperToolsPageRegistration<TPage>(resolver.Resolve<DeveloperToolsPageRegistry<VisualElement>>(), resolver.Resolve<TPage>(), order));
        }
    }
}
