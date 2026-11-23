using System.Numerics;

namespace GroveGames.DeveloperTools;

public sealed class CornerTapGesture
{
    private readonly int _tapCount;
    private readonly float _tapInterval;
    private int _taps;
    private float _lastTapTime;

    public int Taps => _taps;
    public int RequiredTaps => _tapCount;

    public CornerTapGesture(int tapCount, float tapInterval)
    {
        _tapCount = Math.Max(1, tapCount);
        _tapInterval = tapInterval;
    }

    public static Vector2 GetCorner(float safeAreaLeft, float safeAreaTop, float size)
    {
        return new Vector2(safeAreaLeft + size, safeAreaTop - size);
    }

    public bool Tap(Vector2 position, Vector2 corner, float time)
    {
        if (position.X > corner.X || position.Y < corner.Y)
        {
            _taps = 0;
            return false;
        }

        if (_taps > 0 && time - _lastTapTime > _tapInterval)
        {
            _taps = 0;
        }

        _taps++;
        _lastTapTime = time;

        if (_taps < _tapCount)
        {
            return false;
        }

        _taps = 0;
        return true;
    }
}
