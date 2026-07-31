using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Properties;

namespace MathRunner.UI.ViewModels
{
    /// <summary>
    /// Bindable HUD state bridged from GameState / ComboSystem / LivesSystem / PowerUpSystem.
    /// </summary>
    public class RunHudViewModel
    {
        private int score;
        private int multiplier = 1;
        private int lives = 3;
        private int maxLives = 3;
        private int speed;
        private string questionText = "";
        private string countdownText = "";
        private string laneText = "";
        private string difficultyText = "";
        private bool questionVisible;
        private bool countdownVisible;
        private readonly Dictionary<PowerUpType, float> powerUpTimers = new Dictionary<PowerUpType, float>();

        public event Action Changed;

        [CreateProperty]
        public int Score
        {
            get => score;
            set { if (score == value) return; score = value; Notify(); }
        }

        [CreateProperty]
        public int Multiplier
        {
            get => multiplier;
            set { if (multiplier == value) return; multiplier = value; Notify(); }
        }

        [CreateProperty]
        public int Lives
        {
            get => lives;
            set { if (lives == value) return; lives = value; Notify(); }
        }

        [CreateProperty]
        public int MaxLives
        {
            get => maxLives;
            set { if (maxLives == value) return; maxLives = value; Notify(); }
        }

        [CreateProperty]
        public int Speed
        {
            get => speed;
            set { if (speed == value) return; speed = value; Notify(); }
        }

        [CreateProperty]
        public string QuestionText
        {
            get => questionText;
            set { if (questionText == value) return; questionText = value ?? ""; Notify(); }
        }

        [CreateProperty]
        public bool QuestionVisible
        {
            get => questionVisible;
            set { if (questionVisible == value) return; questionVisible = value; Notify(); }
        }

        [CreateProperty]
        public string CountdownText
        {
            get => countdownText;
            set { if (countdownText == value) return; countdownText = value ?? ""; Notify(); }
        }

        [CreateProperty]
        public bool CountdownVisible
        {
            get => countdownVisible;
            set { if (countdownVisible == value) return; countdownVisible = value; Notify(); }
        }

        [CreateProperty]
        public string LaneText
        {
            get => laneText;
            set { if (laneText == value) return; laneText = value ?? ""; Notify(); }
        }

        [CreateProperty]
        public string DifficultyText
        {
            get => difficultyText;
            set { if (difficultyText == value) return; difficultyText = value ?? ""; Notify(); }
        }

        public IReadOnlyDictionary<PowerUpType, float> PowerUpTimers => powerUpTimers;

        public string LivesDisplay
        {
            get
            {
                int max = maxLives > 0 ? maxLives : 3;
                var hearts = "";
                for (int i = 0; i < max; i++)
                    hearts += i < lives ? "\u2665 " : "\u2661 ";
                return hearts.TrimEnd();
            }
        }

        public string ScoreDisplay
        {
            get
            {
                if (multiplier > 1)
                    return score + "  x" + multiplier + "!";
                return score.ToString();
            }
        }

        public string ComboDisplay => multiplier > 1 ? "x" + multiplier + "!" : "";

        public void SetPowerUp(PowerUpType type, float remaining)
        {
            if (remaining <= 0f)
                powerUpTimers.Remove(type);
            else
                powerUpTimers[type] = remaining;
            Notify();
        }

        public void ClearPowerUps()
        {
            powerUpTimers.Clear();
            Notify();
        }

        private void Notify() => Changed?.Invoke();
    }
}
