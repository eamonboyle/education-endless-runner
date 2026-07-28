using System.Collections;
using System.Collections.Generic;
using MathRunner.UI.Toolkit;
using MathRunner.UI.ViewModels;
using UnityEngine;
using UnityEngine.UIElements;

namespace MathRunner.UI.Screens
{
    /// <summary>
    /// Gameplay HUD: lives, score, combo, speed, power-ups, question, countdown.
    /// Root picking-mode is Ignore so swipe input reaches the player.
    /// </summary>
    public class HudScreen : UIScreen
    {
        public override string ScreenId => "hud";
        public override UILayer Layer => UILayer.Hud;
        public override string UxmlResourcePath => null; // pre-mounted by UIRoot

        public RunHudViewModel ViewModel { get; } = new RunHudViewModel();

        private Label livesLabel;
        private Label scoreLabel;
        private Label comboLabel;
        private Label speedLabel;
        private Label questionLabel;
        private VisualElement questionBanner;
        private Label countdownLabel;
        private Label laneLabel;
        private Label difficultyLabel;
        private Button pauseButton;
        private readonly VisualElement[] powerSlots = new VisualElement[3];
        private readonly Label[] powerLabels = new Label[3];
        private readonly PowerUpType?[] slotTypes = new PowerUpType?[3];
        private Coroutine powerTick;
        private Coroutine hostRoutine;
        private MonoBehaviour host;
        private float speedTimer;
        private bool subscribed;

        protected override void OnBind(VisualElement root)
        {
            livesLabel = root.Q<Label>("lives-label");
            scoreLabel = root.Q<Label>("score-label");
            comboLabel = root.Q<Label>("combo-label");
            speedLabel = root.Q<Label>("speed-label");
            questionLabel = root.Q<Label>("question-label");
            questionBanner = root.Q("question-banner");
            countdownLabel = root.Q<Label>("countdown-label");
            laneLabel = root.Q<Label>("lane-label");
            difficultyLabel = root.Q<Label>("difficulty-label");
            pauseButton = root.Q<Button>("pause-button");

            for (int i = 0; i < 3; i++)
            {
                powerSlots[i] = root.Q($"powerup-{i}");
                powerLabels[i] = root.Q<Label>($"powerup-{i}-label");
            }

            if (pauseButton != null)
            {
                pauseButton.UnregisterCallback<ClickEvent>(OnPauseClicked);
                pauseButton.RegisterCallback<ClickEvent>(OnPauseClicked);
            }

            // Ensure non-interactive HUD never blocks swipes.
            root.pickingMode = PickingMode.Ignore;
            var safe = root.Q("safe-area");
            if (safe != null) safe.pickingMode = PickingMode.Ignore;
            if (pauseButton != null) pauseButton.pickingMode = PickingMode.Position;

            ViewModel.Changed -= Refresh;
            ViewModel.Changed += Refresh;
            Refresh();
        }

        protected override void OnShow()
        {
            Subscribe();
            host = UIRouter.Instance;
            if (host != null && hostRoutine == null)
                hostRoutine = host.StartCoroutine(Tick());
            SyncFromSystems();
            Refresh();
        }

        protected override void OnHide()
        {
            Unsubscribe();
            if (host != null && hostRoutine != null)
            {
                host.StopCoroutine(hostRoutine);
                hostRoutine = null;
            }
        }

        private void OnPauseClicked(ClickEvent evt)
        {
            if (GameState.IsRunning())
                GameState.ShowPauseUI();
        }

        private void Subscribe()
        {
            if (subscribed) return;
            GameState.OnScoreChanged += OnScoreChanged;
            if (ComboSystem.Instance != null)
                ComboSystem.Instance.OnMultiplierChanged += OnMultiplierChanged;
            if (LivesSystem.Instance != null)
                LivesSystem.Instance.OnLifeLost += OnLifeLost;
            if (PowerUpSystem.Instance != null)
            {
                PowerUpSystem.Instance.OnPowerUpActivated += OnPowerUpActivated;
                PowerUpSystem.Instance.OnPowerUpExpired += OnPowerUpExpired;
            }
            subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!subscribed) return;
            GameState.OnScoreChanged -= OnScoreChanged;
            if (ComboSystem.Instance != null)
                ComboSystem.Instance.OnMultiplierChanged -= OnMultiplierChanged;
            if (LivesSystem.Instance != null)
                LivesSystem.Instance.OnLifeLost -= OnLifeLost;
            if (PowerUpSystem.Instance != null)
            {
                PowerUpSystem.Instance.OnPowerUpActivated -= OnPowerUpActivated;
                PowerUpSystem.Instance.OnPowerUpExpired -= OnPowerUpExpired;
            }
            subscribed = false;
        }

        private void SyncFromSystems()
        {
            ViewModel.Score = GameState.GetScore();
            ViewModel.Speed = Mathf.RoundToInt(GameState.GetCharacterSpeed());
            if (ComboSystem.Instance != null)
                ViewModel.Multiplier = ComboSystem.Instance.GetMultiplier();
            if (LivesSystem.Instance != null)
            {
                ViewModel.Lives = LivesSystem.Instance.GetLives();
                ViewModel.MaxLives = LivesSystem.Instance.GetMaxLives();
            }
        }

