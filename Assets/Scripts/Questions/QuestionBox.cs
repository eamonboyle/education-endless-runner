using System.Collections.Generic;
using UnityEngine;
using MathRunner.Core;
using MathRunner.Data;

public class QuestionBox : MonoBehaviour
{
    public int number;
    public int correctNumber;
    public string questionText;

    private static PowerUpSpawner cachedPowerUpSpawner;
    private static bool powerUpSpawnerLookupDone;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameObject questionManager = GameObject.Find("QuestionManager");
        if (questionManager == null) return;

        QuestionGeneration questionGeneration = questionManager.GetComponent<QuestionGeneration>();
        if (questionGeneration == null) return;
        if (questionGeneration.questionBoxes.Count < 3) return;

        Destroy(questionGeneration.questionBoxes[0].gameObject);
        Destroy(questionGeneration.questionBoxes[1].gameObject);
        Destroy(questionGeneration.questionBoxes[2].gameObject);
        questionGeneration.questionBoxes.RemoveRange(0, 3);

        bool isCorrect = (number == correctNumber);
        GameState.RecordAnswer(isCorrect);

        string mode = GameState.GetQuestionType();
        PlayerStats.RecordAnswer(isCorrect, mode);
        WeeklyChallengeData.RecordProgress(mode);
        AnalyticsManager.LogEvent("QuestionAnswered", new Dictionary<string, string> {
            { "correct", isCorrect.ToString() },
            { "mode", mode }
        });

        bool isTimeAttack = TimeAttackMode.IsTimeAttack();

        if (!isCorrect)
        {
            HandleIncorrectAnswer(questionGeneration, isTimeAttack);
            return;
        }

        HandleCorrectAnswer(questionGeneration, isTimeAttack);
    }

    private void HandleIncorrectAnswer(QuestionGeneration qg, bool isTimeAttack)
    {
        var powerUpSystem = PowerUpSystem.Instance;
        if (powerUpSystem != null && powerUpSystem.HasActivePowerUp(PowerUpType.Shield))
        {
            powerUpSystem.DeactivatePowerUp(PowerUpType.Shield);
            AnswerFeedback.PlayIncorrect(transform.position);
            HapticFeedback.VibrateOnWrongAnswer();
            var combo = ComboSystem.Instance;
            if (combo != null) combo.RecordWrongAnswer();
            QuestionHistoryDisplay.RecordQuestion(questionText, number, correctNumber);
            // Same advance path as a correct answer: spawn the next row and drop
            // questions[0] once. Calling DeleteLastQuestion here as well would
            // desync the HUD text from the boxes still on the track.
            qg.AddQuestion(true);
            return;
        }

        AnswerFeedback.PlayIncorrect(transform.position);
        ScreenFlash.FlashRed();
        HapticFeedback.VibrateOnWrongAnswer();

        if (ScreenShake.Instance != null)
            ScreenShake.Instance.MediumShake();

        var comboSystem = ComboSystem.Instance;
        if (comboSystem != null) comboSystem.RecordWrongAnswer();

        if (isTimeAttack)
        {
            var timeAttack = TimeAttackMode.Instance;
            if (timeAttack != null) timeAttack.RecordWrongAnswer();
            QuestionHistoryDisplay.RecordQuestion(questionText, number, correctNumber);
            qg.AddQuestion(true);
            return;
        }

        var livesSystem = LivesSystem.Instance;
        if (livesSystem != null && livesSystem.GetLives() > 0)
        {
            bool alive = livesSystem.LoseLife();
            if (alive)
            {
                QuestionHistoryDisplay.RecordQuestion(questionText, number, correctNumber);
                qg.AddQuestion(true);
                return;
            }
        }

        QuestionHistoryDisplay.RecordQuestion(questionText, number, correctNumber);
        // Boxes for this question were already destroyed above. Drop it from the
        // buffer so Continue doesn't show its text against the next row's boxes.
        qg.DeleteOldestQuestion();
        AnsweredIncorrectly();
    }

    private void HandleCorrectAnswer(QuestionGeneration qg, bool isTimeAttack)
    {
        AnswerFeedback.PlayCorrect(transform.position);
        ScreenFlash.FlashGreen();
        QuestionHistoryDisplay.RecordQuestion(questionText, number, correctNumber);

        var audioSource = Camera.main != null ? Camera.main.GetComponent<AudioSource>() : null;
        if (audioSource != null) audioSource.Play();

        var combo = ComboSystem.Instance;
        int bonusPoints = 10;
        if (combo != null)
        {
            combo.RecordCorrectAnswer();
            bonusPoints *= combo.GetMultiplier();
        }

        var powerUp = PowerUpSystem.Instance;
        if (powerUp != null && powerUp.HasActivePowerUp(PowerUpType.DoublePoints))
            bonusPoints *= 2;

        if (isTimeAttack)
        {
            var timeAttack = TimeAttackMode.Instance;
            if (timeAttack != null) timeAttack.RecordCorrectAnswer();
        }

        GameState.AddScore(bonusPoints);
        ScorePopup.Create(transform.position + Vector3.up * 2f, bonusPoints, null);

        DailyChallengeData.RecordProgress(GameState.GetQuestionType());
        WeeklyChallengeData.RecordProgress(GameState.GetQuestionType());

        qg.AddQuestion(true);

        PowerUpSpawner spawner = GetPowerUpSpawner();
        if (spawner != null) spawner.TrySpawnPowerUp(transform.position);
    }

    private static PowerUpSpawner GetPowerUpSpawner()
    {
        if (!powerUpSpawnerLookupDone)
        {
            cachedPowerUpSpawner = Object.FindAnyObjectByType<PowerUpSpawner>();
            powerUpSpawnerLookupDone = true;
        }
        return cachedPowerUpSpawner;
    }

    private void AnsweredIncorrectly()
    {
        string mode = GameState.GetQuestionType();
        int score = GameState.GetScore();

        AnalyticsManager.LogEvent("GameEnded", new Dictionary<string, string> {
            { "score", score.ToString() },
            { "mode", mode },
            { "duration", GameState.GetGameDuration().ToString("F1") }
        });

        int xpEarned = CalculateXP();
        XPSystem.AddXP(xpEarned);
        AchievementData.CheckAchievements();

        PrefsFlush.Flush();

        GameState.ShowGameOverUI();
        PlayFallAnimation();
    }

    private int CalculateXP()
    {
        int baseXP = GameState.GetScore() / 10;
        float accuracy = GameState.GetAccuracyThisGame();
        if (accuracy > 90f) baseXP = (int)(baseXP * 1.5f);
        else if (accuracy > 75f) baseXP = (int)(baseXP * 1.25f);
        return Mathf.Max(1, baseXP);
    }

    private void PlayFallAnimation()
    {
        GameObject player = GameObject.Find("PlayerObject");
        if (player != null)
        {
            var animator = player.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("stumbleBackwards");
                animator.SetBool("isRunning", false);
            }
        }
    }
}
