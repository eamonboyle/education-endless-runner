using MathRunner.UI.Toolkit;
using UnityEngine;

/// <summary>
/// Legacy Inspector button targets. New Toolkit screens use
/// <see cref="NavigationService"/> directly; this remains for scene-wired buttons
/// until those canvases are removed.
/// </summary>
public class SceneSwitcher : MonoBehaviour
{
    public void GoToGame() => NavigationService.GoToGame();

    public void GoToMainMenu() => NavigationService.GoToMainMenu();

    public void GoToModeSelect() => NavigationService.GoToModeSelect();

    public void GoToCharacterSelection() => NavigationService.GoToCharacterSelect();

    public void GoToTutorial() => NavigationService.GoToTutorial();

    public void GoToSettings() => NavigationService.GoToSettings();

    public void ShowPauseMenu()
    {
        if (GameState.IsRunning())
            GameState.ShowPauseUI();
    }

    public void ChooseCharacter(string character)
    {
        GameState.SetCharacter(character);

        if (GameState.IsFirstLoad())
            GoToModeSelect();
        else
            GoToMainMenu();
    }
}
