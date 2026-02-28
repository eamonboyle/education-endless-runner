using System;
using System.Collections.Generic;
using UnityEngine;
using MathRunner.Core;

namespace MathRunner.Data
{
    /// <summary>
    /// Represents a single unlockable achievement.
    /// </summary>
    [Serializable]
    public class Achievement
    {
        /// <summary>Unique identifier used as the PlayerPrefs key suffix.</summary>
        public string Id { get; private set; }

        /// <summary>Human-readable achievement name.</summary>
        public string Name { get; private set; }

        /// <summary>Short description shown to the player.</summary>
        public string Description { get; private set; }

        /// <summary>Delegate that evaluates whether the unlock condition is currently met.</summary>
        public Func<bool> UnlockCondition { get; private set; }

        /// <summary>Whether the achievement has been permanently unlocked.</summary>
        public bool IsUnlocked
        {
            get { return PlayerPrefs.GetInt(GameConstants.PREF_ACHIEVEMENT_PREFIX + Id, 0) == 1; }
        }

        /// <summary>
        /// Creates a new achievement definition.
        /// </summary>
        /// <param name="id">Unique string identifier.</param>
        /// <param name="name">Display name.</param>
        /// <param name="description">Description text.</param>
        /// <param name="unlockCondition">Predicate that returns true when the achievement criteria are met.</param>
        public Achievement(string id, string name, string description, Func<bool> unlockCondition)
        {
            Id = id;
            Name = name;
            Description = description;
            UnlockCondition = unlockCondition;
        }

        /// <summary>
        /// Persists the unlocked state to PlayerPrefs.
        /// </summary>
        public void Unlock()
        {
            PlayerPrefs.SetInt(GameConstants.PREF_ACHIEVEMENT_PREFIX + Id, 1);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Static registry and evaluator for all in-game achievements.
    /// Achievement definitions are created once and evaluated on demand.
    /// </summary>
    public static class AchievementData
    {
        private static List<Achievement> allAchievements;

        /// <summary>
        /// Returns the master list of all achievement definitions.
        /// The list is lazily initialised on first access.
        /// </summary>
        public static List<Achievement> All
        {
            get
            {
                if (allAchievements == null)
                {
                    InitAchievements();
                }
                return allAchievements;
            }
        }

        /// <summary>
        /// Evaluates every achievement's unlock condition.
        /// Newly-qualifying achievements are unlocked and persisted.
        /// </summary>
        /// <returns>A list of achievements that were unlocked during this call.</returns>
        public static List<Achievement> CheckAchievements()
        {
            List<Achievement> newlyUnlocked = new List<Achievement>();

            foreach (Achievement a in All)
            {
                if (!a.IsUnlocked && a.UnlockCondition != null && a.UnlockCondition())
                {
                    a.Unlock();
                    newlyUnlocked.Add(a);
                }
            }

            return newlyUnlocked;
        }

        /// <summary>
        /// Returns all achievements that the player has already unlocked.
        /// </summary>
        public static List<Achievement> GetUnlockedAchievements()
        {
            List<Achievement> list = new List<Achievement>();
            foreach (Achievement a in All)
            {
                if (a.IsUnlocked)
                {
                    list.Add(a);
                }
            }
            return list;
        }

        /// <summary>
        /// Returns all achievements that have not yet been unlocked.
        /// </summary>
        public static List<Achievement> GetLockedAchievements()
        {
            List<Achievement> list = new List<Achievement>();
            foreach (Achievement a in All)
            {
                if (!a.IsUnlocked)
                {
                    list.Add(a);
                }
            }
            return list;
        }

        private static void InitAchievements()
        {
            allAchievements = new List<Achievement>
            {
                new Achievement(
                    "first_game",
                    "First Steps",
                    "Play your first game.",
                    () => PlayerStats.GetTotalGamesPlayed() >= 1
                ),

                new Achievement(
                    "score_100",
                    "Century",
                    "Score 100 or more in a single game.",
                    () => GameState.GetScore() >= 100
                ),

                new Achievement(
                    "score_500",
                    "High Roller",
                    "Score 500 or more in a single game.",
                    () => GameState.GetScore() >= 500
                ),

                new Achievement(
                    "streak_5",
                    "On a Roll",
                    "Get a streak of 5 correct answers.",
                    () => PlayerStats.GetBestStreak("total") >= 5
                ),

                new Achievement(
                    "streak_10",
                    "Unstoppable",
                    "Get a streak of 10 correct answers.",
                    () => PlayerStats.GetBestStreak("total") >= 10
                ),

                new Achievement(
                    "all_modes",
                    "Well Rounded",
                    "Play at least one game in every mode.",
                    () => PlayerStats.GetGamesPlayed("addition") >= 1
                       && PlayerStats.GetGamesPlayed("subtraction") >= 1
                       && PlayerStats.GetGamesPlayed("multiply") >= 1
                       && PlayerStats.GetGamesPlayed("division") >= 1
                ),

                new Achievement(
                    "perfect_10",
                    "Perfect 10",
                    "Answer 10 questions in a row correctly.",
                    () => PlayerStats.GetCurrentStreak() >= 10
                ),

                new Achievement(
                    "math_whiz",
                    "Math Whiz",
                    "Answer 500 questions correctly across all games.",
                    () => PlayerStats.GetCorrectAnswers("total") >= 500
                ),

                new Achievement(
                    "speed_demon",
                    "Speed Demon",
                    "Score 100 in under 60 seconds.",
                    () => GameState.GetScore() >= 100
                       && PlayerStats.GetTotalTimePlayed() > 0f
                       && PlayerStats.GetTotalTimePlayed() < 60f
                ),

                new Achievement(
                    "dedicated",
                    "Dedicated",
                    "Play 50 games.",
                    () => PlayerStats.GetTotalGamesPlayed() >= 50
                ),

                new Achievement(
                    "sharp_mind",
                    "Sharp Mind",
                    "Achieve 90% accuracy over at least 100 questions.",
                    () => PlayerStats.GetTotalQuestions("total") >= 100
                       && PlayerStats.GetAccuracyTotal() >= 0.9f
                )
            };
        }
    }
}
