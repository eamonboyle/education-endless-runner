# Features folder status

Updated after the dead-feature wiring pass.

## Live (via GameBootstrap)

- `ComboSystem`, `LivesSystem`, `DifficultyPresets`, `TimeAttackMode`, `CampaignManager`
- `PowerUpSystem`, `PowerUpSpawner`, `PowerUpDisplay` (procedural pickups + HUD slots)
- `ScreenShake` (shakes `Camera.main`), `AnswerFeedback`, `ScorePopup`
- `ReducedMotionManager` (queried by shake / flash / particles / dust / popups)
- `AccessibilityManager`, `DyslexiaFontManager`, `InputManager`, `TextToSpeechManager`
- `ParticleEffectLibrary`, `MusicManager` (loads `Resources/bg-music1` + `gameover`)
- `InGameHUD`, `HighScoreCelebration`, `ScreenFlash`, `UnlockNotification`, `DifficultyIndicator`
- `AchievementPopup`, `RewardAnimation`, `ProgressionUIBootstrap`, `ObstacleSpawner`

## Progression

- `RunEndPipeline` (from `GameState.ShowGameOverUI`) awards XP, achievements, game/time played,
  character unlocks, campaign completion, and last-played / daily graph prefs.
- MainMenu / Settings get runtime `DailyChallengeDisplay`, `WeeklyChallengeDisplay`, `StatsDisplay`,
  and `AccessibilitySettingsUI` via `ProgressionUIBootstrap`.
- ModeChoice gets `PlayStyleSelect`, a second step shown after the player picks a question type
  (Classic / Time Attack / Campaign + difficulty). It borrows the scene's button sprite, font and
  canvas scaling at runtime, so it restyles itself if the ModeChoice art changes.

## Still optional / art-dependent (manual Unity polish)

- Hand-authored power-up pickup prefab under `Resources/PowerUpPickup` (code falls back to spheres)
- Dyslexia font asset assigned on `DyslexiaFontManager` (OpenDyslexic / Lexend)
- Dedicated menu/boss music tracks
- CharacterSelect lock visuals for the 8 unlockable characters
- Hand-authored challenge / stats panels replacing the runtime overlays

## Intentionally not bootstrapped

- `PauseButton`, `SessionSummary`, `QuestionHistoryDisplay` — duplicate scene Pause/GameOver UI

## Deleted as superseded

- `SettingsEnhanced`, `MainMenuEnhanced`, `ModeSelectEnhanced`, `GameOverEnhanced`
- `ComboDisplay`, `Menu/GameOver`, `SaveSystem`, `PlayerProfile`, `GameOverDataProvider`
- `TutorialCountdown`, legacy `QuestionGenerator`
