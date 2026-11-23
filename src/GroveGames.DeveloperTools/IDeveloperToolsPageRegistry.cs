namespace GroveGames.DeveloperTools;

public interface IDeveloperToolsPageRegistry<TRoot>
{
    public int Count { get; }
    public int Version { get; }

    public IDeveloperToolsPage<TRoot> Get(int index);
    public void Add(IDeveloperToolsPage<TRoot> page);
    public void Add(IDeveloperToolsPage<TRoot> page, int order);
    public void Remove(IDeveloperToolsPage<TRoot> page);
}
