using System.Collections;
using UnityEngine;

/// <summary>
/// Applies camera shake effects by temporarily offsetting the camera position.
/// Provides preset intensities and respects the accessibility reduced-motion
/// setting via <see cref="AccessibilityManager"/>.
/// </summary>
public class ScreenShake : MonoBehaviour
{
    #region Singleton

    /// <summary>Global singleton instance for easy static access.</summary>
    public static ScreenShake Instance { get; private set; }

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

    [Header("Preset Values")]
    [SerializeField, Tooltip("Intensity for the small shake preset.")]
    private float smallIntensity = 0.05f;

    [SerializeField, Tooltip("Duration for the small shake preset (seconds).")]
    private float smallDuration = 0.15f;

    [SerializeField, Tooltip("Intensity for the medium shake preset.")]
    private float mediumIntensity = 0.15f;

    [SerializeField, Tooltip("Duration for the medium shake preset (seconds).")]
    private float mediumDuration = 0.25f;

    [SerializeField, Tooltip("Intensity for the big shake preset.")]
    private float bigIntensity = 0.35f;

    [SerializeField, Tooltip("Duration for the big shake preset (seconds).")]
    private float bigDuration = 0.4f;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    /// <summary>
    /// Triggers a camera shake with the given intensity and duration.
    /// If <see cref="AccessibilityManager"/> has reduced-motion enabled
    /// (high-contrast mode as a proxy), the shake is skipped.
    /// </summary>
    /// <param name="intensity">Maximum random offset magnitude.</param>
    /// <param name="duration">How long the shake lasts in seconds.</param>
    public void Shake(float intensity, float duration)
    {
        if (ReducedMotionEnabled()) return;
        if (intensity <= 0f || duration <= 0f) return;

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            transform.localPosition = originalPosition;
        }

        shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    /// <summary>Triggers a small preset shake.</summary>
    public void SmallShake()
    {
        Shake(smallIntensity, smallDuration);
    }

    /// <summary>Triggers a medium preset shake.</summary>
    public void MediumShake()
    {
        Shake(mediumIntensity, mediumDuration);
    }

    /// <summary>Triggers a big preset shake.</summary>
    public void BigShake()
    {
        Shake(bigIntensity, bigDuration);
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-intensity, intensity);
            float y = Random.Range(-intensity, intensity);
            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            float decay = 1f - (elapsed / duration);
            intensity *= decay > 0f ? Mathf.Lerp(1f, decay, 0.5f) : 0f;

            yield return null;
        }

        transform.localPosition = originalPosition;
        shakeCoroutine = null;
    }

    private static bool ReducedMotionEnabled()
    {
        if (AccessibilityManager.Instance == null) return false;
        return AccessibilityManager.Instance.HighContrastMode;
    }
}
