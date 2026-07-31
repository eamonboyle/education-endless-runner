using System.Collections;
using MathRunner.UI.Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class StartCountdown : MonoBehaviour
{
    public GameObject questionText;
    public GameObject countdownText;
    public GameObject mainCamera;

    void Start()
    {
        // Toolkit HUD replaces both InGameUI and TutorialUI chrome.
        GameState.ShowGameUI();

        mainCamera = GameObject.FindWithTag("MainCamera");
        GameState.QuestionBoxShow(false);
        StartCoroutine(Countdown(4));
    }

    private IEnumerator Countdown(int seconds)
    {
        int count = seconds;
        var hud = UIRouter.Instance?.Hud;

        while (count > 0)
        {
            string label = count == 1 ? "GO!" : (count - 1).ToString();
            if (hud != null)
            {
                hud.SetCountdown(label, true);
            }
            else if (countdownText != null)
            {
                var text = countdownText.GetComponent<Text>();
                if (text != null) text.text = label;
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
        GameState.SetRunning(true);
        GameState.QuestionBoxShow(true);

        UIRouter.Instance?.Hud?.SetCountdown("", false);
        if (questionText != null) questionText.SetActive(true);
        if (countdownText != null) countdownText.SetActive(false);

        var player = GameObject.Find("PlayerObject");
        if (player != null)
        {
            var animator = player.GetComponent<Animator>();
            if (animator != null)
                animator.SetBool("isRunning", true);
        }
    }
}