        private void OnScoreChanged(int score) => ViewModel.Score = score;
        private void OnMultiplierChanged(int m) => ViewModel.Multiplier = m;
        private void OnLifeLost(int remaining) => ViewModel.Lives = remaining;

        private void OnPowerUpActivated(PowerUpType type)
        {
            float remaining = PowerUpSystem.Instance != null
                ? PowerUpSystem.Instance.GetRemainingDuration(type)
                : 10f;
            ViewModel.SetPowerUp(type, remaining);
            AssignSlot(type);
            RefreshPowerSlots();
        }

        private void OnPowerUpExpired(PowerUpType type)
        {
            ViewModel.SetPowerUp(type, 0f);
            for (int i = 0; i < slotTypes.Length; i++)
            {
                if (slotTypes[i] == type) slotTypes[i] = null;
            }
            RefreshPowerSlots();
        }

        private void AssignSlot(PowerUpType type)
        {
            for (int i = 0; i < slotTypes.Length; i++)
            {
                if (slotTypes[i] == type) return;
            }
            for (int i = 0; i < slotTypes.Length; i++)
            {
                if (slotTypes[i] == null)
                {
                    slotTypes[i] = type;
                    return;
                }
            }
            slotTypes[0] = type;
        }

        private IEnumerator Tick()
        {
            while (true)
            {
                if (IsVisible && GameState.IsRunning())
                {
                    speedTimer -= Time.deltaTime;
                    if (speedTimer <= 0f)
                    {
                        speedTimer = 0.25f;
                        ViewModel.Speed = Mathf.RoundToInt(GameState.GetCharacterSpeed());
                    }

                    if (PowerUpSystem.Instance != null)
                    {
                        var keys = new List<PowerUpType>(ViewModel.PowerUpTimers.Keys);
                        foreach (var type in keys)
                        {
                            float rem = PowerUpSystem.Instance.GetRemainingDuration(type);
                            ViewModel.SetPowerUp(type, rem);
                        }
                        RefreshPowerSlots();
                    }
                }
                yield return null;
            }
        }

        private void Refresh()
        {
            if (livesLabel != null) livesLabel.text = ViewModel.LivesDisplay;
            if (scoreLabel != null) scoreLabel.text = ViewModel.ScoreDisplay;
            if (comboLabel != null)
            {
                comboLabel.text = ViewModel.ComboDisplay;
                comboLabel.EnableInClassList("hidden", ViewModel.Multiplier <= 1);
            }
            if (speedLabel != null) speedLabel.text = "Speed " + ViewModel.Speed;
            if (questionLabel != null) questionLabel.text = ViewModel.QuestionText;
            if (questionBanner != null)
                questionBanner.EnableInClassList("hidden", !ViewModel.QuestionVisible);
            if (countdownLabel != null)
            {
                countdownLabel.text = ViewModel.CountdownText;
                countdownLabel.EnableInClassList("hidden", !ViewModel.CountdownVisible);
            }
            if (laneLabel != null)
            {
                laneLabel.text = ViewModel.LaneText;
                laneLabel.EnableInClassList("hidden", string.IsNullOrEmpty(ViewModel.LaneText));
            }
            if (difficultyLabel != null)
            {
                difficultyLabel.text = ViewModel.DifficultyText;
                difficultyLabel.EnableInClassList("hidden", string.IsNullOrEmpty(ViewModel.DifficultyText));
            }
            RefreshPowerSlots();
        }

        private void RefreshPowerSlots()
        {
            for (int i = 0; i < 3; i++)
            {
                var slot = powerSlots[i];
                var label = powerLabels[i];
                if (slot == null) continue;

                var type = slotTypes[i];
                bool visible = type.HasValue && ViewModel.PowerUpTimers.ContainsKey(type.Value);
                slot.EnableInClassList("visible", visible);
                if (!visible || !type.HasValue)
                {
                    slot.RemoveFromClassList("flashing");
                    continue;
                }

                float rem = ViewModel.PowerUpTimers[type.Value];
                string shortName = type.Value switch
                {
                    PowerUpType.Shield => "SH",
                    PowerUpType.SlowMotion => "SL",
                    PowerUpType.DoublePoints => "2X",
                    _ => "?"
                };
                if (label != null)
                {
                    label.text = float.IsInfinity(rem) || rem > 900f
                        ? shortName
                        : shortName + "\n" + Mathf.CeilToInt(rem);
                }
                slot.EnableInClassList("flashing", rem < 3f && rem > 0f && Mathf.FloorToInt(Time.time * 4f) % 2 == 0);
            }
        }

        public void SetQuestion(string text, bool visible)
        {
            ViewModel.QuestionText = text;
            ViewModel.QuestionVisible = visible;
        }

        public void SetCountdown(string text, bool visible)
        {
            ViewModel.CountdownText = text;
            ViewModel.CountdownVisible = visible;
        }

        public void SetLane(string text) => ViewModel.LaneText = text;
        public void SetDifficulty(string text) => ViewModel.DifficultyText = text;
    }
}
