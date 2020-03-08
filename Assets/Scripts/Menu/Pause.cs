﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public GameObject countdownText;
    public GameObject questionText;
    public GameObject mainCamera;

    public void GameOverUIButtonClick(string action)
    {
        switch (action)
        {
            case "continue":
                ContinueGame();
                break;
            case "quit":
                GameState.Init();
                SceneManager.LoadScene("MainMenu");
                break;

            default:
                break;
        }
    }

    private void ContinueGame()
    {
        GameState.ShowGameUI();
        StartCoroutine(Countdown(4));
    }

    private IEnumerator Countdown(int seconds)
    {
        int count = seconds;
        countdownText.SetActive(true);

        while (count > 0)
        {
            if (count == 1)
            {
                countdownText.GetComponent<Text>().text = "GO!";
            }
            else
            {
                countdownText.GetComponent<Text>().text = (count - 1).ToString();
            }

            // TODO: Change this sound and try put an animation on the text
            mainCamera.GetComponent<AudioSource>().Play();

            yield return new WaitForSeconds(1);

            count--;
        }

        StartGame();
    }

    private void StartGame()
    {
        countdownText.SetActive(false);
        questionText.SetActive(true);
        GameState.QuestionBoxShow(true);
        gameObject.GetComponent<Canvas>().enabled = false;
        GameObject.Find("PlayerObject").GetComponent<Animator>().SetBool("isRunning", true);
        GameState.SetRunning(true);
    }
}
