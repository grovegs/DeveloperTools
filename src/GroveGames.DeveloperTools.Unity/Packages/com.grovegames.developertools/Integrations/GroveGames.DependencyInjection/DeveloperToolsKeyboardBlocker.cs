#if GROVEGAMES_INPUT
using System;
using GroveGames.DependencyInjection;
using GroveGames.Input;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class DeveloperToolsKeyboardBlocker : IInitializable, IDisposable, IKeyDownHandler
    {
        private readonly UnityDeveloperToolsWindow _window;
        private readonly IKeyboardDispatcher _keyboardDispatcher;

        public DeveloperToolsKeyboardBlocker(UnityDeveloperToolsWindow window, IKeyboardDispatcher keyboardDispatcher)
        {
            _window = window;
            _keyboardDispatcher = keyboardDispatcher;
        }

        public void Initialize()
        {
            _keyboardDispatcher.Register(this, int.MaxValue);
        }

        public void Dispose()
        {
            _keyboardDispatcher.Unregister(this);
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
