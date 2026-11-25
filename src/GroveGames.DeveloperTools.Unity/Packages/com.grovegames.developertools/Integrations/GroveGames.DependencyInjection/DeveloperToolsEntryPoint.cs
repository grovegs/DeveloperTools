using GroveGames.DependencyInjection;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class DeveloperToolsEntryPoint : IFrameUpdatable
    {
        private readonly UnityDeveloperToolsWindow _window;
        private readonly UnityDeveloperToolsTrigger _trigger;

        public DeveloperToolsEntryPoint(UnityDeveloperToolsSettings settings, UnityDeveloperToolsWindow window)
        {
            _window = window;
            _trigger = new UnityDeveloperToolsTrigger(settings, window);
        }

        public void FrameUpdate(DeltaTime deltaTime)
        {
            _trigger.Update();
            _window.Update();
        }
    }
}
