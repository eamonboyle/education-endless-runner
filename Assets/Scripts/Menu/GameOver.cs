using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public GameObject scoreObject;

    // Start is called before the first frame update
    private void Start()
    {
        int score = GameState.GetScore();
        scoreObject.GetComponent<Text>().text = "Score: " + score;

        GameObject adManager = GameObject.Find("AdManager");
        adManager.GetComponent<AdManager>().enabled = true;
    }

    public void RestartGame()
    {
        GameManager.instance.LoadGame();
    }

    public void QuitGame()
    {
        GameManager.instance.LoadMainMenu();
    }
}