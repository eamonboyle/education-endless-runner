using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public GameObject continueButton;
    public GameObject countdownText;
    public GameObject questionText;
    public GameObject mainCamera;

    private Button myButton;

    private void Start()
    {
        if (continueButton != null)
            myButton = continueButton.GetComponent<Button>();
    }

    public void GameOverUIButtonClick(string action)
    {
        switch (action)
        {
            case "restart":
                GameSession.BeginRun();
                break;

            case "continue":
                // Ads are disabled — continue immediately.
                ContinueGame();
                break;

            case "share":
                if (GameManager.instance != null)
                    GameManager.instance.Screenshot();
                break;

            case "quit":
                if (GameManager.instance != null)
                    GameManager.instance.LoadMainMenu();
                break;

            default:
                break;
        }
    }

    private void ContinueGame()
    {
        GameState.SetGameOver(false);

        if (LivesSystem.Instance != null)
            LivesSystem.Instance.ResetLives();

        var questionGeneration = FindAnyObjectByType<QuestionGeneration>();
        if (questionGeneration != null)
            questionGeneration.ResyncAfterContinue();

        GameState.ShowGameUI();
        StartCoroutine(Countdown(4));
    }

    private IEnumerator Countdown(int seconds)
    {
        int count = seconds;
        if (countdownText != null)
            countdownText.SetActive(true);

        while (count > 0)
        {
            if (countdownText != null)
            {
                var text = countdownText.GetComponent<Text>();
                if (text != null)
                    text.text = count == 1 ? "GO!" : (count - 1).ToString();
            }

            if (mainCamera != null)
            {
                var audio = mainCamera.GetComponent<AudioSource>();
                if (audio != null) audio.Play();
            }

            yield return new WaitForSeconds(1);
            count--;
        }

        StartGame();
    }

    private void StartGame()
    {
        GameState.QuestionBoxShow(true);
        if (countdownText != null) countdownText.SetActive(false);
        if (questionText != null) questionText.SetActive(true);

        var canvas = gameObject.GetComponent<Canvas>();
        if (canvas != null) canvas.enabled = false;

        var player = GameObject.Find("PlayerObject");
        if (player != null)
        {
            var animator = player.GetComponent<Animator>();
            if (animator != null)
                animator.SetBool("isRunning", true);
        }

        GameState.SetRunning(true);
    }
}
