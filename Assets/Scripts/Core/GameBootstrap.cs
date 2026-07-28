using UnityEngine;
using MathRunner.Core;
using MathRunner.UI.Toolkit;

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
        EnsureSingleton<PowerUpSpawner>();
        // PowerUpDisplay / InGameHUD replaced by UI Toolkit HudScreen via UIRouter.
        EnsureSingleton<ObstacleSpawner>();
        EnsureSingleton<ScreenShake>();
        EnsureSingleton<AnswerFeedback>();
        EnsureSingleton<ReducedMotionManager>();
        // UI Toolkit root + router (replaces OnGUI overlays and runtime uGUI HUDs).
        EnsureSingleton<UIRouter>();
        EnsureSingleton<LocalizationManager>();
        // Thin bridges keep existing call sites working during migration.
        EnsureSingleton<ScreenFlash>();
        EnsureSingleton<UnlockNotification>();
        EnsureSingleton<HighScoreCelebration>();
        EnsureSingleton<DifficultyIndicator>();
        EnsureSingleton<SpeedVignette>();
        EnsureSingleton<LaneIndicator>();
        EnsureSingleton<PauseButton>();
        EnsureSingleton<AchievementPopup>();
        EnsureSingleton<RewardAnimation>();
        EnsureSingleton<AccessibilityManager>();
        EnsureSingleton<DyslexiaFontManager>();
        EnsureSingleton<InputManager>();
        EnsureSingleton<TextToSpeechManager>();
        EnsureSingleton<ParticleEffectLibrary>();
        EnsureSingleton<MusicManager>();
        // Progression overlays are now Toolkit modals (stats / challenges / a11y).
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
