using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Addition()
    {
        Debug.Log("CHOSEN ADDITION");
        PlayerPrefs.SetString("mode", "addition");
        GoToGame();
    }

    public void Subtraction()
    {
        Debug.Log("Chosen Subtraction");
        PlayerPrefs.SetString("mode", "subtraction");
        GoToGame();
    }

    public void Multiply()
    {
        Debug.Log("Chosen Multiply");
        PlayerPrefs.SetString("mode", "multiply");
        GoToGame();
    }

    public void Division()
    {
        Debug.Log("Chosen Division");
        PlayerPrefs.SetString("mode", "division");
        GoToGame();
    }

    void GoToGame()
    {
        SceneManager.LoadScene("Game");
    }
}
