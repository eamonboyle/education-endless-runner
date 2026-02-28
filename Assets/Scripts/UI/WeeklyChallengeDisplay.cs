using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MathRunner.Data;

/// <summary>
/// MonoBehaviour that displays the current weekly challenge progress.
/// Shows the challenge description, a fill-based progress bar, numeric
/// progress text, and a completion badge. Call <see cref="Refresh"/>
/// to update the display after progress changes.
/// </summary>
public class WeeklyChallengeDisplay : MonoBehaviour
{
    [SerializeField, Tooltip("Text component for the challenge description.")]
    private TextMeshProUGUI challengeDescriptionText;

    [SerializeField, Tooltip("Image used as a fill-based progress bar.")]
    private Image progressBarFill;

    [SerializeField, Tooltip("Text component showing numeric progress (e.g. 3 / 10).")]
    private TextMeshProUGUI progressText;

    [SerializeField, Tooltip("GameObject activated when the challenge is complete.")]
    private GameObject completedBadge;

    [SerializeField, Tooltip("Text component showing completion status.")]
    private TextMeshProUGUI completedText;

    private void OnEnable()
    {
        Refresh();
    }

    /// <summary>
    /// Reads the current weekly challenge from <see cref="WeeklyChallengeData"/>
    /// and updates all bound UI elements.
    /// </summary>
    public void Refresh()
    {
        WeeklyChallenge challenge = WeeklyChallengeData.GetThisWeekChallenge();
        if (challenge == null) return;

        if (challengeDescriptionText != null)
        {
            challengeDescriptionText.text = challenge.Description;
        }

        int current = challenge.CurrentCount;
        int target = challenge.TargetCount;
        bool isCompleted = challenge.IsComplete;

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
