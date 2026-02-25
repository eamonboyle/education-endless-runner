using UnityEngine;

/// <summary>
/// Automatically creates all singleton systems that the game needs at runtime.
/// Attach this to the Game Manager object in the Persistent Scene.
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    private void Awake()
    {
        EnsureSingleton<ComboSystem>();
        EnsureSingleton<LivesSystem>();
        EnsureSingleton<DifficultyPresets>();
        EnsureSingleton<TimeAttackMode>();
        EnsureSingleton<CampaignManager>();
        EnsureSingleton<PowerUpSystem>();
        EnsureSingleton<ScreenShake>();
        EnsureSingleton<AnswerFeedback>();
        EnsureSingleton<ReducedMotionManager>();
        EnsureSingleton<InGameHUD>();
        EnsureSingleton<HighScoreCelebration>();
        EnsureSingleton<SessionSummary>();
        EnsureSingleton<QuestionHistoryDisplay>();
        EnsureSingleton<HighScoreCelebration>();
        EnsureSingleton<UnlockNotification>();
        EnsureSingleton<SessionSummary>();
        EnsureSingleton<InGameHUD>();
    }

    private void EnsureSingleton<T>() where T : MonoBehaviour
    {
        if (FindObjectOfType<T>() == null)
        {
            var go = new GameObject("[" + typeof(T).Name + "]");
            go.transform.SetParent(transform);
            go.AddComponent<T>();
        }
    }
}
