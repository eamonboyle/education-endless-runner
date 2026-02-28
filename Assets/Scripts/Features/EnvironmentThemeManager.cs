using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Available environment theme types. Each corresponds to a visual style
/// applied to floors, skybox, fog, and ambient lighting.
/// </summary>
public enum ThemeType
{
    /// <summary>Default urban environment.</summary>
    City = 0,
    /// <summary>Forest theme — unlocks at 1 000 total score.</summary>
    Forest = 1,
    /// <summary>Space theme — unlocks at 5 000 total score.</summary>
    Space = 2,
    /// <summary>Underwater theme — unlocks at 10 000 total score.</summary>
    Underwater = 3,
    /// <summary>Desert theme — unlocks at 20 000 total score.</summary>
    Desert = 4
}

/// <summary>
/// Holds all visual data for a single environment theme.
/// </summary>
[Serializable]
public class EnvironmentTheme
{
    /// <summary>Which theme this data represents.</summary>
    public ThemeType Type;

    /// <summary>Material applied to floor tiles.</summary>
    public Material FloorMaterial;

    /// <summary>Material applied to the skybox.</summary>
    public Material SkyboxMaterial;

    /// <summary>Building/obstacle prefabs used by the level generator.</summary>
    public GameObject[] BuildingPrefabs;

    /// <summary>Fog colour for this theme.</summary>
    public Color FogColor = Color.gray;

    /// <summary>Ambient light colour for this theme.</summary>
    public Color AmbientColor = Color.white;
}

/// <summary>
/// Singleton MonoBehaviour that manages environment themes. Themes unlock at
/// cumulative score milestones and are applied to the scene's lighting,
/// skybox, and fog settings. The selected theme persists via PlayerPrefs.
/// </summary>
public class EnvironmentThemeManager : MonoBehaviour
{
    #region Singleton

    /// <summary>Global singleton instance.</summary>
    public static EnvironmentThemeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #endregion

    private const string ThemePrefKey = "selectedTheme";

    [SerializeField, Tooltip("Theme definitions ordered by ThemeType enum value.")]
    private EnvironmentTheme[] themes = new EnvironmentTheme[0];

    private ThemeType currentTheme = ThemeType.City;

    /// <summary>Fired when the active theme changes.</summary>
    public event Action<ThemeType> OnThemeChanged;

    private void Start()
    {
        int saved = PlayerPrefs.GetInt(ThemePrefKey, (int)ThemeType.City);
        currentTheme = (ThemeType)saved;
        ApplyTheme(currentTheme);
    }

    /// <summary>
    /// Changes the active environment theme and persists the selection.
    /// Only applies if the theme is unlocked.
    /// </summary>
    /// <param name="type">Theme to activate.</param>
    public void SetTheme(ThemeType type)
    {
        if (!IsThemeUnlocked(type)) return;

        currentTheme = type;
        PlayerPrefs.SetInt(ThemePrefKey, (int)type);
        PlayerPrefs.Save();
        ApplyTheme(type);
        OnThemeChanged?.Invoke(type);
    }

    /// <summary>Returns the currently active theme type.</summary>
    /// <returns>Active <see cref="ThemeType"/>.</returns>
    public ThemeType GetCurrentTheme()
    {
        return currentTheme;
    }

    /// <summary>
    /// Returns all themes the player has unlocked based on cumulative
    /// high-score totals.
    /// </summary>
    /// <returns>List of unlocked theme types.</returns>
    public List<ThemeType> GetUnlockedThemes()
    {
        List<ThemeType> unlocked = new List<ThemeType>();
        foreach (ThemeType t in Enum.GetValues(typeof(ThemeType)))
        {
            if (IsThemeUnlocked(t))
            {
                unlocked.Add(t);
            }
        }
        return unlocked;
    }

    /// <summary>
    /// Returns the cumulative score milestone required to unlock
    /// the given theme.
    /// </summary>
    /// <param name="type">Theme type.</param>
    /// <returns>Score threshold (0 for the default City theme).</returns>
    public static int GetUnlockThreshold(ThemeType type)
    {
        switch (type)
        {
            case ThemeType.City:       return 0;
            case ThemeType.Forest:     return 1000;
            case ThemeType.Space:      return 5000;
            case ThemeType.Underwater: return 10000;
            case ThemeType.Desert:     return 20000;
            default:                   return int.MaxValue;
        }
    }

    private bool IsThemeUnlocked(ThemeType type)
    {
        return GetCumulativeScore() >= GetUnlockThreshold(type);
    }

    private static int GetCumulativeScore()
    {
        int total = 0;
        string[] modes = { "addition", "subtraction", "multiply", "division" };
        foreach (string mode in modes)
        {
            total += GameState.GetHighScore(mode);
        }
        return total;
    }

    private void ApplyTheme(ThemeType type)
    {
        EnvironmentTheme theme = GetThemeData(type);
        if (theme == null) return;

        if (theme.SkyboxMaterial != null)
        {
            RenderSettings.skybox = theme.SkyboxMaterial;
        }

        RenderSettings.fogColor = theme.FogColor;
        RenderSettings.fog = true;
        RenderSettings.ambientLight = theme.AmbientColor;
    }

    private EnvironmentTheme GetThemeData(ThemeType type)
    {
        if (themes == null) return null;

        foreach (EnvironmentTheme t in themes)
        {
            if (t != null && t.Type == type)
            {
                return t;
            }
        }

        int index = (int)type;
        if (index >= 0 && index < themes.Length)
        {
            return themes[index];
        }

        return null;
    }
}
