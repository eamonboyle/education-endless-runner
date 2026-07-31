using MathRunner.UI.Toolkit;
using UnityEngine;

/// <summary>
/// Bridge: achievement popups use the Toolkit toast document.
/// </summary>
public class AchievementPopup : MonoBehaviour
{
    public static AchievementPopup Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ShowAchievement(string achievementName, string description)
    {
        UIRouter.Instance?.ShowToast(achievementName ?? "Achievement", description ?? "");
    }

    public static void Show(string achievementName, string description)
    {
        if (Instance != null)
            Instance.ShowAchievement(achievementName, description);
        else
            UIRouter.Instance?.ShowToast(achievementName ?? "Achievement", description ?? "");
    }
}
