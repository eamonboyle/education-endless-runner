using UnityEngine;
using MathRunner.Core;

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
        EnsureSingleton<LeaderboardManager>();
        EnsureSingleton<OnlineLeaderboard>();
        EnsureSingleton<PowerUpSystem>();
        EnsureSingleton<PowerUpSpawner>();
        EnsureSingleton<PowerUpDisplay>();
        EnsureSingleton<ObstacleSpawner>();
        EnsureSingleton<ScreenShake>();
        EnsureSingleton<AnswerFeedback>();
        EnsureSingleton<ReducedMotionManager>();
        // InGameHUD overlays score/lives/speed during gameplay (hidden when paused/over).
        EnsureSingleton<InGameHUD>();
        EnsureSingleton<HighScoreCelebration>();
        // SessionSummary / PauseButton / QuestionHistoryDisplay intentionally omitted —
        // they draw OnGUI overlays that duplicate the scene PauseMenu and GameOverUI canvases.
        EnsureSingleton<ScreenFlash>();
        EnsureSingleton<UnlockNotification>();
        EnsureSingleton<DifficultyIndicator>();
        EnsureSingleton<AchievementPopup>();
        EnsureSingleton<RewardAnimation>();
        EnsureSingleton<LocalizationManager>();
        EnsureSingleton<AccessibilityManager>();
        EnsureSingleton<DyslexiaFontManager>();
        EnsureSingleton<InputManager>();
        EnsureSingleton<TextToSpeechManager>();
        EnsureSingleton<ParticleEffectLibrary>();
        EnsureSingleton<MusicManager>();
        EnsureSingleton<EnvironmentThemeManager>();
        EnsureSingleton<ProgressionUIBootstrap>();
    }

    private void EnsureSingleton<T>() where T : Component
    {
        if (FindAnyObjectByType<T>() == null)
        {
            var go = new GameObject("[" + typeof(T).Name + "]");
            go.AddComponent<T>();
        }
    }
}
