using System;
using UnityEngine;

/// <summary>
/// Singleton that manages accessibility settings: colourblind modes,
/// text scaling, high-contrast mode, and distinct audio cues.
/// All settings are persisted in PlayerPrefs.
/// </summary>
public class AccessibilityManager : MonoBehaviour
{
    #region Singleton
    /// <summary>Global singleton instance.</summary>
    public static AccessibilityManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadSettings();
    }
    #endregion

    /// <summary>Supported colourblind simulation modes.</summary>
    public enum ColorblindMode
    {
        /// <summary>No adjustment.</summary>
        Normal,
        /// <summary>Red-green (common).</summary>
        Deuteranopia,
        /// <summary>Red-green (rare).</summary>
        Protanopia,
        /// <summary>Blue-yellow.</summary>
        Tritanopia
    }

    #region PlayerPrefs Keys
    private const string ColorblindKey = "Accessibility_ColorblindMode";
    private const string TextScaleKey = "Accessibility_TextScale";
    private const string AudioCuesKey = "Accessibility_AudioCues";
    private const string HighContrastKey = "Accessibility_HighContrast";
    #endregion

    private ColorblindMode currentMode = ColorblindMode.Normal;
    private float textScaleMultiplier = 1.0f;
    private bool audioCuesEnabled;
    private bool highContrastMode;

    /// <summary>Current colourblind mode.</summary>
    public ColorblindMode CurrentColorblindMode => currentMode;

    /// <summary>Whether high-contrast mode is enabled.</summary>
    public bool HighContrastMode => highContrastMode;

    /// <summary>Whether distinct audio cues per lane are enabled.</summary>
    public bool AudioCuesEnabled => audioCuesEnabled;

    /// <summary>Sets and persists the colourblind simulation mode.</summary>
    public void SetColorblindMode(ColorblindMode mode)
    {
        currentMode = mode;
        PlayerPrefs.SetInt(ColorblindKey, (int)mode);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Returns an adjusted colour based on the active colourblind mode.
    /// Uses simplified channel remapping; production would use a full
    /// Daltonisation matrix or a post-process shader.
    /// </summary>
    public Color GetAdjustedColor(Color original)
    {
        switch (currentMode)
        {
            case ColorblindMode.Deuteranopia:
                return new Color(
                    0.625f * original.r + 0.375f * original.g,
                    0.7f * original.g + 0.3f * original.r,
                    original.b,
                    original.a);

            case ColorblindMode.Protanopia:
                return new Color(
                    0.567f * original.r + 0.433f * original.g,
                    0.558f * original.g + 0.442f * original.r,
                    original.b,
                    original.a);

            case ColorblindMode.Tritanopia:
                return new Color(
                    original.r,
                    0.95f * original.g + 0.05f * original.b,
                    0.433f * original.g + 0.567f * original.b,
                    original.a);

            case ColorblindMode.Normal:
            default:
                return original;
        }
    }

    /// <summary>Sets and persists the text scale multiplier (clamped 1.0 – 2.0).</summary>
    public void SetTextScale(float scale)
    {
        textScaleMultiplier = Mathf.Clamp(scale, 1.0f, 2.0f);
        PlayerPrefs.SetFloat(TextScaleKey, textScaleMultiplier);
        PlayerPrefs.Save();
        SyncToolkitClasses();
    }

    /// <summary>Returns the current text scale multiplier.</summary>
    public float GetTextScale()
    {
        return textScaleMultiplier;
    }

    /// <summary>Enables or disables distinct audio cues for each answer lane.</summary>
    public void SetAudioCuesEnabled(bool enabled)
    {
        audioCuesEnabled = enabled;
        PlayerPrefs.SetInt(AudioCuesKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>Enables or disables high-contrast UI mode.</summary>
    public void SetHighContrastMode(bool enabled)
    {
        highContrastMode = enabled;
        PlayerPrefs.SetInt(HighContrastKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
        SyncToolkitClasses();
    }

    private void LoadSettings()
    {
        currentMode = (ColorblindMode)PlayerPrefs.GetInt(ColorblindKey, (int)ColorblindMode.Normal);
        textScaleMultiplier = PlayerPrefs.GetFloat(TextScaleKey, 1.0f);
        audioCuesEnabled = PlayerPrefs.GetInt(AudioCuesKey, 0) == 1;
        highContrastMode = PlayerPrefs.GetInt(HighContrastKey, 0) == 1;
    }

    private void SyncToolkitClasses()
    {
        bool rm = ReducedMotionManager.Instance != null && ReducedMotionManager.Instance.IsReducedMotion();
        bool dyslexia = PlayerPrefs.GetInt("Accessibility_DyslexiaFont", 0) == 1;
        MathRunner.UI.Toolkit.UIRoot.Instance?.ApplyAccessibilityClasses(
            highContrastMode, rm, dyslexia, textScaleMultiplier);
    }
}
