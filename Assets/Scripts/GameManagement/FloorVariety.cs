using UnityEngine;

/// <summary>
/// Adds visual variety to spawned floor pieces by applying random colour tints
/// and slight rotations to child renderers. Attach to the same GameObject as
/// <see cref="LevelGenerator"/>.
/// </summary>
public class FloorVariety : MonoBehaviour
{
    [SerializeField, Tooltip("Maximum hue shift applied to child renderers (fraction of hue circle).")]
    private float maxHueShift = 0.1f;

    [SerializeField, Tooltip("Maximum random Y-axis rotation (degrees) applied to props.")]
    private float maxRotation = 15f;

    private LevelGenerator levelGenerator;
    private int lastFloorCount;

    private void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();

        if (levelGenerator != null)
        {
            lastFloorCount = levelGenerator.floorPieces.Count;
            foreach (var piece in levelGenerator.floorPieces)
            {
                if (piece != null) ApplyVariety(piece);
            }
        }
    }

    private void Update()
    {
        if (levelGenerator == null) return;

        int currentCount = levelGenerator.floorPieces.Count;
        if (currentCount > lastFloorCount)
        {
            for (int i = lastFloorCount; i < currentCount; i++)
            {
                if (i >= 0 && i < levelGenerator.floorPieces.Count)
                {
                    var piece = levelGenerator.floorPieces[i];
                    if (piece != null) ApplyVariety(piece);
                }
            }
        }
        lastFloorCount = currentCount;
    }

    private void ApplyVariety(GameObject floorPiece)
    {
        Renderer[] renderers = floorPiece.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            if (rend == null) continue;

            foreach (Material mat in rend.materials)
            {
                if (mat == null) continue;

                Color original = mat.color;
                float h, s, v;
                Color.RGBToHSV(original, out h, out s, out v);

                h += Random.Range(-maxHueShift, maxHueShift);
                if (h < 0f) h += 1f;
                if (h > 1f) h -= 1f;

                mat.color = Color.HSVToRGB(h, s, v);
            }
        }

        if (floorPiece.transform.childCount > 0)
        {
            for (int i = 0; i < floorPiece.transform.childCount; i++)
            {
                Transform child = floorPiece.transform.GetChild(i);
                if (Random.value > 0.5f)
                {
                    child.Rotate(0f, Random.Range(-maxRotation, maxRotation), 0f);
                }
            }
        }
    }
}
