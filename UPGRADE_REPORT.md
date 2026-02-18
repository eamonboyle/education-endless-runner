# Unity Upgrade & Refactor Report

## Version upgrade

- **From:** `2022.3.9f1`
- **To:** `6000.3.8f1` (Unity 6 LTS)

`ProjectSettings/ProjectVersion.txt` was updated accordingly.

## Core migration summary

### 1) Baseline engine/package modernization

- Updated package manifest to newer stable package versions compatible with modern Unity.
- Added `com.unity.inputsystem` and enabled dual input backend support in Project Settings:
  - `activeInputHandler: 2` (Both)

### 2) Compile-risk cleanup for legacy APIs

- Removed hard compile dependencies on:
  - `Unity.RemoteConfig` legacy API
  - `UnityEngine.Advertisements` legacy API types
- This keeps gameplay compile path stable on modern Unity, while preserving existing run loop behavior.

### 3) Input modernization (compatibility-first)

- Introduced input abstraction:
  - `IInputService`
  - `LegacyInputService`
  - `InputSystemInputService`
  - `SwipeInputState`
- `PlayerController` now reads input through the service layer and keeps the same public swipe flags used by gameplay code.
- Added backend selection mode in `PlayerController`:
  - `Auto` / `Legacy` / `InputSystem`

### 4) Scene/state and performance refactor

- Consolidated duplicated scene loading paths in `GameManager` through a single `LoadScene(...)` flow.
- Added lightweight pooling:
  - **Floor pooling** in `LevelGenerator`
  - **Question box pooling** in `QuestionGeneration`
- Refactored `QuestionBox` to use explicit owner initialization instead of repeated global lookups/destruction logic.

### 5) Tests

Added EditMode tests for critical logic:

- `GameStateTests`
  - high score persistence behavior
  - first-load default settings seeding
- `QuestionDomainTests`
  - answer inclusion in options
  - correct-lane mapping logic
  - operator text consistency by selected mode

## Direct package changes (manifest)

### Added

- `com.unity.inputsystem: 1.18.0`

### Updated

- `com.unity.ai.navigation: 1.1.4 -> 2.0.10`
- `com.unity.collab-proxy: 2.0.7 -> 2.11.3`
- `com.unity.ext.nunit: 1.0.6 -> 2.0.5`
- `com.unity.ide.visualstudio: 2.0.18 -> 2.0.27`
- `com.unity.test-framework: 1.1.33 -> 1.4.6`
- `com.unity.textmeshpro: 3.0.6 -> 3.0.9`
- `com.unity.timeline: 1.7.5 -> 1.8.10`

### Removed

- `com.unity.ads`
- `com.unity.remote-config`

## Known risks / follow-ups

1. **Unity import/API updater pass still required locally**
   - This environment cannot run Unity Editor batch compilation/import.
   - On first local open in `6000.3.8f1`, allow API Updater + package resolve to complete.

2. **Ads/Remote Config behavior**
   - Legacy compile-time dependencies were removed.
   - If monetization/remote tuning is required, migrate to current Unity Services SDK APIs in a dedicated follow-up.

3. **Built-in Render Pipeline intentionally retained**
   - No URP migration was attempted to avoid broad material/shader regression risk.
   - URP can be a future dedicated migration stream if desired.

4. **Legacy static GameState architecture remains**
   - Hot-path refactors were applied incrementally.
   - A deeper state/persistence service decomposition can be done later without urgent runtime risk.

## Validation guidance

Use the manual QA checklist in `README.md` (main menu flow, run/death/restart loop, scoring, pause/resume, input on mouse+touch, settings persistence).
