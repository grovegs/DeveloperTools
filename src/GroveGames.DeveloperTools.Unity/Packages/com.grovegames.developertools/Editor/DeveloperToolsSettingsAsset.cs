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

        public static DeveloperToolsSettings GetOrCreate()
        {
            if (!EditorBuildSettings.TryGetConfigObject(DeveloperToolsSettings.ConfigName, out DeveloperToolsSettings settings) || settings == null)
            {
                settings = LoadOrCreate();
                EditorBuildSettings.AddConfigObject(DeveloperToolsSettings.ConfigName, settings, true);
            }

            AssignLogo(settings);
            Preload(settings);
            return settings;
        }

        private static DeveloperToolsSettings LoadOrCreate()
        {
            CreateFolder();
            var settings = AssetDatabase.LoadAssetAtPath<DeveloperToolsSettings>(AssetPath);

            if (settings != null)
            {
                return settings;
            }

            var created = ScriptableObject.CreateInstance<DeveloperToolsSettings>();
            var serialized = new SerializedObject(created);
            serialized.FindProperty("_theme").objectReferenceValue = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(ThemePath);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.CreateAsset(created, AssetPath);
            AssetDatabase.SaveAssets();
            return AssetDatabase.LoadAssetAtPath<DeveloperToolsSettings>(AssetPath);
        }

        private static void AssignLogo(DeveloperToolsSettings settings)
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

        private static void Preload(DeveloperToolsSettings settings)
        {
            var preloaded = PlayerSettings.GetPreloadedAssets();
            var assets = new List<Object>(preloaded.Length + 1);
            var isPreloaded = false;

            for (var i = 0; i < preloaded.Length; i++)
            {
                var asset = preloaded[i];

                if (asset is DeveloperToolsSettings && asset != settings)
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
