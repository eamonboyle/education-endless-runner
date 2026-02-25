using UnityEngine;
using MathRunner.Core;

/// <summary>
/// Singleton that manages difficulty presets. Each <see cref="DifficultyLevel"/>
/// maps to a set of tuning values (speed multiplier, wrong-answer range multiplier,
/// speed-increase rate). The selected difficulty is persisted in PlayerPrefs via
/// <see cref="GameConstants.PREF_DIFFICULTY"/>.
/// </summary>
public class DifficultyPresets : MonoBehaviour
{
    #region Singleton

    /// <summary>Global singleton instance.</summary>
    public static DifficultyPresets Instance { get; private set; }

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

    #region Preset Data

    [System.Serializable]
    private struct Preset
    {
        public float speedMultiplier;
        public float wrongAnswerRangeMultiplier;
        public float speedIncreaseRate;
    }

    private static readonly Preset EasyPreset = new Preset
    {
        speedMultiplier = 0.7f,
        wrongAnswerRangeMultiplier = 1.5f,
        speedIncreaseRate = 15.0f
    };

    private static readonly Preset MediumPreset = new Preset
    {
        speedMultiplier = 1.0f,
        wrongAnswerRangeMultiplier = 1.0f,
        speedIncreaseRate = 10.0f
    };

    private static readonly Preset HardPreset = new Preset
    {
        speedMultiplier = 1.3f,
        wrongAnswerRangeMultiplier = 0.6f,
        speedIncreaseRate = 7.0f
    };

    #endregion

    private void Start()
    {
        ApplyDifficulty();
    }

    /// <summary>
    /// Persists the chosen <paramref name="level"/> and immediately applies
    /// the corresponding speed multiplier to <see cref="GameState"/>.
    /// </summary>
    /// <param name="level">The desired difficulty tier.</param>
    public static void SetDifficulty(DifficultyLevel level)
    {
        PlayerPrefs.SetInt(GameConstants.PREF_DIFFICULTY, (int)level);
        PlayerPrefs.Save();

        if (Instance != null)
        {
            Instance.ApplyDifficulty();
        }
    }

    /// <summary>
    /// Returns the currently selected <see cref="DifficultyLevel"/>
    /// (defaults to <see cref="DifficultyLevel.Medium"/>).
    /// </summary>
    public static DifficultyLevel GetDifficulty()
    {
        int stored = PlayerPrefs.GetInt(GameConstants.PREF_DIFFICULTY, (int)DifficultyLevel.Medium);
        if (stored < 0 || stored > 2) stored = (int)DifficultyLevel.Medium;
        return (DifficultyLevel)stored;
    }

    /// <summary>
    /// Returns the initial-speed multiplier for the current difficulty.
    /// Applied to <see cref="GameConstants.DEFAULT_SPEED"/> at game start.
    /// </summary>
    public static float GetSpeedMultiplier()
    {
        return GetPreset(GetDifficulty()).speedMultiplier;
    }

    /// <summary>
    /// Returns the wrong-answer-range multiplier for the current difficulty.
    /// Higher values make wrong answers easier to distinguish from the correct one.
    /// </summary>
    public static float GetWrongAnswerRangeMultiplier()
    {
        return GetPreset(GetDifficulty()).wrongAnswerRangeMultiplier;
    }

    /// <summary>
    /// Returns the speed-increase rate divisor for the current difficulty.
    /// Lower values cause speed to ramp up faster.
    /// </summary>
    public static float GetSpeedIncreaseRate()
    {
        return GetPreset(GetDifficulty()).speedIncreaseRate;
    }

    private void ApplyDifficulty()
    {
        float baseSpeed = GameConstants.DEFAULT_SPEED * GetSpeedMultiplier();
        GameState.SetCharacterSpeed(baseSpeed);
    }

    private static Preset GetPreset(DifficultyLevel level)
    {
        switch (level)
        {
            case DifficultyLevel.Easy:   return EasyPreset;
            case DifficultyLevel.Hard:   return HardPreset;
            case DifficultyLevel.Medium:
            default:                     return MediumPreset;
        }
    }
}
