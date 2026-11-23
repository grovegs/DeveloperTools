using UnityEngine.UIElements;

namespace GroveGames.DeveloperTools.Unity
{
    public interface IDeveloperToolsElementPage : IDeveloperToolsPage
    {
        VisualElement Root { get; }
    }
}
