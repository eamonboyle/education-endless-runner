using UnityEngine;
using UnityEngine.UIElements;

namespace MathRunner.UI.Toolkit
{
    /// <summary>
    /// Layer identifiers for the five shared UIDocuments on UIRoot.
    /// Sort order is applied so all layers share one PanelSettings draw batch.
    /// </summary>
    public enum UILayer
    {
        Hud = 0,
        Overlay = 10,
        Modal = 20,
        Toast = 30,
        Transition = 100
    }

    /// <summary>
    /// Creates the persistent UI Toolkit root with five layered UIDocuments,
    /// a shared PanelSettings (1080x1920 portrait), and applies the design-system
    /// style sheets. Instantiated by GameBootstrap / UIRouter.
    /// </summary>
    public class UIRoot : MonoBehaviour
    {
        public static UIRoot Instance { get; private set; }

        public PanelSettings PanelSettings { get; private set; }
        public UIDocument HudDocument { get; private set; }
        public UIDocument OverlayDocument { get; private set; }
        public UIDocument ModalDocument { get; private set; }
        public UIDocument ToastDocument { get; private set; }
        public UIDocument TransitionDocument { get; private set; }

        private StyleSheet tokensSheet;
        private StyleSheet componentsSheet;
        private StyleSheet accessibilitySheet;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Build();
        }

        private void Build()
        {
            PanelSettings = CreatePanelSettings();
            LoadStyleSheets();

            HudDocument = CreateDocument("HudDocument", UILayer.Hud, "UI/Screens/hud");
            OverlayDocument = CreateDocument("OverlayDocument", UILayer.Overlay, "UI/Screens/overlay");
            ModalDocument = CreateDocument("ModalDocument", UILayer.Modal, null);
            ToastDocument = CreateDocument("ToastDocument", UILayer.Toast, "UI/Screens/toast");
            TransitionDocument = CreateDocument("TransitionDocument", UILayer.Transition, "UI/Screens/transition");

            ApplyTheme(HudDocument);
            ApplyTheme(OverlayDocument);
            ApplyTheme(ModalDocument);
            ApplyTheme(ToastDocument);
            ApplyTheme(TransitionDocument);

            ApplySafeArea(HudDocument);
            ApplySafeArea(ModalDocument);
        }

        private static PanelSettings CreatePanelSettings()
        {
            // Prefer a pre-authored asset if the user created one via the editor menu.
            var fromResources = Resources.Load<PanelSettings>("UI/PanelSettings/MathRunnerPanelSettings");
            if (fromResources != null && fromResources.themeStyleSheet != null)
            {
                // Do NOT clear the color buffer — that wipes the camera every frame.
                // Transparency comes from clear VisualElement backgrounds instead.
                fromResources.clearColor = false;
                return fromResources;
            }

            var settings = ScriptableObject.CreateInstance<PanelSettings>();
            settings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            settings.referenceResolution = new Vector2Int(1080, 1920);
            settings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            settings.match = 0.5f;
            settings.sortingOrder = 100;
            settings.name = "MathRunnerPanelSettings";

            // Overlay UI on top of the 3D camera — never clear the game framebuffer.
            settings.clearColor = false;

            // Required: without a Theme Style Sheet, UI Toolkit will not render controls.
            var theme = Resources.Load<ThemeStyleSheet>("UI/Themes/MathRunner");
            if (theme != null)
            {
                settings.themeStyleSheet = theme;
            }
            else
            {
                Debug.LogError(
                    "[UIRoot] Missing ThemeStyleSheet at Resources/UI/Themes/MathRunner.tss. " +
                    "UI will not render properly. Reimport the .tss or run Math Runner → UI → Create Panel Settings.");
            }

            return settings;
        }

        private void LoadStyleSheets()
        {
            tokensSheet = Resources.Load<StyleSheet>("UI/Styles/tokens");
            componentsSheet = Resources.Load<StyleSheet>("UI/Styles/components");
            accessibilitySheet = Resources.Load<StyleSheet>("UI/Styles/accessibility");
        }

