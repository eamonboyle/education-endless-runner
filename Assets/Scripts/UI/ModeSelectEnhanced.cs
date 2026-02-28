using UnityEngine;
using MathRunner.Core;

/// <summary>
/// Companion to the existing <see cref="ModeSelect"/> MonoBehaviour.
/// Draws an additional "MIXED" mode button and a difficulty selector via
/// OnGUI so it works without modifying scene assets.
/// </summary>
public class ModeSelectEnhanced : MonoBehaviour
{
    private int selectedDifficulty;

    private readonly string[] difficultyLabels = { "Easy", "Medium", "Hard" };

    private void Start()
    {
        selectedDifficulty = (int)DifficultyPresets.GetDifficulty();
    }

    private void OnGUI()
    {
        float buttonWidth = 200f;
        float buttonHeight = 50f;
        float x = (Screen.width - buttonWidth) * 0.5f;
        float baseY = Screen.height * 0.65f;

        // Mixed mode button
        GUIStyle mixedStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 22,
            fontStyle = FontStyle.Bold
        };

        if (GUI.Button(new Rect(x, baseY, buttonWidth, buttonHeight), "MIXED", mixedStyle))
        {
            GameState.SetQuestionType("mixed");

            if (GameManager.instance != null)
            {
                if (GameState.IsFirstLoad())
                    GameManager.instance.LoadTutorial();
                else
                    GameManager.instance.LoadMainMenu();
            }
        }

        // Difficulty selector below the mixed button
        float diffY = baseY + buttonHeight + 20f;
        float diffWidth = 240f;
        float diffX = (Screen.width - diffWidth) * 0.5f;

        GUI.Label(new Rect(diffX, diffY, diffWidth, 20f), "Difficulty:");
        diffY += 22f;

        int newDifficulty = GUI.SelectionGrid(
            new Rect(diffX, diffY, diffWidth, 30f),
            selectedDifficulty, difficultyLabels, 3);

        if (newDifficulty != selectedDifficulty)
        {
            selectedDifficulty = newDifficulty;
            DifficultyPresets.SetDifficulty((DifficultyLevel)selectedDifficulty);
        }
    }
}
