using System.Collections.Generic;
using MathRunner.UI.Toolkit;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    public class SessionSummaryScreen : UIScreen
    {
        public override string ScreenId => "session_summary";
        public override UILayer Layer => UILayer.Modal;
        public override string UxmlResourcePath => "UI/Screens/session_summary";

        private static readonly List<string> History = new List<string>();

        public static void RecordQuestion(string entry)
        {
            if (string.IsNullOrEmpty(entry)) return;
            History.Add(entry);
            if (History.Count > 40) History.RemoveAt(0);
        }

        public static void ClearHistory() => History.Clear();

        protected override void OnBind(VisualElement root)
        {
            var close = root.Q<Button>("close-button");
            if (close != null)
            {
                close.clickable = new Clickable(() =>
                {
                    // Return to game over if we still are in a game-over state.
                    if (GameState.IsGameOver())
                        UIRouter.Instance?.ShowModal("game_over");
                    else
                        UIRouter.Instance?.HideModal();
                });
            }
        }

        protected override void OnShow()
        {
            var score = Q<Label>("summary-score");
            var accuracy = Q<Label>("summary-accuracy");
            var questions = Q<Label>("summary-questions");
            if (score != null) score.text = "Score: " + GameState.GetScore();
            if (accuracy != null) accuracy.text = "Accuracy: " + GameState.GetAccuracyThisGame().ToString("F0") + "%";
            if (questions != null) questions.text = "Questions: " + GameState.GetQuestionsAnsweredThisGame();

            var list = Q<VisualElement>("history-list");
            if (list == null) return;
            list.Clear();
            for (int i = History.Count - 1; i >= 0; i--)
            {
                var row = new Label(History[i]);
                row.AddToClassList("label");
                row.style.marginBottom = 4;
                list.Add(row);
            }
        }
    }
}
