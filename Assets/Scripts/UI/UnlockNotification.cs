using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton MonoBehaviour that displays unlock notification banners at the
/// top of the screen via OnGUI. Other systems (e.g. CharacterUnlockSystem,
/// EnvironmentThemeManager) enqueue messages through the static
/// <see cref="Enqueue"/> method. Created by <see cref="GameBootstrap"/>.
/// </summary>
public class UnlockNotification : MonoBehaviour
{
    #region Singleton

    public static UnlockNotification Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #endregion

    private struct Notification
    {
        public string Message;
        public float ExpireTime;
    }

    private static readonly Queue<string> pendingMessages = new Queue<string>();

    private Notification? activeNotification;
    private const float DisplayDuration = 3f;

    /// <summary>
    /// Enqueues a notification message for display. Safe to call before
    /// the singleton is initialised; messages are held in a static queue.
    /// </summary>
    public static void Enqueue(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        pendingMessages.Enqueue(message);
    }

    /// <summary>Shorthand for character unlock banners.</summary>
    public static void NotifyCharacterUnlocked(string characterName)
    {
        Enqueue("\U0001F513 New Character Unlocked: " + characterName + "!");
    }

    /// <summary>Shorthand for theme unlock banners.</summary>
    public static void NotifyThemeUnlocked(string themeName)
    {
        Enqueue("\U0001F3A8 New Theme Unlocked: " + themeName + "!");
    }

    private void Update()
    {
        if (activeNotification.HasValue && Time.time >= activeNotification.Value.ExpireTime)
        {
            activeNotification = null;
        }

        if (!activeNotification.HasValue && pendingMessages.Count > 0)
        {
            activeNotification = new Notification
            {
                Message = pendingMessages.Dequeue(),
                ExpireTime = Time.time + DisplayDuration
            };
        }
    }

    private void OnGUI()
    {
        if (!activeNotification.HasValue) return;

        float bannerWidth = Mathf.Min(Screen.width * 0.8f, 500f);
        float bannerHeight = 40f;
        float x = (Screen.width - bannerWidth) * 0.5f;
        float y = 10f;

        GUIStyle bannerStyle = new GUIStyle(GUI.skin.box)
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        bannerStyle.normal.textColor = Color.white;

        Color prev = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.2f, 0.6f, 0.2f, 0.9f);
        GUI.Box(new Rect(x, y, bannerWidth, bannerHeight), activeNotification.Value.Message, bannerStyle);
        GUI.backgroundColor = prev;
    }
}
