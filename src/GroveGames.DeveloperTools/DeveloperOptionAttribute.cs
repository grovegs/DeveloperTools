using GroveGames.DeveloperTools.Scripting;

namespace GroveGames.DeveloperTools;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
public sealed class DeveloperOptionAttribute : PreserveAttribute
{
    private readonly string? _label;
    private string? _header;
    private string? _tooltip;
    private float _min;
    private float _max;

    public string? Label => _label;

    public string? Header
    {
        get => _header;
        set => _header = value;
    }

    public string? Tooltip
    {
        get => _tooltip;
        set => _tooltip = value;
    }

    public float Min
    {
        get => _min;
        set => _min = value;
    }

    public float Max
    {
        get => _max;
        set => _max = value;
    }

    public DeveloperOptionAttribute(string? label = null)
    {
        _label = label;
        _min = float.NegativeInfinity;
        _max = float.PositiveInfinity;
    }
}
