using MathRunner.Core;
using UnityEngine;

/// <summary>
/// Displays the difficulty the player selected, colour-coded, in the bottom-left
/// corner during gameplay. Uses OnGUI for rendering — no scene setup required.
/// </summary>
public class DifficultyIndicator : MonoBehaviour
{
    private void OnGUI()
    {
        // Desktop and editor only — the label is too noisy on a phone screen.
        if (Application.isMobilePlatform) return;
        if (!GameState.IsRunning()) return;

        DifficultyLevel level = DifficultyPresets.GetDifficulty();

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.Max(22, Mathf.RoundToInt(Screen.height * 0.022f)),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.LowerLeft
        };
        style.normal.textColor = ColourFor(level);

        const float padding = 24f;
        float height = style.fontSize + 12f;
        GUI.Label(
            new Rect(padding, Screen.height - height - padding, 220f, height),
            LabelFor(level), style);
    }

    /// <summary>Matches the wording on the play-style panel, which calls Medium "Normal".</summary>
    private static string LabelFor(DifficultyLevel level)
    {
        switch (level)
        {
            case DifficultyLevel.Easy: return "EASY";
            case DifficultyLevel.Hard: return "HARD";
            case DifficultyLevel.Medium:
            default: return "NORMAL";
        }
    }

    private static Color ColourFor(DifficultyLevel level)
    {
        switch (level)
        {
            case DifficultyLevel.Easy: return Color.green;
            case DifficultyLevel.Hard: return new Color(1f, 0.4f, 0.3f);
            case DifficultyLevel.Medium:
            default: return Color.yellow;
        }
    }
}
