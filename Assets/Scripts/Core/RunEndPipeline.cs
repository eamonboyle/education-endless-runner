using System;
using MathRunner.Core;
using MathRunner.Data;
using UnityEngine;

/// <summary>
/// Single end-of-run progression pipeline. Called from
/// <see cref="GameState.ShowGameOverUI"/> so every game-over path awards
/// XP, achievements, stats, and unlocks exactly once.
/// </summary>
public static class RunEndPipeline
{
    private static bool processed;

    /// <summary>Resets the once-per-run guard. Call from <see cref="GameState.Init"/>.</summary>
    public static void Reset()
    {
        processed = false;
    }

    /// <summary>
    /// Processes end-of-run rewards. Idempotent for a single run.
    /// </summary>
    public static void Process()
    {
        if (processed) return;
        processed = true;

        string mode = GameState.GetQuestionType();
        int score = GameState.GetScore();
        int correct = GameState.GetCorrectAnswersThisGame();
        int questions = GameState.GetQuestionsAnsweredThisGame();
        float duration = GameState.GetGameDuration();
        int bestStreak = ComboSystem.Instance != null ? ComboSystem.Instance.GetBestStreak() : 0;

        PlayerStats.RecordGamePlayed(mode);
        PlayerStats.RecordTimePlayed(duration);
        PlayerStats.ResetCurrentStreak();

        int xpEarned = XPSystem.CalculateGameXP(score, GameState.GetAccuracyThisGame(), bestStreak);
        if (xpEarned < 1) xpEarned = 1;
        XPSystem.AddXP(xpEarned);
        RewardAnimation.PlayXPBarFill(XPSystem.GetXPProgress());

        var newlyUnlocked = AchievementData.CheckAchievements();
        if (AchievementPopup.Instance != null)
        {
            foreach (var a in newlyUnlocked)
                AchievementPopup.Instance.ShowAchievement(a.Name, a.Description);
        }

        foreach (var c in CharacterUnlockSystem.CheckUnlocks())
            UnlockNotification.NotifyCharacterUnlocked(c.Name);

        if (PlayerPrefs.GetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 0) == 1)
        {
            int level = PlayerPrefs.GetInt(GameConstants.PREF_CAMPAIGN_LEVEL, CampaignManager.GetCurrentLevel());
            CampaignManager.CompleteLevel(level, correct, CampaignManager.QuestionsPerLevel);
            PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 0);
        }

        PlayerPrefs.SetInt(GameConstants.PREF_LAST_BEST_STREAK, bestStreak);
        PlayerPrefs.SetInt(GameConstants.PREF_LAST_CORRECT_ANSWERS, correct);
        PlayerPrefs.SetString(GameConstants.PREF_LAST_PLAYED_MODE, mode ?? "");
        PlayerPrefs.SetInt(GameConstants.PREF_LAST_PLAYED_SCORE, score);

        string dateKey = DateTime.UtcNow.ToString("yyyy-MM-dd");
        string scoreKey = GameConstants.PREF_DAILY_SCORE_PREFIX + dateKey;
        string qKey = GameConstants.PREF_DAILY_QUESTIONS_PREFIX + dateKey;
        int existingScore = PlayerPrefs.GetInt(scoreKey, 0);
        if (score > existingScore)
            PlayerPrefs.SetInt(scoreKey, score);
        PlayerPrefs.SetInt(qKey, PlayerPrefs.GetInt(qKey, 0) + Mathf.Max(0, questions));
        PlayerPrefs.SetInt("dailyCorrect_" + dateKey,
            PlayerPrefs.GetInt("dailyCorrect_" + dateKey, 0) + Mathf.Max(0, correct));

        AnalyticsManager.LogEvent("GameEnded", new System.Collections.Generic.Dictionary<string, string>
        {
            { "score", score.ToString() },
            { "mode", mode },
            { "duration", duration.ToString("F1") },
            { "xp", xpEarned.ToString() }
        });
    }
}
