using UnityEngine.UIElements;

namespace GroveGames.DeveloperTools.Unity
{
    public interface IDeveloperToolsPage
    {
        string Title { get; }
        VisualElement Root { get; }

        void Refresh(bool force);
    }
}
