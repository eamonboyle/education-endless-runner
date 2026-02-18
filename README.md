# Math Runner

Educational endless runner with arithmetic question gates.

## Required Unity version

- **Unity 6 LTS**: `6000.3.8f1`

## Project setup

1. Install Unity Hub.
2. Install editor version `6000.3.8f1`.
3. Clone this repository.
4. Open the project folder in Unity Hub.
5. Let Package Manager resolve dependencies on first open.

## Run the game

1. Open scene: `Assets/Scenes/Persistent Scene.unity`.
2. Press Play in the Unity Editor.
3. Main menu and game scenes are loaded additively via `GameManager`.

## Controls

- **Keyboard/mouse (Editor):** click/drag swipe gestures
- **Touch devices:** swipe gestures

Input is routed through `PlayerController` with an `IInputService` abstraction:

- `Auto` (default): prefers Unity Input System when available
- `Legacy`: forces classic `UnityEngine.Input` path
- `InputSystem`: forces Unity Input System implementation

## Tests

EditMode tests were added under:

- `Assets/Tests/EditMode/GameStateTests.cs`
- `Assets/Tests/EditMode/QuestionDomainTests.cs`

Run from Unity Test Runner:

1. `Window -> General -> Test Runner`
2. Select **EditMode**
3. Run all tests

## Migration notes

- Project upgraded from `2022.3.9f1` to `6000.3.8f1`.
- Legacy Remote Config and Ads API hard dependencies were removed from gameplay compile path.
- Built-in Render Pipeline was intentionally retained to avoid high-risk visual regressions.
- Lightweight pooling added for frequently reused runtime objects:
  - floor segments (`LevelGenerator`)
  - question boxes (`QuestionGeneration`)
- Scene loading flow in `GameManager` was consolidated to reduce duplicated logic.

## Manual QA checklist

- [ ] Launch from `Persistent Scene` and confirm Main Menu loads.
- [ ] Start a normal run and verify forward movement/lane switching feel unchanged.
- [ ] Answer several questions correctly; verify score increments and new boxes spawn.
- [ ] Answer incorrectly; verify stumble animation + Game Over UI.
- [ ] Restart from Game Over and confirm gameplay loop resets correctly.
- [ ] Pause/resume flow works when app focus is lost/restored.
- [ ] Main menu navigation: mode select, character select, settings, tutorial.
- [ ] Test input in Editor (mouse swipe) and on device (touch swipe).
- [ ] Verify sound toggle and graphics quality toggle persist across scene loads.