using UnityEngine;

/// <summary>
/// Builds a power-up pickup GameObject at runtime when no art prefab is assigned.
/// </summary>
public static class PowerUpFactory
{
    public static GameObject CreatePickup(PowerUpType type, Vector3 position, Transform parent)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "PowerUp_" + type;
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.7f;

        Collider col = go.GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        // Remove the default Rigidbody-less physics noise; keep trigger only.
        var renderer = go.GetComponent<MeshRenderer>();
        if (renderer != null && renderer.material != null)
            renderer.material.color = ColorForType(type);

        var collectible = go.AddComponent<PowerUpCollectible>();
        collectible.SetType(type);
        return go;
    }

    public static Color ColorForType(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Shield: return new Color(0.3f, 0.7f, 1f);
            case PowerUpType.SlowMotion: return new Color(0.6f, 0.4f, 1f);
            case PowerUpType.DoublePoints: return new Color(1f, 0.85f, 0.2f);
            default: return Color.white;
        }
    }

    /// <summary>Creates a simple solid-color sprite for UI icons.</summary>
    public static Sprite CreateIconSprite(PowerUpType type, int size = 32)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color fill = ColorForType(type);
        Color[] pixels = new Color[size * size];
        float radius = size * 0.45f;
        float cx = size * 0.5f;
        float cy = size * 0.5f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - cx;
                float dy = y - cy;
                pixels[y * size + x] = (dx * dx + dy * dy) <= radius * radius ? fill : Color.clear;
            }
        }
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
