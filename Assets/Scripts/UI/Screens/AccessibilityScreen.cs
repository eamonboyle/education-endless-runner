using MathRunner.Core;
using MathRunner.UI.Toolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    public class AccessibilityScreen : UIScreen
    {
        public override string ScreenId => "accessibility";
        public override UILayer Layer => UILayer.Modal;
        public override string UxmlResourcePath => "UI/Screens/accessibility";

        private Label statusLabel;

        protected override void OnBind(VisualElement root)
        {
            statusLabel = root.Q<Label>("status-label");
            Wire(root, "reduced-motion", ToggleReducedMotion);
            Wire(root, "high-contrast", ToggleHighContrast);
            Wire(root, "tts", ToggleTts);
            Wire(root, "dyslexia", ToggleDyslexia);
            Wire(root, "one-handed", ToggleOneHanded);
            Wire(root, "colorblind", CycleColorblind);
            Wire(root, "text-scale", () => AdjustTextScale(0.1f));
            Wire(root, "back-button", () => UIRouter.Instance?.ShowModal("settings"));
            RefreshStatus();
            ApplyRootClasses();
        }

        private void ToggleReducedMotion()
        {
            var mgr = ReducedMotionManager.Instance;
            if (mgr == null) return;
            mgr.SetReducedMotion(!mgr.IsReducedMotion());
            ApplyRootClasses();
            RefreshStatus();
        }

        private void ToggleHighContrast()
        {
            var mgr = AccessibilityManager.Instance;
            if (mgr == null) return;
            mgr.SetHighContrastMode(!mgr.HighContrastMode);
            ApplyRootClasses();
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
            bool enabled = PlayerPrefs.GetInt("Accessibility_DyslexiaFont", 0) == 1;
            PlayerPrefs.SetInt("Accessibility_DyslexiaFont", enabled ? 0 : 1);
            PrefsFlush.Flush();
            var mgr = Object.FindAnyObjectByType<DyslexiaFontManager>();
            if (mgr != null)
            {
                if (enabled) mgr.DisableDyslexiaFont();
                else mgr.EnableDyslexiaFont();
            }
            ApplyRootClasses();
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
            float next = mgr.GetTextScale() + delta;
            if (next > 2f) next = 1f;
            mgr.SetTextScale(next);
            ApplyRootClasses();
            RefreshStatus();
        }

        private void ApplyRootClasses()
        {
            bool hc = AccessibilityManager.Instance != null && AccessibilityManager.Instance.HighContrastMode;
            bool rm = ReducedMotionManager.Instance != null && ReducedMotionManager.Instance.IsReducedMotion();
            bool dyslexia = PlayerPrefs.GetInt("Accessibility_DyslexiaFont", 0) == 1;
            float scale = AccessibilityManager.Instance != null ? AccessibilityManager.Instance.GetTextScale() : 1f;
            UIRoot.Instance?.ApplyAccessibilityClasses(hc, rm, dyslexia, scale);
        }

        private void RefreshStatus()
        {
            if (statusLabel == null) return;
            string rm = ReducedMotionManager.Instance != null && ReducedMotionManager.Instance.IsReducedMotion() ? "RM:on" : "RM:off";
            string hc = AccessibilityManager.Instance != null && AccessibilityManager.Instance.HighContrastMode ? "HC:on" : "HC:off";
            string tts = TextToSpeechManager.Instance != null && TextToSpeechManager.Instance.IsEnabled() ? "TTS:on" : "TTS:off";
            string scale = AccessibilityManager.Instance != null ? AccessibilityManager.Instance.GetTextScale().ToString("F1") : "1.0";
            string cb = AccessibilityManager.Instance != null ? AccessibilityManager.Instance.CurrentColorblindMode.ToString() : "Normal";
            string mode = InputManager.Instance != null ? InputManager.Instance.GetInputMode().ToString() : "Swipe";
            string dys = PlayerPrefs.GetInt("Accessibility_DyslexiaFont", 0) == 1 ? "Dys:on" : "Dys:off";
            statusLabel.text = $"{rm}  {hc}  {tts}  {dys}  Scale:{scale}  {cb}  Input:{mode}";
        }

        private static void Wire(VisualElement root, string name, System.Action action)
        {
            var btn = root.Q<Button>(name);
            if (btn == null) return;
            btn.clickable = new Clickable(() => action());
        }
    }
}
