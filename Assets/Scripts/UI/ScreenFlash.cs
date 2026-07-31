using MathRunner.UI.Toolkit;
using UnityEngine;

/// <summary>
/// Bridge: forwards flash requests to UI Toolkit OverlayScreen.
/// Keeps existing static call sites (AnswerFeedback, etc.) working.
/// </summary>
public class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public static void FlashGreen()
    {
        if (IsReducedMotion()) return;
        if (UIRouter.Instance != null)
            UIRouter.Instance.FlashCorrect();
    }

    public static void FlashRed()
    {
        if (IsReducedMotion()) return;
        if (UIRouter.Instance != null)
            UIRouter.Instance.FlashWrong();
    }

    private static bool IsReducedMotion()
    {
        return ReducedMotionManager.Instance != null
            && ReducedMotionManager.Instance.IsReducedMotion();
    }
}
