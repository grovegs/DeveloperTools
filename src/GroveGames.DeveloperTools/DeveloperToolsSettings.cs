namespace GroveGames.DeveloperTools;

public sealed class DeveloperToolsSettings : IDeveloperToolsSettings
{
    private readonly int _cornerSize;
    private readonly int _tapCount;
    private readonly float _tapInterval;

    public int CornerSize => _cornerSize;
    public int TapCount => _tapCount;
    public float TapInterval => _tapInterval;

    public DeveloperToolsSettings(int cornerSize, int tapCount, float tapInterval)
    {
        _cornerSize = cornerSize;
        _tapCount = tapCount;
        _tapInterval = tapInterval;
    }
}
