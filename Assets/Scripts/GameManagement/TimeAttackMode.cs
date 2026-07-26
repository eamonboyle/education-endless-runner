using System;
using UnityEngine;
using MathRunner.Core;

/// <summary>
/// MonoBehaviour that manages the time-attack game mode. A 60-second countdown
/// timer replaces the standard lives-based game-over mechanic: wrong answers
/// subtract time instead of ending the game, and correct answers award a small
/// time bonus. The mode is toggled via PlayerPrefs.
/// </summary>
public class TimeAttackMode : MonoBehaviour
{
    #region Singleton

    /// <summary>Global singleton instance.</summary>
    public static TimeAttackMode Instance { get; private set; }

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

    /// <summary>Fired every frame the timer changes, carrying the remaining seconds.</summary>
    public event Action<float> OnTimeChanged;

    /// <summary>Fired when the timer reaches zero.</summary>
    public event Action OnTimeUp;

    #endregion

    [SerializeField, Tooltip("Starting time in seconds.")]
    private float startTime = 60f;

    [SerializeField, Tooltip("Seconds subtracted for a wrong answer.")]
    private float wrongAnswerPenalty = 5f;

    [SerializeField, Tooltip("Seconds added for a correct answer.")]
    private float correctAnswerBonus = 2f;

    [SerializeField, Tooltip("Score awarded per correct answer in time-attack mode.")]
    private int correctAnswerScore = 10;

    [SerializeField, Tooltip("Maximum timer value (cap).")]
    private float maxTime = 60f;

    private float remainingTime;
    private bool timerRunning;
    private int questionsAnswered;
    private int correctCount;

    /// <summary>
    /// Begins the time-attack countdown. Call after the pre-game countdown ends.
    /// </summary>
    public void StartTimer()
    {
        remainingTime = startTime;
        timerRunning = true;
        questionsAnswered = 0;
        correctCount = 0;
    }

    /// <summary>Returns the number of seconds remaining on the clock.</summary>
    public float GetRemainingTime()
    {
        return remainingTime;
    }

    /// <summary>
    /// Returns <c>true</c> if time-attack mode is currently enabled in PlayerPrefs.
    /// </summary>
    public static bool IsTimeAttack()
    {
        return PlayerPrefs.GetInt(GameConstants.PREF_TIME_ATTACK, 0) == 1;
    }

    /// <summary>
    /// Enables or disables time-attack mode in PlayerPrefs.
    /// </summary>
    /// <param name="enabled">Whether time-attack mode should be active.</param>
    public static void SetTimeAttack(bool enabled)
    {
        PlayerPrefs.SetInt(GameConstants.PREF_TIME_ATTACK, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>Returns the total number of questions answered during this time-attack round.</summary>
    public int GetQuestionsAnswered()
    {
        return questionsAnswered;
    }

    /// <summary>Returns the number of questions answered correctly during this time-attack round.</summary>
    public int GetCorrectCount()
    {
        return correctCount;
    }

    /// <summary>
    /// Records a correct answer: awards score and adds a time bonus (capped at <see cref="maxTime"/>).
    /// </summary>
    public void RecordCorrectAnswer()
    {
        questionsAnswered++;
        correctCount++;
        GameState.AddScore(correctAnswerScore);
        remainingTime = Mathf.Min(remainingTime + correctAnswerBonus, maxTime);
        OnTimeChanged?.Invoke(remainingTime);
    }

    /// <summary>
    /// Records a wrong answer: subtracts time but does <b>not</b> trigger game over.
    /// </summary>
    public void RecordWrongAnswer()
    {
        questionsAnswered++;
        remainingTime -= wrongAnswerPenalty;

        if (remainingTime < 0f)
            remainingTime = 0f;

        OnTimeChanged?.Invoke(remainingTime);
    }

    private void Update()
    {
        if (!timerRunning || !GameState.IsRunning()) return;

        remainingTime -= Time.deltaTime;
        OnTimeChanged?.Invoke(remainingTime);

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            timerRunning = false;
            OnTimeUp?.Invoke();
            GameState.ShowGameOverUI();
        }
    }
}
