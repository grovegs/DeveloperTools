using System.Numerics;

namespace GroveGames.DeveloperTools;

public sealed class DeveloperToolsTrigger
{
    private readonly IDeveloperToolsWindow _window;
    private readonly CornerTapGesture _gesture;

    public DeveloperToolsTrigger(IDeveloperToolsWindow window, IDeveloperToolsSettings settings)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(settings);

        _window = window;
        _gesture = new CornerTapGesture(settings.TapCount, settings.TapInterval);
    }

    public void PressToggleKey()
    {
        _window.Toggle();
    }

    public void PressPointer(Vector2 position, float safeAreaLeft, float safeAreaTop, float cornerSize, float time)
    {
        if (_window.IsOpen)
        {
            return;
        }

        var corner = CornerTapGesture.GetCorner(safeAreaLeft, safeAreaTop, cornerSize);

        if (_gesture.Tap(position, corner, time))
        {
            _window.Open();
            return;
        }

        _window.ShowTapHint(corner, cornerSize, _gesture.Taps, _gesture.RequiredTaps);
    }
}
