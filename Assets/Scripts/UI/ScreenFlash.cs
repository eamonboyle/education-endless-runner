using UnityEngine;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash Instance { get; private set; }
    
    private Color flashColor = Color.clear;
    private float flashAlpha = 0f;
    private float flashDuration = 0.3f;
    private float flashTimer = 0f;
    private bool isFlashing = false;
    private Texture2D flashTexture;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        flashTexture = new Texture2D(1, 1);
    }

    public static void FlashGreen()
    {
        if (Instance != null)
            Instance.StartFlash(new Color(0.2f, 1f, 0.2f, 0.3f), 0.25f);
    }

    public static void FlashRed()
    {
        if (Instance != null)
            Instance.StartFlash(new Color(1f, 0.2f, 0.2f, 0.35f), 0.3f);
    }

    private void StartFlash(Color color, float duration)
    {
        flashColor = color;
        flashDuration = duration;
        flashTimer = duration;
        flashAlpha = color.a;
        isFlashing = true;
    }

    private void Update()
    {
        if (!isFlashing) return;
        flashTimer -= Time.deltaTime;
        if (flashTimer <= 0f)
        {
            isFlashing = false;
            flashAlpha = 0f;
        }
        else
        {
            flashAlpha = flashColor.a * (flashTimer / flashDuration);
        }
    }

    private void OnGUI()
    {
        if (!isFlashing || flashAlpha <= 0f) return;
        Color c = new Color(flashColor.r, flashColor.g, flashColor.b, flashAlpha);
        flashTexture.SetPixel(0, 0, c);
        flashTexture.Apply();
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), flashTexture);
    }
}
