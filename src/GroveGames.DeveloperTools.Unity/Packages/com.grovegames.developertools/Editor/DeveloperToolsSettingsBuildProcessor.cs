using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace GroveGames.DeveloperTools.Unity.Editor
{
    internal sealed class DeveloperToolsSettingsBuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            DeveloperToolsSettingsAsset.GetOrCreate();
        }
    }
}
