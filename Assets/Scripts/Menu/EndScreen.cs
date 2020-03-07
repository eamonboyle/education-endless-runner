using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

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

            default:
                break;
        }
    }
}
