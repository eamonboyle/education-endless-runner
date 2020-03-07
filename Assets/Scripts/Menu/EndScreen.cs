using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public void GameOverUIButtonClick(string action)
    {
        Debug.Log("Game Over UI Button: " + action);

        switch (action)
        {
            case "restart":
                GameState.Init();
                SceneManager.LoadScene("Game");
                break;
            case "continue":
                GameState.ShowGameUI();
                gameObject.GetComponent<Canvas>().enabled = false;
                GameObject.Find("PlayerObject").GetComponent<Animator>().SetBool("isRunning", true);
                GameState.SetRunning(true);
                break;
            case "quit":
                SceneManager.LoadScene("MainMenu");
                break;

            default:
                break;
        }
    }
}
