using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsDisplay : MonoBehaviour
{
    [Header("Stat Text References")]
    [SerializeField] private TextMeshProUGUI totalGamesPlayedText;
    [SerializeField] private TextMeshProUGUI totalCorrectAnswersText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI timePlayedText;

    [Header("Best Streak Per Mode")]
    [SerializeField] private TextMeshProUGUI bestStreakAdditionText;
    [SerializeField] private TextMeshProUGUI bestStreakSubtractionText;
    [SerializeField] private TextMeshProUGUI bestStreakMultiplicationText;
    [SerializeField] private TextMeshProUGUI bestStreakDivisionText;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        int gamesPlayed = PlayerPrefs.GetInt("totalGamesPlayed", 0);
        int correctAnswers = PlayerPrefs.GetInt("totalCorrectAnswers", 0);
        int totalAnswers = PlayerPrefs.GetInt("totalAnswers", 0);
        float timePlayed = PlayerPrefs.GetFloat("totalTimePlayed", 0f);

        if (totalGamesPlayedText != null)
        {
            totalGamesPlayedText.text = gamesPlayed.ToString();
        }

        if (totalCorrectAnswersText != null)
        {
            totalCorrectAnswersText.text = correctAnswers.ToString();
        }

        if (accuracyText != null)
        {
            float accuracy = (totalAnswers > 0) ? ((float)correctAnswers / totalAnswers) * 100f : 0f;
            accuracyText.text = accuracy.ToString("F1") + "%";
        }

        if (timePlayedText != null)
        {
            timePlayedText.text = FormatTime(timePlayed);
        }

        RefreshBestStreak(bestStreakAdditionText, "bestStreak_Addition");
        RefreshBestStreak(bestStreakSubtractionText, "bestStreak_Subtraction");
        RefreshBestStreak(bestStreakMultiplicationText, "bestStreak_Multiplication");
        RefreshBestStreak(bestStreakDivisionText, "bestStreak_Division");
    }

    private void RefreshBestStreak(TextMeshProUGUI textComponent, string prefsKey)
    {
        if (textComponent != null)
        {
            int bestStreak = PlayerPrefs.GetInt(prefsKey, 0);
            textComponent.text = bestStreak.ToString();
        }
    }

    private string FormatTime(float totalSeconds)
    {
        int hours = Mathf.FloorToInt(totalSeconds / 3600f);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);

        if (hours > 0)
        {
            return string.Format("{0}h {1}m {2}s", hours, minutes, seconds);
        }
        else if (minutes > 0)
        {
            return string.Format("{0}m {1}s", minutes, seconds);
        }
        else
        {
            return string.Format("{0}s", seconds);
        }
    }
}
