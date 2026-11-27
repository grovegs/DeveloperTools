#if GROVEGAMES_INPUT
using System;
using GroveGames.DependencyInjection;
using GroveGames.Input;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class DeveloperToolsInputBlocker : IInitializable, IDisposable, IPointerDownHandler, IKeyDownHandler
    {
        private readonly UnityDeveloperToolsWindow _window;
        private readonly IPointerDispatcher? _pointerDispatcher;
        private readonly IKeyboardDispatcher? _keyboardDispatcher;

        public DeveloperToolsInputBlocker(UnityDeveloperToolsWindow window, IPointerDispatcher pointerDispatcher) : this(window, pointerDispatcher, null)
        {
        }

        public DeveloperToolsInputBlocker(UnityDeveloperToolsWindow window, IPointerDispatcher? pointerDispatcher, IKeyboardDispatcher? keyboardDispatcher)
        {
            _window = window;
            _pointerDispatcher = pointerDispatcher;
            _keyboardDispatcher = keyboardDispatcher;
        }

        public void Initialize()
        {
            _pointerDispatcher?.Register(this, int.MaxValue);
            _keyboardDispatcher?.Register(this, int.MaxValue);
        }

        public void Dispose()
        {
            _pointerDispatcher?.Unregister(this);
            _keyboardDispatcher?.Unregister(this);
        }

        public void HandlePointerDown(ref PointerDownEvent pointerEvent)
        {
            if (_window.IsOpen)
            {
                pointerEvent.MarkHandled();
            }
        }

        public void HandleKeyDown(ref KeyDownEvent keyEvent)
        {
            if (_window.IsOpen)
            {
                keyEvent.MarkHandled();
            }
        }
    }
}
#endif
