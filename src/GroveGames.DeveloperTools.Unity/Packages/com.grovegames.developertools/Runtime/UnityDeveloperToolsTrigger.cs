using UnityEngine;
using UnityEngine.InputSystem;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class UnityDeveloperToolsTrigger
    {
        private readonly UnityDeveloperToolsSettings _settings;
        private readonly IDeveloperToolsWindow _window;
        private readonly DeveloperToolsTrigger _trigger;

        public UnityDeveloperToolsTrigger(UnityDeveloperToolsSettings settings, IDeveloperToolsWindow window)
        {
            _settings = settings;
            _window = window;
            _trigger = new DeveloperToolsTrigger(window, settings);
        }

        public void Update()
        {
            UpdateKeyboard();
            UpdatePointer();
        }

        private void UpdateKeyboard()
        {
            var keyboard = Keyboard.current;

            if (keyboard == null || _settings.ToggleKey == Key.None || !keyboard[_settings.ToggleKey].wasPressedThisFrame || _window is UnityDeveloperToolsWindow { IsEditingText: true })
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
            var reference = _settings.ReferenceResolution;
            var scale = Mathf.Min(Screen.width / (float)reference.x, Screen.height / (float)reference.y);
            var size = _settings.CornerSize * scale;
            var safeArea = Screen.safeArea;
            _trigger.PressPointer(new System.Numerics.Vector2(position.x, position.y), safeArea.xMin, safeArea.yMax, size, Time.unscaledTime);
        }
    }
}
