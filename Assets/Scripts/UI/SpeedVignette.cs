using UnityEngine;

public class SpeedVignette : MonoBehaviour
{
    private Texture2D vignetteTexture;
    private float edgeSize = 0.08f;

    private void Awake()
    {
        vignetteTexture = new Texture2D(1, 1);
    }

    private void OnGUI()
    {
        if (!GameState.IsRunning()) return;

        float speed = GameState.GetCharacterSpeed();
        if (speed < 50f) return;

        float intensity;
        if (speed < 80f)
            intensity = Mathf.InverseLerp(50f, 80f, speed) * 0.3f;
        else
            intensity = 0.3f + Mathf.InverseLerp(80f, 120f, speed) * 0.35f;

        intensity = Mathf.Clamp01(intensity);

        Color c = new Color(0f, 0f, 0f, intensity);
        vignetteTexture.SetPixel(0, 0, c);
        vignetteTexture.Apply();

        float w = Screen.width;
        float h = Screen.height;
        float edgeW = w * edgeSize;
        float edgeH = h * edgeSize;

        GUI.DrawTexture(new Rect(0, 0, edgeW, h), vignetteTexture);
        GUI.DrawTexture(new Rect(w - edgeW, 0, edgeW, h), vignetteTexture);
        GUI.DrawTexture(new Rect(0, 0, w, edgeH), vignetteTexture);
        GUI.DrawTexture(new Rect(0, h - edgeH, w, edgeH), vignetteTexture);
    }
}
