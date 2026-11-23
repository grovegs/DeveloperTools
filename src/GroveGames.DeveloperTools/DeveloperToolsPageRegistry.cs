using System.Collections.Generic;

namespace GroveGames.DeveloperTools;

public sealed class DeveloperToolsPageRegistry
{
    public const int DefaultOrder = 1000;

    private readonly List<IDeveloperToolsPage> _pages;
    private readonly List<int> _orders;

    private int _version;
    public int Count => _pages.Count;
    public int Version => _version;

    public DeveloperToolsPageRegistry()
    {
        _pages = new List<IDeveloperToolsPage>();
        _orders = new List<int>();
    }

    public IDeveloperToolsPage Get(int index)
    {
        return _pages[index];
    }

    public void Add(IDeveloperToolsPage page)
    {
        Add(page, DefaultOrder);
    }

    public void Add(IDeveloperToolsPage page, int order)
    {
        if (_pages.Contains(page))
        {
            return;
        }

        var index = _orders.Count;

        while (index > 0 && _orders[index - 1] > order)
        {
            index--;
        }

        _pages.Insert(index, page);
        _orders.Insert(index, order);
        _version++;
    }

    public void Remove(IDeveloperToolsPage page)
    {
        var index = _pages.IndexOf(page);

        if (index < 0)
        {
            return;
        }

        _pages.RemoveAt(index);
        _orders.RemoveAt(index);
        _version++;
    }
}
