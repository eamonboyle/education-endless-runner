using UnityEngine;

/// <summary>
/// Sound state management. Sound settings are handled through SettingState.
/// This class is retained for backward compatibility.
/// </summary>
public static class SoundState
{
    public static bool IsMuted()
    {
        return !SettingState.GetSound();
    }

    public static void ToggleMute()
    {
        SettingState.ChangeSound();
    }
}
