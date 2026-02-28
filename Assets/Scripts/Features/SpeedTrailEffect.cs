using UnityEngine;

/// <summary>
/// Enables, disables, and intensifies a <see cref="TrailRenderer"/> attached
/// to the player based on the current character speed reported by
/// <see cref="GameState"/>. Trail width, time (length), and colour opacity
/// scale proportionally once the speed threshold is exceeded.
/// </summary>
public class SpeedTrailEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Trail renderer to control. Auto-resolved from this GameObject if null.")]
    private TrailRenderer trailRenderer;

    [Header("Speed Thresholds")]
    [SerializeField, Tooltip("Minimum speed at which the trail becomes visible.")]
    private float minSpeedThreshold = 45f;

    [SerializeField, Tooltip("Speed at which the trail reaches maximum intensity.")]
    private float maxSpeedThreshold = 80f;

    [Header("Trail Parameters")]
    [SerializeField, Tooltip("Trail width at minimum intensity.")]
    private float minWidth = 0.1f;

    [SerializeField, Tooltip("Trail width at maximum intensity.")]
    private float maxWidth = 0.5f;

    [SerializeField, Tooltip("Trail time (length) at minimum intensity.")]
    private float minTime = 0.1f;

    [SerializeField, Tooltip("Trail time (length) at maximum intensity.")]
    private float maxTime = 0.6f;

    [SerializeField, Tooltip("Trail alpha at minimum intensity.")]
    private float minAlpha = 0.2f;

    [SerializeField, Tooltip("Trail alpha at maximum intensity.")]
    private float maxAlpha = 1.0f;

    private Color baseStartColor = Color.white;
    private Color baseEndColor = Color.white;
    private bool initialised;

    private void Start()
    {
        if (trailRenderer == null)
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }

        if (trailRenderer != null)
        {
            baseStartColor = trailRenderer.startColor;
            baseEndColor = trailRenderer.endColor;
            trailRenderer.emitting = false;
            initialised = true;
        }
    }

    private void Update()
    {
        if (!initialised || trailRenderer == null) return;

        float speed = GameState.GetCharacterSpeed();

        if (speed < minSpeedThreshold)
        {
            if (trailRenderer.emitting)
            {
                trailRenderer.emitting = false;
            }
            return;
        }

        if (!trailRenderer.emitting)
        {
            trailRenderer.emitting = true;
        }

        float t = Mathf.InverseLerp(minSpeedThreshold, maxSpeedThreshold, speed);

        trailRenderer.startWidth = Mathf.Lerp(minWidth, maxWidth, t);
        trailRenderer.endWidth = trailRenderer.startWidth * 0.3f;
        trailRenderer.time = Mathf.Lerp(minTime, maxTime, t);

        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
        Color start = baseStartColor;
        start.a = alpha;
        Color end = baseEndColor;
        end.a = alpha * 0.5f;

        trailRenderer.startColor = start;
        trailRenderer.endColor = end;
    }
}
