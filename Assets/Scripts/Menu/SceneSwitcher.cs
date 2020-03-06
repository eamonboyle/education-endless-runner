using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void GoToGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void GoToModeSelect()
    {
        SceneManager.LoadScene("ModeChoice");
    }

    public void GoToCharacterSelection()
    {
        SceneManager.LoadScene("CharacterSelect");
    }

    public void ChooseCharacterBoy()
    {
        PlayerPrefs.SetString("character", "boy");
        GoToMainMenu();
    }

    public void ChooseCharacterGirl()
    {
        PlayerPrefs.SetString("character", "girl");
        GoToMainMenu();
    }
}
