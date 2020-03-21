using UnityEngine;

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
            GameManager.instance.LoadTutorial();
        }
        else
        {
            GameManager.instance.LoadMainMenu();
        }
    }
}