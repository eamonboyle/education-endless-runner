using UnityEngine;

/// <summary>
/// Draws a semi-transparent pause button in the top-right corner during
/// active gameplay via OnGUI. Tapping it calls
/// <see cref="GameState.ShowPauseUI"/>.
/// </summary>
public class PauseButton : MonoBehaviour
{
    private void OnGUI()
    {
        if (!GameState.IsRunning()) return;

        float size = 50f;
        float margin = 10f;
        float x = Screen.width - size - margin;
        float y = margin;

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 28,
            alignment = TextAnchor.MiddleCenter
        };

        Color prev = GUI.backgroundColor;
        GUI.backgroundColor = new Color(1f, 1f, 1f, 0.4f);

        if (GUI.Button(new Rect(x, y, size, size), "\u23F8", buttonStyle))
        {
            GameState.ShowPauseUI();
        }

        GUI.backgroundColor = prev;
    }
}
