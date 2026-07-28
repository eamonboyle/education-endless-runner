using MathRunner.Core;
using MathRunner.UI.Toolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    public class ModeChoiceScreen : UIScreen
    {
        public override string ScreenId => "mode_choice";
        public override UILayer Layer => UILayer.Modal;
        public override string UxmlResourcePath => "UI/Screens/mode_choice";

        private VisualElement questionPanel;
        private VisualElement stylePanel;
        private Label questionsPill;
        private string questionType = "addition";
        private PlayStyle style = PlayStyle.Classic;
        private DifficultyLevel difficulty = DifficultyLevel.Medium;

        private enum PlayStyle { Classic, TimeAttack, Campaign }

        protected override void OnBind(VisualElement root)
        {
            questionPanel = root.Q("question-type-panel");
            stylePanel = root.Q("play-style-panel");
            questionsPill = root.Q<Label>("questions-pill");

            Wire(root, "addition-button", () => ChooseType("addition"));
            Wire(root, "subtraction-button", () => ChooseType("subtraction"));
            Wire(root, "multiply-button", () => ChooseType("multiply"));
            Wire(root, "division-button", () => ChooseType("division"));
            Wire(root, "home-button", () => NavigationService.GoToMainMenu());

            var classic = root.Q("classic-card");
            var timeAttack = root.Q("timeattack-card");
            var campaign = root.Q("campaign-card");
            if (classic != null) classic.RegisterCallback<ClickEvent>(_ => SelectStyle(PlayStyle.Classic));
            if (timeAttack != null) timeAttack.RegisterCallback<ClickEvent>(_ => SelectStyle(PlayStyle.TimeAttack));
            if (campaign != null) campaign.RegisterCallback<ClickEvent>(_ => SelectStyle(PlayStyle.Campaign));

            Wire(root, "easy-chip", () => SelectDifficulty(DifficultyLevel.Easy));
            Wire(root, "normal-chip", () => SelectDifficulty(DifficultyLevel.Medium));
            Wire(root, "hard-chip", () => SelectDifficulty(DifficultyLevel.Hard));
            Wire(root, "play-button", Play);
            Wire(root, "back-button", () => ShowQuestionTypes());

            ShowQuestionTypes();
            RefreshSelection(root);
        }

        private void ChooseType(string mode)
        {
            questionType = mode;
            GameState.SetQuestionType(mode);

            if (GameState.IsFirstLoad())
            {
                TimeAttackMode.SetTimeAttack(false);
                PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 0);
                if (GameManager.instance != null)
                    GameManager.instance.LoadTutorial();
                return;
            }

            ShowPlayStyle();
        }

        private void ShowQuestionTypes()
        {
            questionPanel?.RemoveFromClassList("hidden");
            stylePanel?.AddToClassList("hidden");
        }

        private void ShowPlayStyle()
        {
            questionPanel?.AddToClassList("hidden");
            stylePanel?.RemoveFromClassList("hidden");
            if (questionsPill != null)
                questionsPill.text = DisplayName(questionType);
            RefreshSelection(Root);
        }

        private void SelectStyle(PlayStyle s)
        {
            style = s;
            RefreshSelection(Root);
        }

        private void SelectDifficulty(DifficultyLevel d)
        {
            difficulty = d;
            RefreshSelection(Root);
        }

        private void RefreshSelection(VisualElement root)
        {
            if (root == null) return;
            SetSelected(root.Q("classic-card"), style == PlayStyle.Classic);
            SetSelected(root.Q("timeattack-card"), style == PlayStyle.TimeAttack);
            SetSelected(root.Q("campaign-card"), style == PlayStyle.Campaign);
            SetSelected(root.Q<Button>("easy-chip"), difficulty == DifficultyLevel.Easy);
            SetSelected(root.Q<Button>("normal-chip"), difficulty == DifficultyLevel.Medium);
            SetSelected(root.Q<Button>("hard-chip"), difficulty == DifficultyLevel.Hard);
        }

        private static void SetSelected(VisualElement el, bool selected)
        {
            el?.EnableInClassList("selected", selected);
        }

        private void Play()
        {
            switch (style)
            {
                case PlayStyle.TimeAttack:
                    TimeAttackMode.SetTimeAttack(true);
                    PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 0);
                    GameState.SetQuestionType(questionType);
                    break;
                case PlayStyle.Campaign:
                    TimeAttackMode.SetTimeAttack(false);
                    int level = CampaignManager.GetCurrentLevel();
                    PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 1);
                    PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_LEVEL, level);
                    GameState.SetQuestionType(CampaignManager.GetLevelConfig(level).MathMode.ToPlayerPrefsString());
                    break;
                default:
                    TimeAttackMode.SetTimeAttack(false);
                    PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 0);
                    GameState.SetQuestionType(questionType);
                    break;
            }

            DifficultyPresets.SetDifficulty(difficulty);
            PrefsFlush.Flush();
            UIRouter.Instance?.HideModal();
            GameSession.BeginRun();
        }

        private static string DisplayName(string mode) => mode switch
        {
            "addition" => "Addition",
            "subtraction" => "Subtraction",
            "multiply" => "Multiplication",
            "division" => "Division",
            _ => mode
        };

        private static void Wire(VisualElement root, string name, System.Action action)
        {
            var btn = root.Q<Button>(name);
            if (btn == null) return;
            btn.clickable = new Clickable(() => action());
        }
    }
}
