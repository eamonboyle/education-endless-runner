using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelect : MonoBehaviour
{
    public void Choose(string mode)
    {
        GameState.SetQuestionType(mode);
        GoToGame();
    }

    void GoToGame()
    {
        SceneManager.LoadScene("Game");
    }
}
