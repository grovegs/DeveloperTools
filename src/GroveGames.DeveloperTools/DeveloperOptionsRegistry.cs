using System.Collections.Generic;

namespace GroveGames.DeveloperTools;

public sealed class DeveloperOptionsRegistry
{
    private readonly List<IDeveloperOptions> _options;

    private int _version;
    public int Count => _options.Count;
    public int Version => _version;

    public DeveloperOptionsRegistry()
    {
        _options = new List<IDeveloperOptions>();
    }

    public IDeveloperOptions Get(int index)
    {
        return _options[index];
    }

    public void Add(IDeveloperOptions options)
    {
        if (_options.Contains(options))
        {
            return;
        }

        _options.Add(options);
        _version++;
    }

    public void Remove(IDeveloperOptions options)
    {
        if (!_options.Remove(options))
        {
            return;
        }

        _version++;
    }
}
