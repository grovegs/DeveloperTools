using UnityEngine;
using UnityEngine.InputSystem;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class UnityDeveloperToolsTrigger
    {
        private readonly DeveloperToolsSettings _settings;
        private readonly DeveloperToolsTrigger _trigger;

        public UnityDeveloperToolsTrigger(DeveloperToolsSettings settings, IDeveloperToolsWindow window)
        {
            _settings = settings;
            _trigger = new DeveloperToolsTrigger(window, settings.TapCount, settings.TapInterval);
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

            _trigger.PressToggleKey();
        }

        private void UpdatePointer()
        {
            var pointer = Pointer.current;

            if (pointer == null || !pointer.press.wasPressedThisFrame)
            {
                return;
            }

            var position = pointer.position.ReadValue();
            var size = _settings.CornerSize * Screen.width / _settings.ReferenceResolution.x;
            var safeArea = Screen.safeArea;
            _trigger.PressPointer(new System.Numerics.Vector2(position.x, position.y), safeArea.xMin, safeArea.yMax, size, Time.unscaledTime);
        }
    }
}
