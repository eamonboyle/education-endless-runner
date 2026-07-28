using MathRunner.Core;
using MathRunner.UI.Toolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    public class SettingsScreen : UIScreen
    {
        public override string ScreenId => "settings";
        public override UILayer Layer => UILayer.Modal;
        public override string UxmlResourcePath => "UI/Screens/settings";

        private Button soundButton;
        private Label graphicsLabel;
        private VisualElement resetOverlay;

        protected override void OnBind(VisualElement root)
        {
            soundButton = root.Q<Button>("sound-button");
            graphicsLabel = root.Q<Label>("graphics-label");
            resetOverlay = root.Q("reset-overlay");

            if (soundButton != null) soundButton.clicked += ToggleSound;
            Wire(root, "graphics-up", () => ChangeGraphics(true));
            Wire(root, "graphics-down", () => ChangeGraphics(false));
            Wire(root, "accessibility-button", () => UIRouter.Instance?.ShowModal("accessibility"));
            Wire(root, "reset-button", () => resetOverlay?.RemoveFromClassList("hidden"));
            Wire(root, "reset-confirm", ConfirmReset);
            Wire(root, "reset-cancel", () => resetOverlay?.AddToClassList("hidden"));
            Wire(root, "home-button", () => NavigationService.GoToMainMenu());

            Refresh();
        }

        private void Refresh()
        {
            bool soundOn = PlayerPrefs.GetInt("sound", 1) == 1;
            if (soundButton != null) soundButton.text = soundOn ? "Sound: On" : "Sound: Off";
            if (graphicsLabel != null)
            {
                int q = QualitySettings.GetQualityLevel();
                string[] names = QualitySettings.names;
                graphicsLabel.text = q >= 0 && q < names.Length ? names[q] : "Medium";
            }
        }

        private void ToggleSound()
        {
            bool soundOn = PlayerPrefs.GetInt("sound", 1) == 1;
            PlayerPrefs.SetInt("sound", soundOn ? 0 : 1);
            PrefsFlush.Flush();
            AudioListener.volume = soundOn ? 0f : 1f;
            Refresh();
        }

        private void ChangeGraphics(bool increase)
        {
            int index = QualitySettings.GetQualityLevel();
            int max = QualitySettings.names.Length - 1;
            index = increase ? Mathf.Min(index + 1, max) : Mathf.Max(index - 1, 0);
            QualitySettings.SetQualityLevel(index);
            Refresh();
        }

        private void ConfirmReset()
        {
            PlayerPrefs.DeleteAll();
            PrefsFlush.Flush();
            resetOverlay?.AddToClassList("hidden");
            NavigationService.GoToMainMenu();
        }

        private static void Wire(VisualElement root, string name, System.Action action)
        {
            var btn = root.Q<Button>(name);
            if (btn == null) return;
            btn.clickable = new Clickable(() => action());
        }
    }
}
