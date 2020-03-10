using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelect : MonoBehaviour
{
    public GameObject homeButton;

    private void Start()
    {
        if (GameState.IsFirstLoad())
        {
            homeButton.SetActive(false);
        }
        else
        {
            homeButton.SetActive(true);
        }
    }

    public void Choose(string mode)
    {
        GameState.SetQuestionType(mode);

        if (GameState.IsFirstLoad())
        {
            SceneManager.LoadScene("Tutorial");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
