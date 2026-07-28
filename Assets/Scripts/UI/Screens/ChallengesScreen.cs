using MathRunner.UI.Toolkit;
using MathRunner.UI.ViewModels;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    public class ChallengesScreen : UIScreen
    {
        public override string ScreenId => "challenges";
        public override UILayer Layer => UILayer.Modal;
        public override string UxmlResourcePath => "UI/Screens/challenges";

        private readonly ProgressionViewModel vm = new ProgressionViewModel();

        protected override void OnBind(VisualElement root)
        {
            var back = root.Q<Button>("back-button");
            if (back != null) back.clickable = new Clickable(() => NavigationService.GoToMainMenu());
        }

        protected override void OnShow()
        {
            vm.RefreshFromPlayerStats();
            Set("daily-desc", vm.DailyDescription);
            Set("daily-progress", vm.DailyProgressLabel);
            Set("weekly-desc", vm.WeeklyDescription);
            Set("weekly-progress", vm.WeeklyProgressLabel);

            var dailyFill = Q<VisualElement>("daily-fill");
            if (dailyFill != null) dailyFill.style.width = Length.Percent(vm.DailyProgress * 100f);
            var weeklyFill = Q<VisualElement>("weekly-fill");
            if (weeklyFill != null) weeklyFill.style.width = Length.Percent(vm.WeeklyProgress * 100f);
        }

        private void Set(string name, string value)
        {
            var label = Q<Label>(name);
            if (label != null) label.text = value ?? "";
        }
    }
}
