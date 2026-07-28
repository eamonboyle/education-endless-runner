using MathRunner.UI.Toolkit;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    public class MainMenuScreen : UIScreen
    {
        public override string ScreenId => "main_menu";
        public override UILayer Layer => UILayer.Modal;
        public override string UxmlResourcePath => "UI/Screens/main_menu";

        protected override void OnBind(VisualElement root)
        {
            Wire(root, "play-button", () => NavigationService.GoToModeSelect());
            Wire(root, "mode-button", () => NavigationService.GoToModeSelect());
            Wire(root, "character-button", () => NavigationService.GoToCharacterSelect());
            Wire(root, "stats-button", () => UIRouter.Instance?.ShowModal("stats"));
            Wire(root, "challenges-button", () => UIRouter.Instance?.ShowModal("challenges"));
            Wire(root, "settings-button", () => NavigationService.GoToSettings());

            SetText(root, "play-button", Loc("menu_play", "Run"));
            SetText(root, "mode-button", Loc("menu_mode", "Mode") + " " + DisplaySymbol(GameState.GetQuestionType()));
            SetText(root, "character-button", Loc("menu_character", "Character"));
            SetText(root, "stats-button", Loc("menu_stats", "Stats"));
            SetText(root, "challenges-button", Loc("menu_challenges", "Challenges"));
            SetText(root, "settings-button", Loc("menu_settings", "Settings"));
        }

        private static string Loc(string key, string fallback)
        {
            string value = L(key);
            return value == key ? fallback : value;
        }

        private static string DisplaySymbol(string mode)
        {
            return mode switch
            {
                "addition" => "+",
                "subtraction" => "−",
                "multiply" => "×",
                "division" => "÷",
                _ => "?"
            };
        }

        private static void SetText(VisualElement root, string name, string text)
        {
            var label = root.Q<Label>(name);
            if (label != null) { label.text = text; return; }
            var button = root.Q<Button>(name);
            if (button != null) button.text = text;
        }

        private static void Wire(VisualElement root, string name, System.Action action)
        {
            var btn = root.Q<Button>(name);
            if (btn == null) return;
            btn.clickable = new Clickable(() => action());
        }
    }
}
