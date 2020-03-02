using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.Advertisements;

public class GameOver : MonoBehaviour
{
    public GameObject scoreObject;

    //private string gameId = "3492332";
    //private bool testMode = true;

    // Start is called before the first frame update
    void Start()
    {
        int score = PlayerPrefs.GetInt("score");
        scoreObject.GetComponent<Text>().text = "Score: " + score;

        //Advertisement.Initialize(gameId, testMode);

        //Advertisement.Show();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(1);
    }
}
