# Features folder status

Most scripts in this folder were added as aspirational systems and are **not
wired into scenes or prefabs**. They still compile into the player (IL2CPP /
low managed stripping).

## Live (via GameBootstrap / QuestionBox)

- `ComboSystem`
- `PowerUpSystem`
- `AnswerFeedback`
- `ScreenShake`
- `ScorePopup` (static factory)
- `ReducedMotionManager` (instance exists; gameplay does not query it yet)

## Called but non-functional without a scene instance

- `PowerUpSpawner` / `PowerUpCollectible`

## Currently inert (leave alone unless integrating)

Leaderboards, music, themes, unlocks, challenge codes, share cards, TTS,
dyslexia font, one-handed mode, accessibility manager, ghost runs, particle
library, speed trail, online leaderboard.

## Intentionally not bootstrapped (duplicate scene UI)

- `PauseButton` — scene already has `PauseMenu` on `InGameUI`
- `SessionSummary` — scene already has `GameOverUI` canvas
- `QuestionHistoryDisplay` — OnGUI panel stacked on top of `GameOverUI`
