# Math Runner - Development Guide

## Project Overview

Educational endless runner mobile game built with **Unity 2022.3.9f1** (C#). Player runs through a 3D environment answering math questions (addition, subtraction, multiplication, division) by swiping into the correct lane.

## Cursor Cloud specific instructions

### Prerequisites

- **Unity Editor 2022.3.9f1** installed via Unity Hub at `~/Unity/Hub/Editor/2022.3.9f1/Editor/Unity`
- **Unity Hub** installed (`unityhub` package) — used for license management and editor installs
- **.NET SDK 8.0** installed for auxiliary C# tooling

### License Activation

Unity **Personal license** requires signing in through Unity Hub GUI. If `UNITY_USERNAME` / `UNITY_PASSWORD` secrets are set, the license should already be cached. Otherwise:

1. Start dbus: `eval $(dbus-launch --sh-syntax)`
2. Launch Unity Hub: `DISPLAY=:1 unityhub --no-sandbox &`
3. Sign in with a Unity ID in the Hub GUI (Desktop pane)
4. Accept the Personal license terms when prompted

The `--no-sandbox` flag is **required** because this environment runs inside a container. Without it Unity Hub crashes with crashpad/dbus errors.

### Running the Project

**GUI mode** (for play-testing via Desktop pane):
```
DISPLAY=:1 ~/Unity/Hub/Editor/2022.3.9f1/Editor/Unity -projectPath /workspace &
```
Open `Assets/Scenes/Persistent Scene.unity` then press Play. The game flows: Character Select → Mode Choice → Tutorial/Game → Game Over.

**Batch mode** (headless compilation check — no GPU needed):
```
~/Unity/Hub/Editor/2022.3.9f1/Editor/Unity -batchmode -nographics -projectPath /workspace -logFile /tmp/unity.log -quit
```
Exit code 0 = all scripts compiled. Expect ~10 CS warnings (deprecation + unused vars) but zero errors.

### Lint / Build / Test

- **Lint**: No separate linter configured. Compilation warnings serve as the lint check (run batch mode above).
- **Build**: Use Unity batch mode with `-buildTarget` flag (e.g. `-buildTarget StandaloneLinux64`).
- **Tests**: 21 unit tests in `Assets/Editor/Tests/` (QuestionTests + GameStateTests). Run with:
  ```
  ~/Unity/Hub/Editor/2022.3.9f1/Editor/Unity -batchmode -nographics -projectPath /workspace -runTests -testPlatform EditMode -testResults /tmp/test_results.xml -logFile /tmp/unity_tests.log
  ```

### Project Structure (135 C# files)

- `Assets/Scripts/Core/` (9) — GameConstants, GameEnums, CountdownHelper, ObjectPool, SafeFind, LocalizationManager, SaveSystem, InputManager, AnalyticsManager
- `Assets/Scripts/Data/` (6) — PlayerStats, AchievementData, DailyChallengeData, WeeklyChallengeData, XPSystem, PlayerProfile
- `Assets/Scripts/Features/` (22) — ComboSystem, PowerUpSystem, AnswerFeedback, ScorePopup, LeaderboardManager, AccessibilityManager, CharacterUnlockSystem, EnvironmentThemeManager, MusicManager, ScreenShake, SpeedTrailEffect, ParticleEffectLibrary, GhostRunSystem, OnlineLeaderboard, ShareCardGenerator, ChallengeCodeSystem, DyslexiaFontManager, ReducedMotionManager, OneHandedMode, TextToSpeechManager, PowerUpCollectible, PowerUpSpawner
- `Assets/Scripts/GameManagement/` (15) — GameManager, GameState, Score, DifficultyManager, DifficultyPresets, LivesSystem, BossQuestion, TimeAttackMode, CampaignManager, ObstacleSpawner, Obstacle, LevelGenerator, StartCountdown, SoundManager, SceneIndexes
- `Assets/Scripts/UI/` (12) — SceneTransition, ComboDisplay, PowerUpDisplay, AchievementPopup, AnimatedText, StatsDisplay, StatsGraphDisplay, DailyChallengeDisplay, WeeklyChallengeDisplay, GameOverEnhanced, MainMenuEnhanced, RewardAnimation
- `Assets/Editor/Tests/` (2) — QuestionTests (11 tests), GameStateTests (10 tests)
- `.github/workflows/unity-tests.yml` — CI pipeline using GameCI
- `Assets/Scenes/` — Unity scenes: Persistent Scene, MainMenu, CharacterSelect, ModeChoice, Game, Tutorial, Settings
- `Assets/Prefabs/` — Prefab game objects
- `Assets/Models/` — 3D models (low-poly city environment)
- `Packages/manifest.json` — Unity Package Manager dependencies
- `ProjectSettings/` — Unity project configuration

### Key Notes

- No backend services, databases, or Docker containers are needed — the project is entirely self-contained
- Ad monetization code (`Assets/Scripts/Monetization/`) is currently commented out/disabled
- Unity Remote Config is used for difficulty tuning (`DifficultyManager.cs`) but falls back to defaults when offline
- Touch input is simulated via mouse click-and-drag in the Unity Editor (`PlayerController.cs`)
- The project targets Android (min SDK 22) and iOS (min 12.0); a `user.keystore` for Android signing is included in the repo
- First batch-mode open takes ~40s (asset import + compilation). Subsequent opens are ~5s.
- ALSA audio errors in logs are harmless — the VM has no sound card; the game continues without audio
