using UnityEngine;

/// <summary>
/// Static utility class for triggering haptic/vibration feedback on mobile devices.
/// No-op on desktop platforms.
/// </summary>
public static class HapticFeedback
{
    /// <summary>
    /// Vibrates the device when the player answers incorrectly.
    /// </summary>
    public static void VibrateOnWrongAnswer()
    {
        if (Application.isMobilePlatform)
        {
            Handheld.Vibrate();
        }
    }

    /// <summary>
    /// Vibrates the device when the player reaches a combo milestone.
    /// </summary>
    public static void VibrateOnComboMilestone()
    {
        if (Application.isMobilePlatform)
        {
            Handheld.Vibrate();
        }
    }
}
