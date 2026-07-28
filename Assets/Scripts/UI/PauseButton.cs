using UnityEngine;

/// <summary>
/// Bridge: pause is triggered via Toolkit HUD pause button / GameState.ShowPauseUI.
/// </summary>
public class PauseButton : MonoBehaviour
{
    private void Update()
    {
        // Keep a keyboard shortcut for editor testing.
        if (GameState.IsRunning() && Input.GetKeyDown(KeyCode.Escape))
            GameState.ShowPauseUI();
    }
}
