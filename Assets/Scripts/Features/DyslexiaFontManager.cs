using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Toggles a dyslexia-friendly font on all <see cref="Text"/> and
/// <see cref="TextMeshProUGUI"/> components in the active scene.
/// The setting is persisted in PlayerPrefs and reapplied on each scene load.
/// </summary>
public class DyslexiaFontManager : MonoBehaviour
{
    [SerializeField, Tooltip("Dyslexia-friendly replacement font for legacy UI Text components.")]
    private Font dyslexiaFont;

    [SerializeField, Tooltip("Dyslexia-friendly TMP font asset for TextMeshPro components.")]
    private TMP_FontAsset dyslexiaTMPFont;

    private const string PrefsKey = "Accessibility_DyslexiaFont";

    private Font[] originalFonts;
    private TMP_FontAsset[] originalTMPFonts;
    private bool applied;

    private void Start()
    {
        if (IsEnabled())
        {
            EnableDyslexiaFont();
        }
    }

    /// <summary>
    /// Finds all text components in the scene and replaces their fonts
    /// with the assigned dyslexia-friendly fonts.
    /// </summary>
    public void EnableDyslexiaFont()
    {
        if (applied) return;

        Text[] textComponents = FindObjectsByType<Text>(FindObjectsInactive.Include);
        originalFonts = new Font[textComponents.Length];
        for (int i = 0; i < textComponents.Length; i++)
        {
            originalFonts[i] = textComponents[i].font;
            if (dyslexiaFont != null)
            {
                textComponents[i].font = dyslexiaFont;
            }
        }

        TextMeshProUGUI[] tmpComponents = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include);
        originalTMPFonts = new TMP_FontAsset[tmpComponents.Length];
        for (int i = 0; i < tmpComponents.Length; i++)
        {
            originalTMPFonts[i] = tmpComponents[i].font;
            if (dyslexiaTMPFont != null)
            {
                tmpComponents[i].font = dyslexiaTMPFont;
            }
        }

        applied = true;
        PlayerPrefs.SetInt(PrefsKey, 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Restores all text components to their original fonts.
    /// </summary>
    public void DisableDyslexiaFont()
    {
        if (!applied) return;

        Text[] textComponents = FindObjectsByType<Text>(FindObjectsInactive.Include);
        for (int i = 0; i < textComponents.Length && i < originalFonts.Length; i++)
        {
            if (originalFonts[i] != null)
            {
                textComponents[i].font = originalFonts[i];
            }
        }

        TextMeshProUGUI[] tmpComponents = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include);
        for (int i = 0; i < tmpComponents.Length && i < originalTMPFonts.Length; i++)
        {
            if (originalTMPFonts[i] != null)
            {
                tmpComponents[i].font = originalTMPFonts[i];
            }
        }

        applied = false;
        PlayerPrefs.SetInt(PrefsKey, 0);
        PlayerPrefs.Save();
    }

    /// <summary>Returns whether the dyslexia font mode is currently enabled.</summary>
    public bool IsEnabled()
    {
        return PlayerPrefs.GetInt(PrefsKey, 0) == 1;
    }
}
