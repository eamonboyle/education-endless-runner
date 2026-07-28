using System.Collections;
using MathRunner.UI.Toolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    public class GameOverScreen : UIScreen
    {
        public override string ScreenId => "game_over";
        public override UILayer Layer => UILayer.Modal;
        public override string UxmlResourcePath => "UI/Screens/game_over";

        private Label currentScore;
        private Label highScore;

        protected override void OnBind(VisualElement root)
        {
            currentScore = root.Q<Label>("current-score");
            highScore = root.Q<Label>("high-score");

            Wire(root, "restart-button", () => GameSession.BeginRun());
            Wire(root, "share-button", () =>
            {
                if (GameManager.instance != null) GameManager.instance.Screenshot();
            });
            Wire(root, "continue-button", () =>
            {
                if (UIRouter.Instance != null)
                    UIRouter.Instance.StartCoroutine(ContinueRoutine());
            });
            Wire(root, "quit-button", () =>
            {
                UIRouter.Instance?.HideModal();
                UIRouter.Instance?.HideHud();
                if (GameManager.instance != null) GameManager.instance.LoadMainMenu();
            });
            Wire(root, "history-button", () => UIRouter.Instance?.ShowModal("session_summary"));
        }

        protected override void OnShow()
        {
            if (currentScore != null) currentScore.text = GameState.GetScore().ToString();
            if (highScore != null) highScore.text = GameState.GetHighScore().ToString();
        }

        private static void Wire(VisualElement root, string name, System.Action action)
        {
            var btn = root.Q<Button>(name);
            if (btn == null) return;
            btn.clickable = new Clickable(() => action());
        }

        private static IEnumerator ContinueRoutine()
        {
            GameState.SetGameOver(false);
            if (LivesSystem.Instance != null)
                LivesSystem.Instance.ResetLives();

            var questionGeneration = Object.FindAnyObjectByType<QuestionGeneration>();
            if (questionGeneration != null)
                questionGeneration.ResyncAfterContinue();

            UIRouter.Instance?.HideModal();
            GameState.ShowGameUI();

            var hud = UIRouter.Instance?.Hud;
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
