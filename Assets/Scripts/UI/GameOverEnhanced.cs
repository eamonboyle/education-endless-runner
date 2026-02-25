using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverEnhanced : MonoBehaviour
{
    [Header("Score Display")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject newHighScoreBanner;

    [Header("Score Breakdown")]
    [SerializeField] private TextMeshProUGUI timeScoreText;
    [SerializeField] private TextMeshProUGUI comboScoreText;

    [Header("Game Stats")]
    [SerializeField] private TextMeshProUGUI streakInfoText;
    [SerializeField] private TextMeshProUGUI accuracyText;

    [Header("Buttons")]
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button shareButton;

    [Header("Animation Settings")]
    [SerializeField] private float pulseDuration = 0.5f;
    [SerializeField] private float pulseScale = 1.3f;

    private Coroutine pulseCoroutine;

    private void OnEnable()
    {
        SetupButtons();
        DisplayResults();
    }

    private void SetupButtons()
    {
        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.RemoveAllListeners();
            tryAgainButton.onClick.AddListener(OnTryAgain);
        }

        if (backToMenuButton != null)
        {
            backToMenuButton.onClick.RemoveAllListeners();
            backToMenuButton.onClick.AddListener(OnBackToMenu);
        }

        if (shareButton != null)
        {
            shareButton.onClick.RemoveAllListeners();
            shareButton.onClick.AddListener(OnShare);
        }
    }

    private void DisplayResults()
    {
        int score = GameState.GetScore();
        int highScore = GameState.GetHighScore();
        bool isNewHighScore = score >= highScore && score > 0;

        if (finalScoreText != null)
        {
            finalScoreText.text = score.ToString();
        }

        if (highScoreText != null)
        {
            highScoreText.text = highScore.ToString();
        }

        if (newHighScoreBanner != null)
        {
            newHighScoreBanner.SetActive(isNewHighScore);
        }

        if (isNewHighScore && finalScoreText != null)
        {
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
            }
            pulseCoroutine = StartCoroutine(ScalePulseCoroutine(finalScoreText.transform));
        }

        int timeScore = PlayerPrefs.GetInt("lastTimeScore", 0);
        int comboBonus = PlayerPrefs.GetInt("lastComboBonus", 0);

        if (timeScoreText != null)
        {
            timeScoreText.text = timeScore.ToString();
        }

        if (comboScoreText != null)
        {
            comboScoreText.text = "+" + comboBonus.ToString();
        }

        int bestStreak = PlayerPrefs.GetInt("lastBestStreak", 0);
        if (streakInfoText != null)
        {
            streakInfoText.text = bestStreak.ToString();
        }

        int correct = PlayerPrefs.GetInt("lastCorrectAnswers", 0);
        int total = PlayerPrefs.GetInt("lastTotalAnswers", 0);
        if (accuracyText != null)
        {
            float accuracy = (total > 0) ? ((float)correct / total) * 100f : 0f;
            accuracyText.text = accuracy.ToString("F0") + "%";
        }
    }

    private IEnumerator ScalePulseCoroutine(Transform target)
    {
        if (target == null) yield break;

        Vector3 originalScale = Vector3.one;

        while (gameObject.activeInHierarchy)
        {
            float elapsed = 0f;
            float halfDuration = pulseDuration * 0.5f;

            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                target.localScale = Vector3.Lerp(originalScale, originalScale * pulseScale, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                target.localScale = Vector3.Lerp(originalScale * pulseScale, originalScale, t);
                yield return null;
            }

            target.localScale = originalScale;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnTryAgain()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.LoadGame();
        }
    }

    private void OnBackToMenu()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.LoadMainMenu();
        }
    }

    private void OnShare()
    {
        if (GameManager.instance != null)
        {
            string shareText = "I scored " + GameState.GetScore() + " in Math Runner!";
            GameManager.instance.ShareScreenshotWithText(shareText);
        }
    }

    private void OnDisable()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        if (finalScoreText != null)
        {
            finalScoreText.transform.localScale = Vector3.one;
        }
    }
}
