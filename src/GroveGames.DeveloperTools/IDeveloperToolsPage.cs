namespace GroveGames.DeveloperTools;

public interface IDeveloperToolsPage
{
    public string Title { get; }

    public void Refresh(bool force);
}
