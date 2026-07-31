using System.Collections;
using MathRunner.UI.Toolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    public class PauseScreen : UIScreen
    {
        public override string ScreenId => "pause";
        public override UILayer Layer => UILayer.Modal;
        public override string UxmlResourcePath => "UI/Screens/pause";

        protected override void OnBind(VisualElement root)
        {
            var continueBtn = root.Q<Button>("continue-button");
            var quitBtn = root.Q<Button>("quit-button");
            if (continueBtn != null)
            {
                continueBtn.UnregisterCallback<ClickEvent>(OnContinue);
                continueBtn.RegisterCallback<ClickEvent>(OnContinue);
            }
            if (quitBtn != null)
            {
                quitBtn.UnregisterCallback<ClickEvent>(OnQuit);
                quitBtn.RegisterCallback<ClickEvent>(OnQuit);
            }
        }

        private void OnContinue(ClickEvent evt)
        {
            UIRouter.Instance?.HideModal();
            if (UIRouter.Instance != null)
                UIRouter.Instance.StartCoroutine(ResumeCountdown());
        }

        private void OnQuit(ClickEvent evt)
        {
            UIRouter.Instance?.HideModal();
            UIRouter.Instance?.HideHud();
            if (GameManager.instance != null)
                GameManager.instance.LoadMainMenu();
        }

        private static IEnumerator ResumeCountdown()
        {
            var hud = UIRouter.Instance?.Hud;
            GameState.ShowGameUI();
            int count = 4;
            while (count > 0)
            {
                hud?.SetCountdown(count == 1 ? "GO!" : (count - 1).ToString(), true);
                yield return new WaitForSeconds(1f);
                count--;
            }
            hud?.SetCountdown("", false);
            GameState.QuestionBoxShow(true);
            var player = GameObject.Find("PlayerObject");
            if (player != null)
            {
                var animator = player.GetComponent<Animator>();
                if (animator != null) animator.SetBool("isRunning", true);
            }
            GameState.SetRunning(true);
        }
    }
}
