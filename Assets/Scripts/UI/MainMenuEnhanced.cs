using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuEnhanced : MonoBehaviour
{
    [Header("Daily Challenge")]
    [SerializeField] private DailyChallengeDisplay dailyChallengeDisplay;

    [Header("Last Played Info")]
    [SerializeField] private TextMeshProUGUI lastPlayedModeText;
    [SerializeField] private TextMeshProUGUI lastPlayedScoreText;

    [Header("Quick Play")]
    [SerializeField] private Button quickPlayButton;
    [SerializeField] private TextMeshProUGUI quickPlayLabel;

    [Header("Character Preview")]
    [SerializeField] private GameObject characterPreview;

    [Header("Animated Background")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float panSpeed = 0.5f;
    [SerializeField] private float panRange = 2f;

    [Header("Character Idle Animation")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private string idleAnimationParam = "idle";

    private Vector3 cameraStartPosition;

    private void Start()
    {
        if (cameraTransform != null)
        {
            cameraStartPosition = cameraTransform.position;
        }

        if (quickPlayButton != null)
        {
            quickPlayButton.onClick.RemoveAllListeners();
            quickPlayButton.onClick.AddListener(OnQuickPlay);
        }

        if (characterAnimator != null)
        {
            characterAnimator.SetBool(idleAnimationParam, true);
        }

        RefreshLastPlayed();
        RefreshDailyChallenge();
    }

    private void Update()
    {
        AnimateBackground();
    }

    private void AnimateBackground()
    {
        if (cameraTransform == null) return;

        float offset = Mathf.Sin(Time.time * panSpeed) * panRange;
        cameraTransform.position = cameraStartPosition + new Vector3(offset, 0f, 0f);
    }

    private void RefreshLastPlayed()
    {
        string lastMode = PlayerPrefs.GetString("lastPlayedMode", "");
        int lastScore = PlayerPrefs.GetInt("lastPlayedScore", 0);

        if (lastPlayedModeText != null)
        {
            if (!string.IsNullOrEmpty(lastMode))
            {
                lastPlayedModeText.text = "Last: " + lastMode;
            }
            else
            {
                lastPlayedModeText.text = "";
            }
        }

        if (lastPlayedScoreText != null)
        {
            if (!string.IsNullOrEmpty(lastMode))
            {
                lastPlayedScoreText.text = "Score: " + lastScore;
            }
            else
            {
                lastPlayedScoreText.text = "";
            }
        }

        if (quickPlayLabel != null)
        {
            if (!string.IsNullOrEmpty(lastMode))
            {
                quickPlayLabel.text = "Quick Play (" + lastMode + ")";
            }
            else
            {
                quickPlayLabel.text = "Play";
            }
        }
    }

    public void RefreshDailyChallenge()
    {
        if (dailyChallengeDisplay != null)
        {
            dailyChallengeDisplay.Refresh();
        }
    }

    private void OnQuickPlay()
    {
        string lastMode = PlayerPrefs.GetString("lastPlayedMode", "");

        if (!string.IsNullOrEmpty(lastMode))
        {
            GameState.SetQuestionType(lastMode);
        }

        if (GameManager.instance != null)
        {
            GameSession.BeginRun();
        }
    }
}
