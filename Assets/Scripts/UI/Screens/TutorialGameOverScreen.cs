using MathRunner.UI.Toolkit;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    public class TutorialGameOverScreen : UIScreen
    {
        public override string ScreenId => "tutorial_gameover";
        public override UILayer Layer => UILayer.Modal;
        public override string UxmlResourcePath => "UI/Screens/tutorial_gameover";

        protected override void OnBind(VisualElement root)
        {
            var btn = root.Q<Button>("restart-button");
            if (btn != null)
            {
                btn.clickable = new Clickable(() =>
                {
                    UIRouter.Instance?.HideModal();
                    if (GameManager.instance != null)
                        GameManager.instance.LoadTutorial();
                });
            }
        }
    }
}
