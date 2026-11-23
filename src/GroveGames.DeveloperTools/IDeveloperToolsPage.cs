namespace GroveGames.DeveloperTools;

public interface IDeveloperToolsPage<out TRoot>
{
    public string Title { get; }
    public TRoot Root { get; }

    public void Refresh(bool force);
}