        private UIDocument CreateDocument(string name, UILayer layer, string uxmlResource)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            var doc = go.AddComponent<UIDocument>();
            doc.panelSettings = PanelSettings;
            doc.sortingOrder = (int)layer;

            if (!string.IsNullOrEmpty(uxmlResource))
            {
                var asset = Resources.Load<VisualTreeAsset>(uxmlResource);
                if (asset != null)
                    doc.visualTreeAsset = asset;
                else
                    Debug.LogWarning($"[UIRoot] Missing UXML resource: {uxmlResource}");
            }

            return doc;
        }

        public void ApplyTheme(UIDocument document)
        {
            if (document == null) return;
            var root = document.rootVisualElement;
            if (root == null) return;

            root.style.flexGrow = 1;
            root.style.width = Length.Percent(100);
            root.style.height = Length.Percent(100);
            root.style.backgroundColor = Color.clear;

            if (tokensSheet != null && !root.styleSheets.Contains(tokensSheet))
                root.styleSheets.Add(tokensSheet);
            if (componentsSheet != null && !root.styleSheets.Contains(componentsSheet))
                root.styleSheets.Add(componentsSheet);
            if (accessibilitySheet != null && !root.styleSheets.Contains(accessibilitySheet))
                root.styleSheets.Add(accessibilitySheet);

            // This is a touch-first UI. Unity's keyboard focus skin replaces our
            // nine-sliced artwork with a large theme highlight after a tap.
            // Buttons remain clickable, but do not retain keyboard focus.
            root.Query<Button>().ForEach(button => button.focusable = false);
        }

        public void ApplySafeArea(UIDocument document)
        {
            if (document == null) return;
            var root = document.rootVisualElement;
            if (root == null) return;

            var safe = root.Q("safe-area") ?? root;
            ApplySafeAreaPadding(safe);
        }

        public static void ApplySafeAreaPadding(VisualElement element)
        {
            if (element == null) return;

            Rect safe = Screen.safeArea;
            float w = Screen.width > 0 ? Screen.width : 1f;
            float h = Screen.height > 0 ? Screen.height : 1f;

            // Convert pixel safe-area insets to percentages of the panel.
            float left = safe.xMin / w * 100f;
            float right = (w - safe.xMax) / w * 100f;
            float top = (h - safe.yMax) / h * 100f;
            float bottom = safe.yMin / h * 100f;

            element.style.paddingLeft = Length.Percent(left);
            element.style.paddingRight = Length.Percent(right);
            element.style.paddingTop = Length.Percent(top);
            element.style.paddingBottom = Length.Percent(bottom);
        }

        public UIDocument GetDocument(UILayer layer)
        {
            switch (layer)
            {
                case UILayer.Hud: return HudDocument;
                case UILayer.Overlay: return OverlayDocument;
                case UILayer.Modal: return ModalDocument;
                case UILayer.Toast: return ToastDocument;
                case UILayer.Transition: return TransitionDocument;
                default: return ModalDocument;
            }
        }

        public void ApplyAccessibilityClasses(bool highContrast, bool reducedMotion, bool dyslexia, float textScale)
        {
            foreach (var doc in new[] { HudDocument, OverlayDocument, ModalDocument, ToastDocument, TransitionDocument })
            {
                if (doc?.rootVisualElement == null) continue;
                var root = doc.rootVisualElement;
                root.EnableInClassList("high-contrast", highContrast);
                root.EnableInClassList("reduced-motion", reducedMotion);
                root.EnableInClassList("dyslexia", dyslexia);
                root.EnableInClassList("text-scale-large", textScale >= 1.2f && textScale < 1.45f);
                root.EnableInClassList("text-scale-xlarge", textScale >= 1.45f);
            }
        }
    }
}
