using Assets.Scripts.GameManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartCountdown : MonoBehaviour
{
    public GameObject questionText;
    public GameObject countdownText;

    public GameObject mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        GameState.ShowGameUI();

        mainCamera = GameObject.FindWithTag("MainCamera");

        StartCoroutine(Countdown(4));
    }

    private IEnumerator Countdown(int seconds)
    {
        int count = seconds;

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
        GameState.SetRunning(true);
        questionText.SetActive(true);
        countdownText.SetActive(false);
        GameObject.Find("PlayerObject").GetComponent<Animator>().SetBool("isRunning", true);
    }
}
