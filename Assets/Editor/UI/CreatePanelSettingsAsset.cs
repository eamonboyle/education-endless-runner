using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MathRunner.Editor.UI
{
    /// <summary>
    /// Creates PanelSettings + ensures the runtime Theme Style Sheet is assigned.
    /// Menu: Math Runner / UI / Create Panel Settings
    /// </summary>
    public static class CreatePanelSettingsAsset
    {
        private const string AssetPath = "Assets/UI/PanelSettings/MathRunnerPanelSettings.asset";
        private const string ResourcesPanelPath = "Assets/Resources/UI/PanelSettings/MathRunnerPanelSettings.asset";
        private const string ThemePath = "Assets/Resources/UI/Themes/MathRunner.tss";
        private const string ThemePathAuthoring = "Assets/UI/Themes/MathRunner.tss";

        [MenuItem("Math Runner/UI/Create Panel Settings")]
        public static void Create()
        {
            EnsureFolders();

            var theme = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(ThemePath);
            if (theme == null)
                theme = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(ThemePathAuthoring);

            if (theme == null)
            {
                Debug.LogError(
                    "Could not find MathRunner.tss. Expected at " + ThemePath +
                    ". Create a Theme Style Sheet that @imports unity-theme://default.");
                return;
            }

            WritePanelSettings(AssetPath, theme);
            WritePanelSettings(ResourcesPanelPath, theme);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var loaded = AssetDatabase.LoadAssetAtPath<PanelSettings>(ResourcesPanelPath);
            Selection.activeObject = loaded;
            Debug.Log("Created/updated PanelSettings with theme: " + theme.name);
        }

        private static void WritePanelSettings(string path, ThemeStyleSheet theme)
        {
            var settings = AssetDatabase.LoadAssetAtPath<PanelSettings>(path);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<PanelSettings>();
                AssetDatabase.CreateAsset(settings, path);
            }

            settings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            settings.referenceResolution = new Vector2Int(1080, 1920);
            settings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            settings.match = 0.5f;
            settings.sortingOrder = 100;
            settings.themeStyleSheet = theme;
            // Overlay on top of the camera — clearing the color buffer blacks out the 3D scene.
            settings.clearColor = false;
            EditorUtility.SetDirty(settings);
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets/UI", "PanelSettings");
            EnsureFolder("Assets/Resources", "UI");
            EnsureFolder("Assets/Resources/UI", "PanelSettings");
            EnsureFolder("Assets/Resources/UI", "Themes");
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, child);
        }
    }
}
