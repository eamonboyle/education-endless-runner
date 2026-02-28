using UnityEngine;
using MathRunner.Core;

namespace MathRunner.Data
{
    /// <summary>
    /// Tracks cumulative player statistics via <see cref="PlayerPrefs"/>.
    /// All data is persisted immediately on write so it survives application
    /// termination.
    /// </summary>
    public static class PlayerStats
    {
        #region Recording

        /// <summary>
        /// Records the outcome of a single answered question.
        /// Updates per-mode and overall totals, and maintains the current streak.
        /// </summary>
        /// <param name="correct">Whether the player answered correctly.</param>
        /// <param name="mode">
        /// The mode string as stored in PlayerPrefs
        /// (e.g. <c>"addition"</c>, <c>"multiply"</c>).
        /// </param>
        public static void RecordAnswer(bool correct, string mode)
        {
            IncrementInt(GameConstants.PREF_TOTAL_QUESTIONS_PREFIX + mode);
            IncrementInt(GameConstants.PREF_TOTAL_QUESTIONS_PREFIX + "total");

            if (correct)
            {
                IncrementInt(GameConstants.PREF_CORRECT_ANSWERS_PREFIX + mode);
                IncrementInt(GameConstants.PREF_CORRECT_ANSWERS_PREFIX + "total");

                int streak = PlayerPrefs.GetInt(GameConstants.PREF_CURRENT_STREAK, 0) + 1;
                PlayerPrefs.SetInt(GameConstants.PREF_CURRENT_STREAK, streak);

                int bestStreak = PlayerPrefs.GetInt(GameConstants.PREF_BEST_STREAK_PREFIX + mode, 0);
                if (streak > bestStreak)
                {
                    PlayerPrefs.SetInt(GameConstants.PREF_BEST_STREAK_PREFIX + mode, streak);
                }

                int bestStreakTotal = PlayerPrefs.GetInt(GameConstants.PREF_BEST_STREAK_PREFIX + "total", 0);
                if (streak > bestStreakTotal)
                {
                    PlayerPrefs.SetInt(GameConstants.PREF_BEST_STREAK_PREFIX + "total", streak);
                }
            }
            else
            {
                IncrementInt(GameConstants.PREF_WRONG_ANSWERS_PREFIX + mode);
                IncrementInt(GameConstants.PREF_WRONG_ANSWERS_PREFIX + "total");
                PlayerPrefs.SetInt(GameConstants.PREF_CURRENT_STREAK, 0);
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Records a completed game session for the given <paramref name="mode"/>.
        /// </summary>
        /// <param name="mode">The mode string.</param>
        public static void RecordGamePlayed(string mode)
        {
            IncrementInt(GameConstants.PREF_GAMES_PLAYED);
            IncrementInt(GameConstants.PREF_GAMES_PLAYED_PREFIX + mode);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Adds <paramref name="seconds"/> to the total time-played counter.
        /// </summary>
        /// <param name="seconds">Elapsed time in seconds.</param>
        public static void RecordTimePlayed(float seconds)
        {
            float total = PlayerPrefs.GetFloat(GameConstants.PREF_TOTAL_TIME_PLAYED, 0f) + seconds;
            PlayerPrefs.SetFloat(GameConstants.PREF_TOTAL_TIME_PLAYED, total);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Resets the current answer streak to zero.
        /// Call this at the start of each new game session.
        /// </summary>
        public static void ResetCurrentStreak()
        {
            PlayerPrefs.SetInt(GameConstants.PREF_CURRENT_STREAK, 0);
        }

        #endregion

        #region Queries

        /// <summary>
        /// Returns the total number of questions answered in the given
        /// <paramref name="mode"/>.
        /// </summary>
        /// <param name="mode">Mode string, or <c>"total"</c> for all modes.</param>
        /// <returns>Total questions answered.</returns>
        public static int GetTotalQuestions(string mode)
        {
            return PlayerPrefs.GetInt(GameConstants.PREF_TOTAL_QUESTIONS_PREFIX + mode, 0);
        }

        /// <summary>
        /// Returns the number of correct answers for the given
        /// <paramref name="mode"/>.
        /// </summary>
        /// <param name="mode">Mode string, or <c>"total"</c> for all modes.</param>
        /// <returns>Number of correct answers.</returns>
        public static int GetCorrectAnswers(string mode)
        {
            return PlayerPrefs.GetInt(GameConstants.PREF_CORRECT_ANSWERS_PREFIX + mode, 0);
        }

        /// <summary>
        /// Returns the number of wrong answers for the given
        /// <paramref name="mode"/>.
        /// </summary>
        /// <param name="mode">Mode string, or <c>"total"</c> for all modes.</param>
        /// <returns>Number of wrong answers.</returns>
        public static int GetWrongAnswers(string mode)
        {
            return PlayerPrefs.GetInt(GameConstants.PREF_WRONG_ANSWERS_PREFIX + mode, 0);
        }

        /// <summary>
        /// Returns the best answer streak recorded for the given
        /// <paramref name="mode"/>.
        /// </summary>
        /// <param name="mode">Mode string, or <c>"total"</c> for all modes.</param>
        /// <returns>Best streak count.</returns>
        public static int GetBestStreak(string mode)
        {
            return PlayerPrefs.GetInt(GameConstants.PREF_BEST_STREAK_PREFIX + mode, 0);
        }

        /// <summary>
        /// Returns the current in-session answer streak.
        /// </summary>
        /// <returns>Current streak count.</returns>
        public static int GetCurrentStreak()
        {
            return PlayerPrefs.GetInt(GameConstants.PREF_CURRENT_STREAK, 0);
        }

        /// <summary>
        /// Returns the total time played across all sessions, in seconds.
        /// </summary>
        /// <returns>Total play time in seconds.</returns>
        public static float GetTotalTimePlayed()
        {
            return PlayerPrefs.GetFloat(GameConstants.PREF_TOTAL_TIME_PLAYED, 0f);
        }

        /// <summary>
        /// Returns the number of games played for the given
        /// <paramref name="mode"/>.
        /// </summary>
        /// <param name="mode">Mode string.</param>
        /// <returns>Number of games played.</returns>
        public static int GetGamesPlayed(string mode)
        {
            return PlayerPrefs.GetInt(GameConstants.PREF_GAMES_PLAYED_PREFIX + mode, 0);
        }

        /// <summary>
        /// Returns the total number of games played across all modes.
        /// </summary>
        /// <returns>Total games played.</returns>
        public static int GetTotalGamesPlayed()
        {
            return PlayerPrefs.GetInt(GameConstants.PREF_GAMES_PLAYED, 0);
        }

        /// <summary>
        /// Returns the player's answer accuracy for the given
        /// <paramref name="mode"/> as a value between 0 and 1.
        /// Returns 0 if no questions have been answered.
        /// </summary>
        /// <param name="mode">Mode string.</param>
        /// <returns>Accuracy ratio (0–1).</returns>
        public static float GetAccuracy(string mode)
        {
            int total = GetTotalQuestions(mode);
            if (total == 0) return 0f;
            return (float)GetCorrectAnswers(mode) / total;
        }

        /// <summary>
        /// Returns the overall accuracy across all modes as a value
        /// between 0 and 1. Returns 0 if no questions have been answered.
        /// </summary>
        /// <returns>Overall accuracy ratio (0–1).</returns>
        public static float GetAccuracyTotal()
        {
            return GetAccuracy("total");
        }

        #endregion

        #region Helpers

        private static void IncrementInt(string key)
        {
            int value = PlayerPrefs.GetInt(key, 0);
            PlayerPrefs.SetInt(key, value + 1);
        }

        #endregion
    }
}
