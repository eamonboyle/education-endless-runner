using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject scoreObject;

    // Start is called before the first frame update
    void Start()
    {
        int score = GameState.GetScore();
        scoreObject.GetComponent<Text>().text = "Score: " + score;

        GameObject adManager = GameObject.Find("AdManager");
        adManager.GetComponent<AdManager>().enabled = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
