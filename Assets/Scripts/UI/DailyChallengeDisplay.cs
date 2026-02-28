using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailyChallengeDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI challengeDescriptionText;
    [SerializeField] private Image progressBarFill;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private GameObject completedBadge;
    [SerializeField] private TextMeshProUGUI completedText;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        string todayKey = System.DateTime.Now.ToString("yyyyMMdd");

        string description = PlayerPrefs.GetString("dailyChallenge_desc_" + todayKey, "No challenge today");
        int current = PlayerPrefs.GetInt("dailyChallenge_current_" + todayKey, 0);
        int target = PlayerPrefs.GetInt("dailyChallenge_target_" + todayKey, 1);
        bool isCompleted = current >= target;

        if (challengeDescriptionText != null)
        {
            challengeDescriptionText.text = description;
        }

        if (progressBarFill != null)
        {
            float fill = (target > 0) ? Mathf.Clamp01((float)current / target) : 0f;
            progressBarFill.fillAmount = fill;
        }

        if (progressText != null)
        {
            progressText.text = current + " / " + target;
        }

        if (completedBadge != null)
        {
            completedBadge.SetActive(isCompleted);
        }

        if (completedText != null)
        {
            completedText.text = isCompleted ? "Completed!" : "In Progress";
            completedText.color = isCompleted ? Color.green : Color.yellow;
        }
    }
}
