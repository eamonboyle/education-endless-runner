# Math Runner - Development Guide

## Project Overview

Educational endless runner mobile game built with **Unity 2022.3.9f1** (C#). Player runs through a 3D environment answering math questions (addition, subtraction, multiplication, division) by swiping into the correct lane.

## Cursor Cloud specific instructions

### Prerequisites

- **Unity Editor 2022.3.9f1** installed via Unity Hub at `~/Unity/Hub/Editor/2022.3.9f1/Editor/Unity`
- **Unity Hub** installed (`unityhub` package) — used for license management and editor installs
- **.NET SDK 8.0** installed for auxiliary C# tooling

### License Activation (Required Before First Use)

Unity **Personal license** requires signing in through Unity Hub GUI. In this cloud VM:

1. Start dbus: `eval $(dbus-launch --sh-syntax)`
2. Launch Unity Hub: `DISPLAY=:1 unityhub --no-sandbox &`
3. Sign in with a Unity ID in the Hub GUI (Desktop pane)
4. Activate a Personal license via Hub → Preferences → Licenses → Add → Personal

The `--no-sandbox` flag is required because this environment runs inside a container.

### Opening the Project

After license activation:
```
~/Unity/Hub/Editor/2022.3.9f1/Editor/Unity -projectPath /workspace
```

Or in batch mode (headless compilation/build):
```
~/Unity/Hub/Editor/2022.3.9f1/Editor/Unity -batchmode -nographics -projectPath /workspace -logFile /tmp/unity.log -quit
```

### Project Structure

- `Assets/Scripts/` — All C# game scripts (37 files across 9 directories)
- `Assets/Scenes/` — Unity scenes: Persistent Scene, MainMenu, CharacterSelect, ModeChoice, Game, Tutorial, Settings
- `Assets/Prefabs/` — Prefab game objects
- `Assets/Models/` — 3D models (low-poly city environment)
- `Packages/manifest.json` — Unity Package Manager dependencies
- `ProjectSettings/` — Unity project configuration

### Key Notes

- No automated test suites exist in this project (no `Tests/` directory or `.asmdef` test assemblies)
- No backend services, databases, or Docker containers are needed — the project is entirely self-contained
- Ad monetization code (`Assets/Scripts/Monetization/`) is currently commented out/disabled
- Unity Remote Config is used for difficulty tuning (`DifficultyManager.cs`) but falls back to defaults when offline
- Touch input is simulated via mouse in the Unity Editor (see `PlayerController.cs`)
- The project targets Android (min SDK 22) and iOS (min 12.0); a `user.keystore` for Android signing is included in the repo
