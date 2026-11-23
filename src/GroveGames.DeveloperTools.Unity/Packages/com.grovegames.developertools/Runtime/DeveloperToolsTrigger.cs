using UnityEngine;
using UnityEngine.InputSystem;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class DeveloperToolsTrigger
    {
        private readonly DeveloperToolsSettings _settings;
        private readonly DeveloperToolsWindow _window;
        private readonly CornerTapGesture _gesture;

        public DeveloperToolsTrigger(DeveloperToolsSettings settings, DeveloperToolsWindow window)
        {
            _settings = settings;
            _window = window;
            _gesture = new CornerTapGesture(settings.TapCount, settings.TapInterval);
        }

        public void Update()
        {
            UpdateKeyboard();
            UpdatePointer();
        }

        private void UpdateKeyboard()
        {
            var keyboard = Keyboard.current;

            if (keyboard == null || _settings.ToggleKey == Key.None || !keyboard[_settings.ToggleKey].wasPressedThisFrame)
            {
                return;
            }

            _window.Toggle();
        }

        private void UpdatePointer()
        {
            var pointer = Pointer.current;

            if (pointer == null || _window.IsOpen || !pointer.press.wasPressedThisFrame)
            {
                return;
            }

            var position = pointer.position.ReadValue();
            var size = _settings.CornerSize * Screen.width / _settings.ReferenceResolution.x;
            var safeArea = Screen.safeArea;
            var corner = CornerTapGesture.GetCorner(safeArea.xMin, safeArea.yMax, size);

            if (_gesture.Tap(new System.Numerics.Vector2(position.x, position.y), corner, Time.unscaledTime))
            {
                _window.Open();
                return;
            }

            _window.ShowTapHint(new Rect(safeArea.xMin, safeArea.yMax - size, size, size), _gesture.Taps, _gesture.RequiredTaps);
        }
    }
}
