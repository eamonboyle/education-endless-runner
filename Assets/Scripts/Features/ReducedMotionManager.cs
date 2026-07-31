using UnityEngine;

/// <summary>
/// Singleton that provides a global reduced-motion flag. When enabled,
/// particle effects, screen shake, UI animations, and trail effects
/// are suppressed. Other systems query <see cref="IsReducedMotion"/>
/// before playing visual effects.
/// </summary>
public class ReducedMotionManager : MonoBehaviour
{
    #region Singleton

    /// <summary>Global singleton instance.</summary>
    public static ReducedMotionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSetting();
    }

    #endregion

    private const string PrefsKey = "Accessibility_ReducedMotion";

    private bool reducedMotion;

    /// <summary>
    /// Enables or disables reduced-motion mode and persists the setting.
    /// When transitioning to enabled, any active particle systems in the
    /// scene are stopped.
    /// </summary>
    /// <param name="enabled"><c>true</c> to enable reduced motion.</param>
    public void SetReducedMotion(bool enabled)
    {
        reducedMotion = enabled;
        PlayerPrefs.SetInt(PrefsKey, enabled ? 1 : 0);
        PlayerPrefs.Save();

        if (enabled)
        {
            DisableActiveParticles();
        }
    }

    /// <summary>
    /// Returns <c>true</c> when reduced-motion mode is active.
    /// Check this before playing particle effects, screen shake,
    /// trail renderers, or animated UI transitions.
    /// </summary>
    public bool IsReducedMotion()
    {
        return reducedMotion;
    }

    private void LoadSetting()
    {
        reducedMotion = PlayerPrefs.GetInt(PrefsKey, 0) == 1;
    }

    private static void DisableActiveParticles()
    {
        ParticleSystem[] particles = FindObjectsByType<ParticleSystem>(FindObjectsInactive.Include);
        foreach (ParticleSystem ps in particles)
        {
            if (ps == null) continue;
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
