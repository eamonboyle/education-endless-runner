using MathRunner.UI.Toolkit;
using UnityEngine;

/// <summary>
/// Bridge: unlock banners go through the Toolkit toast layer.
/// </summary>
public class UnlockNotification : MonoBehaviour
{
    public static void NotifyCharacterUnlocked(string characterName)
    {
        UIRouter.Instance?.ShowToast("Unlocked!", characterName ?? "New character");
    }

    public static void NotifyThemeUnlocked(string themeName)
    {
        UIRouter.Instance?.ShowToast("Theme Unlocked!", themeName ?? "New theme");
    }

    public static void Enqueue(string message)
    {
        UIRouter.Instance?.ShowToast("Unlocked!", message ?? "");
    }
}
