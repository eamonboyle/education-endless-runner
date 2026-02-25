using UnityEngine;
using UnityEngine.UI;
using MathRunner.Data;

public class GameOver : MonoBehaviour
{
    public GameObject scoreObject;

    private void Start()
    {
        int score = GameState.GetScore();

        if (scoreObject != null)
        {
            var text = scoreObject.GetComponent<Text>();
            if (text != null)
                text.text = "Score: " + score;
        }

        AchievementData.CheckAchievements();
    }

    public void RestartGame()
    {
        if (GameManager.instance != null)
            GameManager.instance.LoadGame();
    }

    public void QuitGame()
    {
        if (GameManager.instance != null)
            GameManager.instance.LoadMainMenu();
    }
}
