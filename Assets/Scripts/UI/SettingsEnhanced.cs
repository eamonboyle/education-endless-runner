using UnityEngine;
using MathRunner.Core;

/// <summary>
/// Enhanced settings panel rendered entirely via OnGUI so it works without
/// scene-based UI setup. Exposes difficulty presets, input mode, and
/// accessibility toggles (colorblind, dyslexia font, reduced motion).
/// Reads current values on Start and persists every change immediately.
/// </summary>
public class SettingsEnhanced : MonoBehaviour
{
    private int selectedDifficulty;
    private int selectedInputMode;
    private bool colorblindEnabled;
    private bool dyslexiaFontEnabled;
    private bool reducedMotionEnabled;

    private readonly string[] difficultyLabels = { "Easy", "Medium", "Hard" };
    private readonly string[] inputModeLabels = { "Swipe", "Tap", "Buttons" };

    private void Start()
    {
        selectedDifficulty = (int)DifficultyPresets.GetDifficulty();

        if (InputManager.Instance != null)
        {
            selectedInputMode = (int)InputManager.Instance.GetInputMode();
        }

        if (AccessibilityManager.Instance != null)
        {
            colorblindEnabled = AccessibilityManager.Instance.CurrentColorblindMode != AccessibilityManager.ColorblindMode.Normal;
        }

        var dyslexiaMgr = FindObjectOfType<DyslexiaFontManager>();
        if (dyslexiaMgr != null)
        {
            dyslexiaFontEnabled = dyslexiaMgr.IsEnabled();
        }

        if (ReducedMotionManager.Instance != null)
        {
            reducedMotionEnabled = ReducedMotionManager.Instance.IsReducedMotion();
        }
    }

    private void OnGUI()
    {
        float panelWidth = 320f;
        float panelHeight = 340f;
        float x = (Screen.width - panelWidth) * 0.5f;
        float y = (Screen.height - panelHeight) * 0.5f;

        GUI.Box(new Rect(x, y, panelWidth, panelHeight), "Enhanced Settings");

        float lineY = y + 30f;
        float labelX = x + 10f;
        float controlX = x + 150f;

        // Difficulty
        GUI.Label(new Rect(labelX, lineY, 130f, 25f), "Difficulty:");
        int newDifficulty = GUI.SelectionGrid(
            new Rect(controlX, lineY, 160f, 25f),
            selectedDifficulty, difficultyLabels, 3);
        if (newDifficulty != selectedDifficulty)
        {
            selectedDifficulty = newDifficulty;
            DifficultyPresets.SetDifficulty((DifficultyLevel)selectedDifficulty);
        }

        lineY += 35f;

        // Input Mode
        GUI.Label(new Rect(labelX, lineY, 130f, 25f), "Input Mode:");
        int newInput = GUI.SelectionGrid(
            new Rect(controlX, lineY, 160f, 25f),
            selectedInputMode, inputModeLabels, 3);
        if (newInput != selectedInputMode)
        {
            selectedInputMode = newInput;
            if (InputManager.Instance != null)
            {
                InputManager.Instance.SetInputMode((InputManager.InputMode)selectedInputMode);
            }
        }

        lineY += 45f;

        // Accessibility header
        GUI.Label(new Rect(labelX, lineY, 200f, 25f), "--- Accessibility ---");
        lineY += 30f;

        // Colorblind mode
        bool newColorblind = GUI.Toggle(
            new Rect(labelX, lineY, 290f, 25f),
            colorblindEnabled, "  Colorblind Mode");
        if (newColorblind != colorblindEnabled)
        {
            colorblindEnabled = newColorblind;
            if (AccessibilityManager.Instance != null)
            {
                AccessibilityManager.Instance.SetColorblindMode(
                    colorblindEnabled
                        ? AccessibilityManager.ColorblindMode.Deuteranopia
                        : AccessibilityManager.ColorblindMode.Normal);
            }
        }

        lineY += 30f;

        // Dyslexia font
        bool newDyslexia = GUI.Toggle(
            new Rect(labelX, lineY, 290f, 25f),
            dyslexiaFontEnabled, "  Dyslexia-Friendly Font");
        if (newDyslexia != dyslexiaFontEnabled)
        {
            dyslexiaFontEnabled = newDyslexia;
            var dyslexiaMgr = FindObjectOfType<DyslexiaFontManager>();
            if (dyslexiaMgr != null)
            {
                if (dyslexiaFontEnabled)
                    dyslexiaMgr.EnableDyslexiaFont();
                else
                    dyslexiaMgr.DisableDyslexiaFont();
            }
        }

        lineY += 30f;

        // Reduced motion
        bool newReduced = GUI.Toggle(
            new Rect(labelX, lineY, 290f, 25f),
            reducedMotionEnabled, "  Reduced Motion");
        if (newReduced != reducedMotionEnabled)
        {
            reducedMotionEnabled = newReduced;
            if (ReducedMotionManager.Instance != null)
            {
                ReducedMotionManager.Instance.SetReducedMotion(reducedMotionEnabled);
            }
        }
    }
}
