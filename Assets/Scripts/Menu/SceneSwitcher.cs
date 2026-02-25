using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    public void GoToGame()
    {
        if (GameManager.instance != null)
            GameManager.instance.LoadGame();
        else
            Debug.LogError("SceneSwitcher: GameManager.instance is null. Is the Persistent Scene loaded?");
    }

    public void GoToMainMenu()
    {
        if (GameManager.instance != null)
            GameManager.instance.LoadMainMenu();
        else
            Debug.LogError("SceneSwitcher: GameManager.instance is null.");
    }

    public void GoToModeSelect()
    {
        if (GameManager.instance != null)
            GameManager.instance.LoadModeSelect();
        else
            Debug.LogError("SceneSwitcher: GameManager.instance is null.");
    }

    public void GoToCharacterSelection()
    {
        if (GameManager.instance != null)
            GameManager.instance.LoadCharacterSelection();
        else
            Debug.LogError("SceneSwitcher: GameManager.instance is null.");
    }

    public void GoToTutorial()
    {
        if (GameManager.instance != null)
            GameManager.instance.LoadTutorial();
        else
            Debug.LogError("SceneSwitcher: GameManager.instance is null.");
    }

    public void GoToSettings()
    {
        if (GameManager.instance != null)
            GameManager.instance.LoadSettings();
        else
            Debug.LogError("SceneSwitcher: GameManager.instance is null.");
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
