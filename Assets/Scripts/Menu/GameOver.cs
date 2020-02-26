using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject scoreObject;

    // Start is called before the first frame update
    void Start()
    {
        int score = PlayerPrefs.GetInt("score");
        scoreObject.GetComponent<TextMeshProUGUI>().text = "You scored: " + score;
    }

    // Update is called once per frame
    void Update()
    {
        
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
