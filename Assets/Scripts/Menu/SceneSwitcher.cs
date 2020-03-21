using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    public void GoToGame()
    {
        GameManager.instance.LoadGame();
    }

    public void GoToMainMenu()
    {
        GameManager.instance.LoadMainMenu();
    }

    public void GoToModeSelect()
    {
        GameManager.instance.LoadModeSelect();
    }

    public void GoToCharacterSelection()
    {
        GameManager.instance.LoadCharacterSelection();
    }

    public void GoToTutorial()
    {
        GameManager.instance.LoadTutorial();
    }

    public void GoToSettings()
    {
        GameManager.instance.LoadSettings();
    }

    public void ShowPauseMenu()
    {
        if (GameState.IsRunning())
        {
            GameState.ShowPauseUI();
        }
    }

    public void ChooseCharacter(string character)
    {
        GameState.SetCharacter(character);

        if (GameState.IsFirstLoad())
        {
            GoToModeSelect();
        }
        else
        {
            GoToMainMenu();
        }
    }
}