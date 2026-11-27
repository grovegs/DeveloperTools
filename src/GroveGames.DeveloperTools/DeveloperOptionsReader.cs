using System;
using System.Collections.Generic;
#if NET
using System.Diagnostics.CodeAnalysis;
#endif
using System.Reflection;
using System.Text;

namespace GroveGames.DeveloperTools;

public static class DeveloperOptionsReader
{
    private const BindingFlags MemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const string ValidateMethodName = "OnValidate";
    private const string TryParseMethodName = "TryParse";
    private const string ReadOnlyProblem = "Read-only fields are not supported";
    private const string ParametersProblem = "Methods with parameters are not supported";
    private const string StaticFieldProblem = "Static fields are not supported";
    private const string StaticMethodProblem = "Static methods are not supported";
    private const string LimitsProblem = "Min is greater than Max";
    private static readonly Dictionary<Type, IReadOnlyList<DeveloperOption>> s_cache = new();

#if NET
    [RequiresUnreferencedCode("Options are read by reflection. Keep the members marked with DeveloperOptionAttribute, OnValidate and TryParse.")]
#endif
    public static IReadOnlyList<DeveloperOption> Read(Type type)
    {
        if (s_cache.TryGetValue(type, out var cached))
        {
            return cached;
        }

        var options = new List<DeveloperOption>();
        var hierarchy = new List<Type>();

        for (var current = type; current != null && current != typeof(object); current = current.BaseType)
        {
            hierarchy.Add(current);
        }

        MethodInfo? validate = null;

        for (var i = 0; i < hierarchy.Count && validate == null; i++)
        {
            validate = hierarchy[i].GetMethod(ValidateMethodName, MemberFlags, null, Type.EmptyTypes, null);
        }

        string? header = null;

        for (var i = hierarchy.Count - 1; i >= 0; i--)
        {
            foreach (var field in hierarchy[i].GetFields(MemberFlags | BindingFlags.Static))
            {
                var attribute = field.GetCustomAttribute<DeveloperOptionAttribute>();

                if (attribute == null)
                {
                    continue;
                }

                if (attribute.Header != null && !field.IsStatic)
                {
                    header = attribute.Header;
                }

                var label = attribute.Label ?? Nicify(field.Name);
                var tryParse = FindTryParse(field.FieldType);
                var kind = GetKind(field, attribute, tryParse, out var min, out var max);
                var problem = GetProblem(field, kind, min, max);
                options.Add(new DeveloperOption(kind, label, header, attribute.Tooltip, field, min, max, validate, tryParse, problem));
            }
        }

        for (var i = hierarchy.Count - 1; i >= 0; i--)
        {
            foreach (var method in hierarchy[i].GetMethods(MemberFlags | BindingFlags.Static))
            {
                var attribute = method.GetCustomAttribute<DeveloperOptionAttribute>();

                if (attribute == null)
                {
                    continue;
                }

                if (attribute.Header != null && !method.IsStatic)
                {
                    header = attribute.Header;
                }

                var label = attribute.Label ?? Nicify(method.Name);
                var problem = method.IsStatic ? StaticMethodProblem : method.GetParameters().Length > 0 ? ParametersProblem : null;
                options.Add(new DeveloperOption(label, header, attribute.Tooltip, method, problem));
            }
        }

        s_cache.Add(type, options);
        return options;
    }

    public static string Nicify(string name)
    {
        var start = 0;

        if (name.StartsWith("m_", StringComparison.Ordinal) || name.StartsWith("s_", StringComparison.Ordinal))
        {
            start = 2;
        }

        while (start < name.Length && name[start] == '_')
        {
            start++;
        }

        var builder = new StringBuilder(name.Length + 4);

        for (var i = start; i < name.Length; i++)
        {
            var character = name[i];

            if (character == '_')
            {
                builder.Append(' ');
                continue;
            }

            if (builder.Length == 0)
            {
                builder.Append(char.ToUpperInvariant(character));
                continue;
            }

            var previous = name[i - 1];

            if (char.IsUpper(character) && (char.IsLower(previous) || char.IsDigit(previous)))
            {
                builder.Append(' ');
            }

            builder.Append(character);
        }

        return builder.ToString();
    }

    private static string? GetProblem(FieldInfo field, DeveloperOptionKind kind, float min, float max)
    {
        if (field.IsStatic)
        {
            return StaticFieldProblem;
        }

        if (field.IsInitOnly)
        {
            return ReadOnlyProblem;
        }

        if (kind == DeveloperOptionKind.Unsupported)
        {
            return $"{field.FieldType.Name} is not supported";
        }

        return min > max ? LimitsProblem : null;
    }

#if NET
    [RequiresUnreferencedCode("Options are read by reflection. Keep the members marked with DeveloperOptionAttribute, OnValidate and TryParse.")]
#endif
    private static MethodInfo? FindTryParse(Type type)
    {
        var method = type.GetMethod(TryParseMethodName, BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string), type.MakeByRefType() }, null);
        return method != null && method.ReturnType == typeof(bool) ? method : null;
    }

    private static DeveloperOptionKind GetKind(FieldInfo field, DeveloperOptionAttribute attribute, MethodInfo? tryParse, out float min, out float max)
    {
        var type = field.FieldType;
        min = float.NegativeInfinity;
        max = float.PositiveInfinity;

        if (field.IsStatic || field.IsInitOnly)
        {
            return DeveloperOptionKind.Unsupported;
        }

        min = attribute.Min;
        max = attribute.Max;

        if (type == typeof(bool))
        {
            return DeveloperOptionKind.Toggle;
        }

        if (type == typeof(int) || type == typeof(float))
        {
            if (!float.IsInfinity(attribute.Min) && !float.IsInfinity(attribute.Max))
            {
                return DeveloperOptionKind.Slider;
            }

            return type == typeof(int) ? DeveloperOptionKind.Integer : DeveloperOptionKind.Float;
        }

        if (type == typeof(string))
        {
            return DeveloperOptionKind.Text;
        }

        if (type.IsEnum)
        {
            return DeveloperOptionKind.Enum;
        }

        return tryParse != null ? DeveloperOptionKind.Parsable : DeveloperOptionKind.Unsupported;
    }
}
