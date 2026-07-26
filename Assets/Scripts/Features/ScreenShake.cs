using System.Collections;
using UnityEngine;

/// <summary>
/// Applies camera shake effects by temporarily offsetting the main camera position.
/// Respects the accessibility reduced-motion setting via <see cref="ReducedMotionManager"/>.
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

    private Transform cameraTransform;
    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    private void Start()
    {
        ResolveCamera();
    }

    private void ResolveCamera()
    {
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            originalPosition = cameraTransform.localPosition;
        }
    }

    /// <summary>
    /// Triggers a camera shake with the given intensity and duration.
    /// Skipped when reduced motion is enabled.
    /// </summary>
    public void Shake(float intensity, float duration)
    {
        if (ReducedMotionEnabled()) return;
        if (intensity <= 0f || duration <= 0f) return;

        if (cameraTransform == null)
            ResolveCamera();
        if (cameraTransform == null) return;

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            cameraTransform.localPosition = originalPosition;
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
            if (cameraTransform == null)
            {
                shakeCoroutine = null;
                yield break;
            }

            float x = Random.Range(-intensity, intensity);
            float y = Random.Range(-intensity, intensity);
            cameraTransform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            float decay = 1f - (elapsed / duration);
            intensity *= decay > 0f ? Mathf.Lerp(1f, decay, 0.5f) : 0f;

            yield return null;
        }

        if (cameraTransform != null)
            cameraTransform.localPosition = originalPosition;
        shakeCoroutine = null;
    }

    private static bool ReducedMotionEnabled()
    {
        return ReducedMotionManager.Instance != null
            && ReducedMotionManager.Instance.IsReducedMotion();
    }
}
