using System;
using UnityEngine;
using MathRunner.Core;

/// <summary>
/// Singleton that manages a lives/hearts system. The number of starting lives
/// depends on the selected <see cref="DifficultyLevel"/>. Fires events when
/// lives are lost so UI and game-over logic can respond.
/// </summary>
public class LivesSystem : MonoBehaviour
{
    #region Singleton

    /// <summary>Global singleton instance.</summary>
    public static LivesSystem Instance { get; private set; }

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

    /// <summary>Fired when a life is lost. Parameter is the number of lives remaining.</summary>
    public event Action<int> OnLifeLost;

    /// <summary>Fired when all lives have been lost.</summary>
    public event Action OnAllLivesLost;

    #endregion

    [SerializeField, Tooltip("Default number of starting lives (used when difficulty is Medium).")]
    private int defaultLives = 3;

    [SerializeField, Tooltip("UI container that holds heart icons. Assign in Inspector.")]
    private Transform heartContainer;

    private int currentLives;
    private int maxLives;

    private void Start()
    {
        ResetLives();
    }

    /// <summary>
    /// Resets lives to the maximum for the current difficulty.
    /// Call at the start of each game.
    /// </summary>
    public void ResetLives()
    {
        maxLives = GetMaxLivesForDifficulty(DifficultyPresets.GetDifficulty());
        currentLives = maxLives;
    }

    /// <summary>
    /// Decrements the life counter by one.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the player still has lives remaining;
    /// <c>false</c> if all lives are exhausted (game over).
    /// </returns>
    public bool LoseLife()
    {
        if (currentLives <= 0) return false;

        currentLives--;
        OnLifeLost?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            OnAllLivesLost?.Invoke();
            return false;
        }

        return true;
    }

    /// <summary>Returns the number of lives the player currently has.</summary>
    public int GetLives()
    {
        return currentLives;
    }

    /// <summary>Returns the maximum number of lives for the current difficulty.</summary>
    public int GetMaxLives()
    {
        return maxLives;
    }

    /// <summary>Returns the UI container assigned for heart icons (may be null).</summary>
    public Transform GetHeartContainer()
    {
        return heartContainer;
    }

    private int GetMaxLivesForDifficulty(DifficultyLevel level)
    {
        switch (level)
        {
            case DifficultyLevel.Easy: return 5;
            case DifficultyLevel.Hard: return 1;
            case DifficultyLevel.Medium:
            default: return defaultLives;
        }
    }
}
