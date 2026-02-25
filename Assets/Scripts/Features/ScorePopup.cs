using TMPro;
using UnityEngine;

/// <summary>
/// Floating score text (e.g. "+20") that drifts upward, fades out,
/// and destroys itself after one second.  Requires a world-space
/// <see cref="TextMeshPro"/> component.
/// </summary>
[RequireComponent(typeof(TextMeshPro))]
public class ScorePopup : MonoBehaviour
{
    [SerializeField, Tooltip("How fast the popup floats upward (units/s).")]
    private float floatSpeed = 2f;

    [SerializeField, Tooltip("Total lifetime in seconds before self-destruct.")]
    private float lifetime = 1f;

    private TextMeshPro textMesh;
    private Color startColor;
    private float elapsed;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            startColor = textMesh.color;
        }
    }

    private void Update()
    {
        elapsed += Time.deltaTime;

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (textMesh != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / lifetime);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
        }

        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Factory method that creates a floating score popup at the given world position.
    /// </summary>
    /// <param name="position">World position to spawn at.</param>
    /// <param name="points">Point value to display (e.g. 20).</param>
    /// <param name="parent">Optional parent transform.</param>
    /// <returns>The created <see cref="ScorePopup"/> instance, or null if no prefab is found.</returns>
    public static ScorePopup Create(Vector3 position, int points, Transform parent = null)
    {
        GameObject go = new GameObject("ScorePopup");
        go.transform.position = position;

        if (parent != null)
        {
            go.transform.SetParent(parent, worldPositionStays: true);
        }

        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = "+" + points;
        tmp.fontSize = 6f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.yellow;

        ScorePopup popup = go.AddComponent<ScorePopup>();
        return popup;
    }
}
