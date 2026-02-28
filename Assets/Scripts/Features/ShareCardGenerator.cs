using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Generates a share-ready image from a UI panel showing score, mode,
/// streak, accuracy, and rank. Falls back to a text-only share when
/// rendering fails.
/// </summary>
public class ShareCardGenerator : MonoBehaviour
{
    [SerializeField, Tooltip("The RectTransform of the share-card UI panel to capture.")]
    private RectTransform shareCardPanel;

    [SerializeField, Tooltip("Camera used to render the share card. If null, Camera.main is used.")]
    private Camera renderCamera;

    [SerializeField, Tooltip("Width of the generated share image in pixels.")]
    private int imageWidth = 1080;

    [SerializeField, Tooltip("Height of the generated share image in pixels.")]
    private int imageHeight = 1080;

    /// <summary>
    /// Renders the share card panel to an image, saves it, and invokes
    /// a native share dialog via <c>NativeShare</c> (if available).
    /// Falls back to text-only sharing on failure.
    /// </summary>
    /// <param name="score">The player's score.</param>
    /// <param name="mode">Game mode name.</param>
    /// <param name="streak">Best streak achieved.</param>
    /// <param name="accuracy">Accuracy percentage (0–100).</param>
    /// <param name="rank">Leaderboard rank.</param>
    public void ShareCard(int score, string mode, int streak, float accuracy, int rank)
    {
        string imagePath = null;

        try
        {
            imagePath = RenderCardToFile();
        }
        catch (Exception e)
        {
            Debug.LogWarning("ShareCardGenerator: Render failed – " + e.Message);
        }

        if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
        {
            ShareImage(imagePath, score, mode);
        }
        else
        {
            ShareText(score, mode, streak, accuracy, rank);
        }
    }

    /// <summary>
    /// Renders the share-card panel to a PNG file in
    /// <see cref="Application.persistentDataPath"/>.
    /// </summary>
    /// <returns>Full path to the saved image, or <c>null</c> on failure.</returns>
    private string RenderCardToFile()
    {
        if (shareCardPanel == null)
        {
            Debug.LogWarning("ShareCardGenerator: Share card panel is not assigned.");
            return null;
        }

        Camera cam = renderCamera != null ? renderCamera : Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("ShareCardGenerator: No camera available for rendering.");
            return null;
        }

        RenderTexture rt = new RenderTexture(imageWidth, imageHeight, 24);
        RenderTexture prev = cam.targetTexture;

        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;
        Texture2D screenshot = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        screenshot.Apply();

        cam.targetTexture = prev;
        RenderTexture.active = null;

        rt.Release();
        Destroy(rt);

        byte[] pngData = screenshot.EncodeToPNG();
        Destroy(screenshot);

        if (pngData == null || pngData.Length == 0)
        {
            Debug.LogWarning("ShareCardGenerator: PNG encoding produced no data.");
            return null;
        }

        string filePath = Path.Combine(Application.persistentDataPath, "share_card.png");
        File.WriteAllBytes(filePath, pngData);
        return filePath;
    }

    /// <summary>
    /// Attempts to share an image using the NativeShare plugin.
    /// If NativeShare is not available, logs the share path instead.
    /// </summary>
    private static void ShareImage(string imagePath, int score, string mode)
    {
        string text = $"I scored {score} in {mode} on Math Runner! Can you beat me?";

        // NativeShare integration placeholder.
        // If the NativeShare package is installed:
        //   new NativeShare().AddFile(imagePath).SetText(text).Share();
        Debug.Log($"ShareCardGenerator: Image ready at {imagePath}. Text: {text}");
    }

    /// <summary>
    /// Fallback text-only share when image rendering is unavailable.
    /// </summary>
    private static void ShareText(int score, string mode, int streak, float accuracy, int rank)
    {
        string text = $"Math Runner - {mode}\n" +
                       $"Score: {score}\n" +
                       $"Streak: {streak}\n" +
                       $"Accuracy: {accuracy:F1}%\n" +
                       $"Rank: #{rank}\n" +
                       "Can you beat me?";

        // NativeShare text-only fallback:
        //   new NativeShare().SetText(text).Share();
        Debug.Log("ShareCardGenerator (text fallback): " + text);
    }
}
