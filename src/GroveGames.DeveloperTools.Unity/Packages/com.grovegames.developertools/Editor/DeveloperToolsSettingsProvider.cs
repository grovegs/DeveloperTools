using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GroveGames.DeveloperTools.Unity.Editor
{
    internal static class DeveloperToolsSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new SettingsProvider("Project/GroveGames/Developer Tools", SettingsScope.Project)
            {
                label = "Developer Tools",
                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = DeveloperToolsSettingsAsset.GetOrCreate();
                    var serializedObject = new SerializedObject(settings);

                    var container = new VisualElement
                    {
                        style =
                        {
                            paddingLeft = 10,
                            paddingRight = 10,
                            paddingTop = 10,
                            paddingBottom = 10
                        }
                    };

                    container.Add(new Label("Developer Tools Settings")
                    {
                        style =
                        {
                            fontSize = 19,
                            unityFontStyleAndWeight = FontStyle.Bold,
                            marginBottom = 10
                        }
                    });

                    var assetField = new ObjectField("Settings Asset")
                    {
                        objectType = typeof(UnityDeveloperToolsSettings),
                        value = settings,
                        style = { marginBottom = 10 }
                    };
                    assetField.SetEnabled(false);
                    container.Add(assetField);

                    container.Add(new PropertyField(serializedObject.FindProperty("_cornerSize"), "Corner Size"));
                    container.Add(new PropertyField(serializedObject.FindProperty("_tapCount"), "Tap Count"));
                    container.Add(new PropertyField(serializedObject.FindProperty("_tapInterval"), "Tap Interval"));
                    container.Add(new PropertyField(serializedObject.FindProperty("_toggleKey"), "Toggle Key"));
                    container.Add(new PropertyField(serializedObject.FindProperty("_theme"), "Theme"));
                    container.Add(new PropertyField(serializedObject.FindProperty("_font"), "Font"));
                    container.Add(new PropertyField(serializedObject.FindProperty("_boldFont"), "Bold Font"));
                    container.Add(new PropertyField(serializedObject.FindProperty("_referenceResolution"), "Reference Resolution"));
                    container.Add(new PropertyField(serializedObject.FindProperty("_sortingOrder"), "Sorting Order"));

                    rootElement.Add(container);
                    rootElement.Bind(serializedObject);
                },
                keywords = new HashSet<string>(new[] { "Developer", "Tools", "Logs", "Debug", "Console", "Grove Games" })
            };
        }
    }
}
