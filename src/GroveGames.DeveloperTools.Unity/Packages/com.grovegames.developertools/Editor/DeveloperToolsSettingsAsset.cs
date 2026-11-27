using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GroveGames.DeveloperTools.Unity.Editor
{
    internal static class DeveloperToolsSettingsAsset
    {
        public const string AssetPath = "Assets/Settings/GroveGames/DeveloperToolsSettings.asset";

        private const string ThemePath = "Packages/com.grovegames.developertools/Runtime/Themes/DeveloperToolsTheme.tss";
        private const string LogoPath = "Packages/com.grovegames.developertools/Runtime/Icons/GroveGamesLogo.png";

        [InitializeOnLoadMethod]
        private static void CreateOnLoad()
        {
            EditorApplication.delayCall += HandleDelayCall;
        }

        private static void HandleDelayCall()
        {
            GetOrCreate();
        }

        public static UnityDeveloperToolsSettings GetOrCreate()
        {
            if (!EditorBuildSettings.TryGetConfigObject(UnityDeveloperToolsSettings.ConfigName, out UnityDeveloperToolsSettings settings) || settings == null)
            {
                settings = LoadOrCreate();
                EditorBuildSettings.AddConfigObject(UnityDeveloperToolsSettings.ConfigName, settings, true);
            }

            AssignTheme(settings);
            AssignLogo(settings);
            Preload(settings);
            return settings;
        }

        private static UnityDeveloperToolsSettings LoadOrCreate()
        {
            CreateFolder();
            var settings = AssetDatabase.LoadAssetAtPath<UnityDeveloperToolsSettings>(AssetPath);

            if (settings != null)
            {
                return settings;
            }

            var created = ScriptableObject.CreateInstance<UnityDeveloperToolsSettings>();
            AssetDatabase.CreateAsset(created, AssetPath);
            AssetDatabase.SaveAssets();
            return AssetDatabase.LoadAssetAtPath<UnityDeveloperToolsSettings>(AssetPath);
        }

        private static void AssignTheme(UnityDeveloperToolsSettings settings)
        {
            if (settings.Theme != null)
            {
                return;
            }

            var theme = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(ThemePath);

            if (theme == null)
            {
                return;
            }

            var serialized = new SerializedObject(settings);
            serialized.FindProperty("_theme").objectReferenceValue = theme;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssetIfDirty(settings);
        }

        private static void AssignLogo(UnityDeveloperToolsSettings settings)
        {
            var logo = AssetDatabase.LoadAssetAtPath<Texture2D>(LogoPath);

            if (settings.Logo == logo)
            {
                return;
            }

            var serialized = new SerializedObject(settings);
            serialized.FindProperty("_logo").objectReferenceValue = logo;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssetIfDirty(settings);
        }

        private static void Preload(UnityDeveloperToolsSettings settings)
        {
            var preloaded = PlayerSettings.GetPreloadedAssets();
            var assets = new List<Object>(preloaded.Length + 1);
            var isPreloaded = false;

            for (var i = 0; i < preloaded.Length; i++)
            {
                var asset = preloaded[i];

                if (asset is UnityDeveloperToolsSettings && asset != settings)
                {
                    continue;
                }

                isPreloaded |= asset == settings;
                assets.Add(asset);
            }

            if (!isPreloaded)
            {
                assets.Add(settings);
            }

            if (isPreloaded && assets.Count == preloaded.Length)
            {
                return;
            }

            PlayerSettings.SetPreloadedAssets(assets.ToArray());
        }

        private static void CreateFolder()
        {
            var current = "Assets";

            foreach (var part in AssetPath.Substring("Assets/".Length).Split('/'))
            {
                if (part.EndsWith(".asset", StringComparison.Ordinal))
                {
                    return;
                }

                var next = current + "/" + part;

                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, part);
                }

                current = next;
            }
        }
    }
}
