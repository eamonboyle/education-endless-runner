using MathRunner.UI.Toolkit;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    public class TutorialCompleteScreen : UIScreen
    {
        public override string ScreenId => "tutorial_complete";
        public override UILayer Layer => UILayer.Modal;
        public override string UxmlResourcePath => "UI/Screens/tutorial_complete";

        protected override void OnBind(VisualElement root)
        {
            var btn = root.Q<Button>("menu-button");
            if (btn != null)
            {
                btn.clickable = new Clickable(() =>
                {
                    UIRouter.Instance?.HideModal();
                    NavigationService.GoToMainMenu();
                });
            }
        }
    }
}
