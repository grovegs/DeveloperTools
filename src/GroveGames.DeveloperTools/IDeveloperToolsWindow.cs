using System.Numerics;

namespace GroveGames.DeveloperTools;

public interface IDeveloperToolsWindow
{
    public bool IsOpen { get; }

    public void Open();
    public void Close();
    public void Toggle();
    public void ShowTapHint(Vector2 corner, float size, int taps, int requiredTaps);
}
