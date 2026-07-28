using MathRunner.UI.Toolkit;
using MathRunner.UI.ViewModels;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    public class StatsScreen : UIScreen
    {
        public override string ScreenId => "stats";
        public override UILayer Layer => UILayer.Modal;
        public override string UxmlResourcePath => "UI/Screens/stats";

        private readonly ProgressionViewModel vm = new ProgressionViewModel();

        protected override void OnBind(VisualElement root)
        {
            var back = root.Q<Button>("back-button");
            if (back != null) back.clickable = new Clickable(() => NavigationService.GoToMainMenu());
        }

        protected override void OnShow()
        {
            vm.RefreshFromPlayerStats();
            Set("games-played", vm.GamesPlayed.ToString());
            Set("best-score", vm.BestScore.ToString());
            Set("accuracy", vm.Accuracy.ToString("F1") + "%");
            Set("best-streak", vm.BestStreak.ToString());
            Set("total-correct", vm.TotalCorrect.ToString());

            var graph = Q<VisualElement>("graph-row");
            if (graph == null) return;
            graph.Clear();
            foreach (float v in vm.WeeklyScores)
            {
                var bar = new VisualElement();
                bar.AddToClassList("graph-bar");
                bar.style.height = Length.Percent(MathfMax(8f, v * 100f));
                graph.Add(bar);
            }
        }

        private void Set(string name, string value)
        {
            var label = Q<Label>(name);
            if (label != null) label.text = value;
        }

        private static float MathfMax(float a, float b) => a > b ? a : b;
    }
}
