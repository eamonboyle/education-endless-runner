using MathRunner.Core;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime accessibility settings panel for the Settings scene.
/// Spawned by <see cref="ProgressionUIBootstrap"/>.
/// </summary>
public class AccessibilitySettingsUI : MonoBehaviour
{
    private Text statusText;

    private void Start()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        var canvasGo = new GameObject("AccessibilitySettingsCanvas");
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 70;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        var panel = new GameObject("A11yPanel");
        panel.transform.SetParent(canvasGo.transform, false);
        var rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(0f, 16f);
        rt.sizeDelta = new Vector2(-24f, 220f);
        panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.72f);

        statusText = CreateLabel(panel.transform, "Status", new Vector2(0f, -8f), font, 14);
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.text = "Accessibility";

        float y = -40f;
        CreateToggleButton(panel.transform, "Reduced Motion", new Vector2(-160f, y), font, ToggleReducedMotion);
        CreateToggleButton(panel.transform, "High Contrast", new Vector2(0f, y), font, ToggleHighContrast);
        CreateToggleButton(panel.transform, "TTS", new Vector2(160f, y), font, ToggleTts);

        y = -90f;
        CreateToggleButton(panel.transform, "Dyslexia Font", new Vector2(-160f, y), font, ToggleDyslexia);
        CreateToggleButton(panel.transform, "One-Handed", new Vector2(0f, y), font, ToggleOneHanded);
        CreateToggleButton(panel.transform, "Colorblind Cycle", new Vector2(160f, y), font, CycleColorblind);

        y = -140f;
        CreateToggleButton(panel.transform, "Text Scale -", new Vector2(-100f, y), font, () => AdjustTextScale(-0.1f));
        CreateToggleButton(panel.transform, "Text Scale +", new Vector2(100f, y), font, () => AdjustTextScale(0.1f));

        RefreshStatus();
    }

    private void ToggleReducedMotion()
    {
        var mgr = ReducedMotionManager.Instance;
        if (mgr == null) return;
        mgr.SetReducedMotion(!mgr.IsReducedMotion());
        RefreshStatus();
    }

    private void ToggleHighContrast()
    {
        var mgr = AccessibilityManager.Instance;
        if (mgr == null) return;
        mgr.SetHighContrastMode(!mgr.HighContrastMode);
        RefreshStatus();
    }

    private void ToggleTts()
    {
        var mgr = TextToSpeechManager.Instance;
        if (mgr == null) return;
        mgr.SetEnabled(!mgr.IsEnabled());
        RefreshStatus();
    }

    private void ToggleDyslexia()
    {
        var mgr = Object.FindAnyObjectByType<DyslexiaFontManager>();
        if (mgr == null) return;
        if (mgr.IsEnabled()) mgr.DisableDyslexiaFont();
        else mgr.EnableDyslexiaFont();
        RefreshStatus();
    }

    private void ToggleOneHanded()
    {
        var input = InputManager.Instance;
        if (input == null) return;
        bool tap = input.GetInputMode() != InputManager.InputMode.Tap;
        input.SetInputMode(tap ? InputManager.InputMode.Tap : InputManager.InputMode.Swipe);
        PlayerPrefs.SetInt("Accessibility_OneHanded", tap ? 1 : 0);
        PrefsFlush.Flush();
        RefreshStatus();
    }

    private void CycleColorblind()
    {
        var mgr = AccessibilityManager.Instance;
        if (mgr == null) return;
        int next = ((int)mgr.CurrentColorblindMode + 1) % 4;
        mgr.SetColorblindMode((AccessibilityManager.ColorblindMode)next);
        RefreshStatus();
    }

    private void AdjustTextScale(float delta)
    {
        var mgr = AccessibilityManager.Instance;
        if (mgr == null) return;
        mgr.SetTextScale(mgr.GetTextScale() + delta);
        RefreshStatus();
    }

    private void RefreshStatus()
    {
        if (statusText == null) return;
        string rm = ReducedMotionManager.Instance != null && ReducedMotionManager.Instance.IsReducedMotion() ? "RM:on" : "RM:off";
        string hc = AccessibilityManager.Instance != null && AccessibilityManager.Instance.HighContrastMode ? "HC:on" : "HC:off";
        string tts = TextToSpeechManager.Instance != null && TextToSpeechManager.Instance.IsEnabled() ? "TTS:on" : "TTS:off";
        string scale = AccessibilityManager.Instance != null ? AccessibilityManager.Instance.GetTextScale().ToString("F1") : "1.0";
        string cb = AccessibilityManager.Instance != null ? AccessibilityManager.Instance.CurrentColorblindMode.ToString() : "Normal";
        string mode = InputManager.Instance != null ? InputManager.Instance.GetInputMode().ToString() : "Swipe";
        statusText.text = $"{rm}  {hc}  {tts}  Scale:{scale}  {cb}  Input:{mode}";
    }

    private static Text CreateLabel(Transform parent, string name, Vector2 pos, Font font, int size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(-16f, 28f);
        var text = go.AddComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.color = Color.white;
        return text;
    }

    private static void CreateToggleButton(Transform parent, string label, Vector2 pos, Font font, UnityEngine.Events.UnityAction onClick)
    {
        var go = new GameObject(label + "Btn");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(140f, 36f);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.25f, 0.4f, 0.55f, 0.95f);
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(onClick);

        var textGo = new GameObject("Label");
        textGo.transform.SetParent(go.transform, false);
        var trt = textGo.AddComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;
        var text = textGo.AddComponent<Text>();
        text.font = font;
        text.fontSize = 13;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.text = label;
    }
}
