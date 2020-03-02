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
        int score = PlayerPrefs.GetInt("score");
        scoreObject.GetComponent<Text>().text = "Score: " + score;
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
