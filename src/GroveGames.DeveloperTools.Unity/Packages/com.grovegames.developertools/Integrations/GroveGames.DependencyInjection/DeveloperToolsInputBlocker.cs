#if GROVEGAMES_INPUT
using System;
using GroveGames.DependencyInjection;
using GroveGames.Input.Unity;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class DeveloperToolsInputBlocker : IInitializable, IDisposable, IPointerDownHandler
    {
        private readonly UnityDeveloperToolsWindow _window;
        private readonly IPointerDispatcher _pointerDispatcher;

        public DeveloperToolsInputBlocker(UnityDeveloperToolsWindow window, IPointerDispatcher pointerDispatcher)
        {
            _window = window;
            _pointerDispatcher = pointerDispatcher;
        }

        public void Initialize()
        {
            _pointerDispatcher.Register(this, int.MaxValue);
        }

        public void Dispose()
        {
            _pointerDispatcher.Unregister(this);
        }

        public void HandlePointerDown(ref PointerDownEvent pointerEvent)
        {
            if (_window.IsOpen)
            {
                pointerEvent.MarkHandled();
            }
        }
    }
}
#endif
