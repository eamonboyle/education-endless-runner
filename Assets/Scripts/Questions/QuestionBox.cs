using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MathRunner.Data;

public class QuestionBox : MonoBehaviour
{
    public int number;
    public int correctNumber;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

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

        if (!isCorrect)
        {
            var powerUpSystem = PowerUpSystem.Instance;
            if (powerUpSystem != null && powerUpSystem.HasActivePowerUp(PowerUpType.Shield))
            {
                powerUpSystem.DeactivatePowerUp(PowerUpType.Shield);
                AnswerFeedback.PlayIncorrect(transform.position);

                var combo = ComboSystem.Instance;
                if (combo != null) combo.RecordWrongAnswer();

                questionGeneration.DeleteLastQuestion();
                questionGeneration.AddQuestion(true);
                return;
            }

            AnswerFeedback.PlayIncorrect(transform.position);

            var comboSystem = ComboSystem.Instance;
            if (comboSystem != null) comboSystem.RecordWrongAnswer();

            questionGeneration.DeleteLastQuestion();
            AnsweredIncorrectly();
            return;
        }

        AnswerFeedback.PlayCorrect(transform.position);

        var audioSource = Camera.main != null ? Camera.main.GetComponent<AudioSource>() : null;
        if (audioSource != null) audioSource.Play();

        var combo2 = ComboSystem.Instance;
        int bonusPoints = 10;
        if (combo2 != null)
        {
            combo2.RecordCorrectAnswer();
            bonusPoints *= combo2.GetMultiplier();
        }

        var powerUp = PowerUpSystem.Instance;
        if (powerUp != null && powerUp.HasActivePowerUp(PowerUpType.DoublePoints))
        {
            bonusPoints *= 2;
        }

        GameState.AddScore(bonusPoints);
        ScorePopup.Create(transform.position + Vector3.up * 2f, bonusPoints, null);

        MathRunner.Data.DailyChallengeData.RecordProgress(GameState.GetQuestionType());

        questionGeneration.AddQuestion(true);
    }

    private void AnsweredIncorrectly()
    {
        GameState.ShowGameOverUI();
        PlayFallAnimation();
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
