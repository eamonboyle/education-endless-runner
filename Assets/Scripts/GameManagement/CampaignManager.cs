using System;
using UnityEngine;
using MathRunner.Core;

/// <summary>
/// Singleton that manages a 30-level campaign mode. Levels are generated
/// procedurally with increasing difficulty. Completion and star ratings
/// are persisted via PlayerPrefs.
/// </summary>
public class CampaignManager : MonoBehaviour
{
    #region Singleton

    /// <summary>Global singleton instance.</summary>
    public static CampaignManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #endregion

    /// <summary>Total number of campaign levels.</summary>
    public const int TotalLevels = 30;

    /// <summary>Number of questions per campaign level.</summary>
    public const int QuestionsPerLevel = 10;

    /// <summary>
    /// Data container describing a single campaign level's configuration.
    /// </summary>
    [Serializable]
    public class CampaignLevel
    {
        /// <summary>One-based level number.</summary>
        public int LevelNumber;

        /// <summary>Number of questions in this level.</summary>
        public int QuestionCount;

        /// <summary>Correct answers needed for 1 star.</summary>
        public int RequiredCorrectOneStar;

        /// <summary>Correct answers needed for 2 stars.</summary>
        public int RequiredCorrectTwoStars;

        /// <summary>Correct answers needed for 3 stars.</summary>
        public int RequiredCorrectThreeStars;

        /// <summary>Speed multiplier applied at the start of this level.</summary>
        public float SpeedMultiplier;

        /// <summary>Math operation mode for this level.</summary>
        public MathMode MathMode;
    }

    /// <summary>
    /// Returns the <see cref="CampaignLevel"/> configuration for the specified
    /// <paramref name="level"/> number (1-based).
    /// </summary>
    /// <param name="level">Level number (1–30).</param>
    /// <returns>The generated level configuration.</returns>
    public static CampaignLevel GetLevelConfig(int level)
    {
        level = Mathf.Clamp(level, 1, TotalLevels);

        var config = new CampaignLevel
        {
            LevelNumber = level,
            QuestionCount = QuestionsPerLevel,
            RequiredCorrectOneStar = Mathf.Max(1, QuestionsPerLevel / 2),
            RequiredCorrectTwoStars = Mathf.CeilToInt(QuestionsPerLevel * 0.7f),
            RequiredCorrectThreeStars = Mathf.CeilToInt(QuestionsPerLevel * 0.9f),
            SpeedMultiplier = 1.0f + (level - 1) * 0.05f,
            MathMode = GetMathModeForLevel(level)
        };

        return config;
    }

    /// <summary>Returns the highest unlocked level (1-based). Level 1 is always unlocked.</summary>
    public static int GetCurrentLevel()
    {
        int progress = PlayerPrefs.GetInt(GameConstants.PREF_CAMPAIGN_PROGRESS, 1);
        return Mathf.Clamp(progress, 1, TotalLevels);
    }

    /// <summary>
    /// Records the completion of a campaign level. Awards stars based on the
    /// number of correct answers and unlocks the next level if at least 1 star
    /// is earned.
    /// </summary>
    /// <param name="level">The level that was completed (1-based).</param>
    /// <param name="correct">Number of correct answers.</param>
    /// <param name="total">Total number of questions.</param>
    /// <returns>Number of stars earned (0–3).</returns>
    public static int CompleteLevel(int level, int correct, int total)
    {
        level = Mathf.Clamp(level, 1, TotalLevels);
        CampaignLevel config = GetLevelConfig(level);

        int stars = 0;
        if (correct >= config.RequiredCorrectOneStar) stars = 1;
        if (correct >= config.RequiredCorrectTwoStars) stars = 2;
        if (correct >= config.RequiredCorrectThreeStars) stars = 3;

        int previousBest = GetStars(level);
        if (stars > previousBest)
        {
            PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_STARS_PREFIX + level, stars);
        }

        if (stars >= 1 && level < TotalLevels)
        {
            int currentProgress = PlayerPrefs.GetInt(GameConstants.PREF_CAMPAIGN_PROGRESS, 1);
            if (level + 1 > currentProgress)
            {
                PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_PROGRESS, level + 1);
            }
        }

        PlayerPrefs.Save();
        return stars;
    }

    /// <summary>
    /// Returns the best star rating achieved on the specified <paramref name="level"/>.
    /// </summary>
    /// <param name="level">Level number (1-based).</param>
    /// <returns>Star count (0–3).</returns>
    public static int GetStars(int level)
    {
        return PlayerPrefs.GetInt(GameConstants.PREF_CAMPAIGN_STARS_PREFIX + level, 0);
    }

    /// <summary>
    /// Returns the sum of all stars earned across every campaign level.
    /// </summary>
    public static int GetTotalStars()
    {
        int total = 0;
        for (int i = 1; i <= TotalLevels; i++)
        {
            total += GetStars(i);
        }
        return total;
    }

    /// <summary>
    /// Returns <c>true</c> if the specified <paramref name="level"/> is unlocked.
    /// Level 1 is always unlocked; subsequent levels require at least 1 star on
    /// the previous level.
    /// </summary>
    /// <param name="level">Level number (1-based).</param>
    public static bool IsLevelUnlocked(int level)
    {
        if (level <= 1) return true;
        int currentProgress = PlayerPrefs.GetInt(GameConstants.PREF_CAMPAIGN_PROGRESS, 1);
        return level <= currentProgress;
    }

    private static MathMode GetMathModeForLevel(int level)
    {
        if (level <= 10)
        {
            return MathMode.Addition;
        }
        else if (level <= 20)
        {
            switch ((level - 11) % 3)
            {
                case 0: return MathMode.Addition;
                case 1: return MathMode.Subtraction;
                default: return MathMode.Mixed;
            }
        }
        else
        {
            return MathMode.Mixed;
        }
    }
}
