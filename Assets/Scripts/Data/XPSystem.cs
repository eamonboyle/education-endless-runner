using System;
using UnityEngine;

namespace MathRunner.Data
{
    /// <summary>
    /// Static XP and levelling system. XP is earned after each game based on
    /// score and performance bonuses. Level thresholds follow a linear curve
    /// (level N requires N * 100 XP). All state is persisted in PlayerPrefs.
    /// </summary>
    public static class XPSystem
    {
        private const string XPKey = "xp_total";
        private const string LevelKey = "xp_level";

        /// <summary>Fired when XP is added, carrying the amount gained.</summary>
        public static event Action<int> OnXPGained;

        /// <summary>Fired when the player reaches a new level.</summary>
        public static event Action<int> OnLevelUp;

        /// <summary>
        /// Adds <paramref name="amount"/> XP and checks for level-ups.
        /// Negative values are ignored.
        /// </summary>
        /// <param name="amount">XP to add.</param>
        public static void AddXP(int amount)
        {
            if (amount <= 0) return;

            int current = GetCurrentXP();
            int previousLevel = GetCurrentLevel();

            current += amount;
            PlayerPrefs.SetInt(XPKey, current);
            PlayerPrefs.Save();

            OnXPGained?.Invoke(amount);

            int newLevel = CalculateLevel(current);
            if (newLevel > previousLevel)
            {
                PlayerPrefs.SetInt(LevelKey, newLevel);
                PlayerPrefs.Save();
                OnLevelUp?.Invoke(newLevel);
            }
        }

        /// <summary>Returns the player's total accumulated XP.</summary>
        /// <returns>Total XP.</returns>
        public static int GetCurrentXP()
        {
            return PlayerPrefs.GetInt(XPKey, 0);
        }

        /// <summary>Returns the player's current level.</summary>
        /// <returns>Current level (0-based).</returns>
        public static int GetCurrentLevel()
        {
            return CalculateLevel(GetCurrentXP());
        }

        /// <summary>
        /// Returns the total XP required to reach the next level.
        /// Level N requires N * 100 XP, so the threshold for the next
        /// level after the current one is (currentLevel + 1) * 100.
        /// </summary>
        /// <returns>XP needed for the next level.</returns>
        public static int GetXPForNextLevel()
        {
            int level = GetCurrentLevel();
            return (level + 1) * 100;
        }

        /// <summary>
        /// Returns the player's progress toward the next level as a
        /// normalised float between 0 and 1.
        /// </summary>
        /// <returns>Progress ratio (0–1).</returns>
        public static float GetXPProgress()
        {
            int xp = GetCurrentXP();
            int level = CalculateLevel(xp);
            int xpForCurrentLevel = GetCumulativeXPForLevel(level);
            int xpForNextLevel = GetCumulativeXPForLevel(level + 1);
            int range = xpForNextLevel - xpForCurrentLevel;

            if (range <= 0) return 1f;
            return Mathf.Clamp01((float)(xp - xpForCurrentLevel) / range);
        }

        /// <summary>
        /// Calculates how much XP to award for a completed game session.
        /// Base XP equals score / 10. Accuracy above 90 % grants a 50 %
        /// bonus. Each streak milestone (per 5) adds an extra 10 XP.
        /// </summary>
        /// <param name="score">Final game score.</param>
        /// <param name="accuracy">Accuracy as a 0–100 percentage.</param>
        /// <param name="bestStreak">Best streak achieved during the game.</param>
        /// <returns>Total XP to award.</returns>
        public static int CalculateGameXP(int score, float accuracy, int bestStreak)
        {
            int baseXP = Mathf.Max(score / 10, 0);

            float accuracyBonus = accuracy > 90f ? 0.5f : 0f;
            int streakBonus = Mathf.Max(bestStreak / 5, 0) * 10;

            int total = Mathf.RoundToInt(baseXP * (1f + accuracyBonus)) + streakBonus;
            return Mathf.Max(total, 0);
        }

        private static int CalculateLevel(int totalXP)
        {
            if (totalXP <= 0) return 0;

            int level = 0;
            int cumulative = 0;
            while (true)
            {
                int needed = (level + 1) * 100;
                if (cumulative + needed > totalXP) break;
                cumulative += needed;
                level++;
            }
            return level;
        }

        private static int GetCumulativeXPForLevel(int level)
        {
            if (level <= 0) return 0;
            int total = 0;
            for (int i = 1; i <= level; i++)
            {
                total += i * 100;
            }
            return total;
        }
    }
}
