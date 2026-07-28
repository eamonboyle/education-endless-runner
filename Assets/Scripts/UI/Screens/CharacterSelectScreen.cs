using MathRunner.UI.Toolkit;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    public class CharacterSelectScreen : UIScreen
    {
        public override string ScreenId => "character_select";
        public override UILayer Layer => UILayer.Modal;
        public override string UxmlResourcePath => "UI/Screens/character_select";

        protected override void OnBind(VisualElement root)
        {
            Wire(root, "boy-button", () => Choose("boy"));
            Wire(root, "girl-button", () => Choose("girl"));
            Wire(root, "back-button", () => NavigationService.GoToMainMenu());
        }

        private static void Choose(string character)
        {
            GameState.SetCharacter(character);
            if (GameState.IsFirstLoad())
                NavigationService.GoToModeSelect();
            else
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
