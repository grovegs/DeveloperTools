using System;
using System.Reflection;

namespace GroveGames.DeveloperTools;

public sealed class DeveloperOption
{
    private readonly FieldInfo? _field;
    private readonly MethodInfo? _method;
    private readonly MethodInfo? _validate;
    private readonly MethodInfo? _tryParse;

    private readonly DeveloperOptionKind _kind;
    private readonly string _label;
    private readonly string? _header;
    private readonly string? _tooltip;
    private readonly Type _valueType;
    private readonly float _min;
    private readonly float _max;
    private readonly string? _problem;
    public DeveloperOptionKind Kind => _kind;
    public string Label => _label;
    public string? Header => _header;
    public string? Tooltip => _tooltip;
    public Type ValueType => _valueType;
    public float Min => _min;
    public float Max => _max;
    public string? Problem => _problem;

    public DeveloperOption(DeveloperOptionKind kind, string label, string? header, string? tooltip, FieldInfo field, float min, float max, MethodInfo? validate, MethodInfo? tryParse, string? problem)
    {
        _tryParse = tryParse;
        _kind = kind;
        _problem = problem;
        _label = label;
        _header = header;
        _tooltip = tooltip;
        _valueType = field.FieldType;
        _min = min;
        _max = max;
        _field = field;
        _validate = validate;
    }

    public DeveloperOption(string label, MethodInfo method, string? problem)
    {
        _kind = problem == null ? DeveloperOptionKind.Action : DeveloperOptionKind.Unsupported;
        _problem = problem;
        _label = label;
        _valueType = typeof(void);
        _min = float.NegativeInfinity;
        _max = float.PositiveInfinity;
        _method = method;
    }

    public object? GetValue(object target)
    {
        return _field?.GetValue(target);
    }

    public void SetValue(object target, object? value)
    {
        if (_field == null)
        {
            return;
        }

        _field.SetValue(target, value);
        _validate?.Invoke(target, null);
    }

    public bool TryParse(string text, out object? value)
    {
        value = null;

        if (_tryParse == null)
        {
            return false;
        }

        var arguments = new object?[] { text, null };

        if (_tryParse.Invoke(null, arguments) is not true)
        {
            return false;
        }

        value = arguments[1];
        return true;
    }

    public void Invoke(object target)
    {
        _method?.Invoke(target, null);
    }
}
