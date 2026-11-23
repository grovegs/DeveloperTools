using GroveGames.DependencyInjection;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class DeveloperToolsEntryPoint : IFrameUpdatable
    {
        private readonly DeveloperToolsWindow _window;
        private readonly DeveloperToolsTrigger _trigger;

        public DeveloperToolsEntryPoint(DeveloperToolsSettings settings, DeveloperToolsWindow window)
        {
            _window = window;
            _trigger = new DeveloperToolsTrigger(settings, window);
        }

        public void FrameUpdate(float deltaTime)
        {
            _trigger.Update();
            _window.Update();
        }
    }
}
