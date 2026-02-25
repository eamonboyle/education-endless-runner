using System;
using UnityEngine;

/// <summary>
/// Singleton that tracks consecutive-correct-answer streaks and applies
/// score multipliers.  Persists the best streak across sessions via PlayerPrefs.
/// </summary>
public class ComboSystem : MonoBehaviour
{
    #region Singleton
    /// <summary>Global singleton instance.</summary>
    public static ComboSystem Instance { get; private set; }

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

    #region Events
    /// <summary>Fired whenever the streak count changes.</summary>
    public event Action<int> OnStreakChanged;

    /// <summary>Fired whenever the multiplier tier changes.</summary>
    public event Action<int> OnMultiplierChanged;

    /// <summary>Fired when a streak is broken, carrying the final streak value.</summary>
    public event Action<int> OnStreakBroken;
    #endregion

    private const string BestStreakKey = "BestStreak";

    private int currentStreak;
    private int bestStreak;
    private int lastMultiplier = 1;

    private void Start()
    {
        bestStreak = PlayerPrefs.GetInt(BestStreakKey, 0);
    }

    /// <summary>
    /// Call when the player answers a question correctly.
    /// Increments the streak, updates the multiplier, awards bonus score.
    /// </summary>
    public void RecordCorrectAnswer()
    {
        currentStreak++;

        if (currentStreak > bestStreak)
        {
            bestStreak = currentStreak;
            PlayerPrefs.SetInt(BestStreakKey, bestStreak);
        }

        OnStreakChanged?.Invoke(currentStreak);
        UpdateMultiplier();

        int bonus = GameConstants.BASE_CORRECT_POINTS * GetMultiplier();
        GameState.SetScore(GameState.GetScore() + bonus);
    }

    /// <summary>
    /// Call when the player answers incorrectly.
    /// Breaks the streak and resets the multiplier.
    /// </summary>
    public void RecordWrongAnswer()
    {
        int finalStreak = currentStreak;
        currentStreak = 0;

        OnStreakBroken?.Invoke(finalStreak);
        OnStreakChanged?.Invoke(currentStreak);
        UpdateMultiplier();
    }

    /// <summary>Returns the current consecutive-correct streak.</summary>
    public int GetCurrentStreak()
    {
        return currentStreak;
    }

    /// <summary>
    /// Returns the score multiplier for the current streak.
    /// 1x for 0-2, 2x for 3-5, 3x for 6-9, 4x for 10+.
    /// </summary>
    public int GetMultiplier()
    {
        if (currentStreak >= 10) return 4;
        if (currentStreak >= 6)  return 3;
        if (currentStreak >= 3)  return 2;
        return 1;
    }

    /// <summary>Returns the best streak ever achieved (persisted in PlayerPrefs).</summary>
    public int GetBestStreak()
    {
        return bestStreak;
    }

    private void UpdateMultiplier()
    {
        int current = GetMultiplier();
        if (current != lastMultiplier)
        {
            lastMultiplier = current;
            OnMultiplierChanged?.Invoke(current);
        }
    }
}
