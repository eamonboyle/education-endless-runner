# Math Runner UI Toolkit

Runtime UI is driven by **UI Toolkit** (`UIDocument` + UXML/USS).

## Layout

| Path | Purpose |
|------|---------|
| `Assets/Resources/UI/Screens/*.uxml` | Screen trees loaded at runtime via `Resources.Load` |
| `Assets/Resources/UI/Styles/*.uss` | Design tokens, components, accessibility (also mirrored under `Assets/UI/Styles/`) |
| `Assets/Art/UI/` | Painted logo, button skins, asphalt dock (Sprite 2D and UI, 9-sliced in USS) |
| `Assets/UI/Themes/MathRunner.tss` | Theme Style Sheet importing the USS sheets |
| `Assets/UI/PanelSettings/` | Create via menu **Math Runner → UI → Create Panel Settings** (optional; runtime creates PanelSettings if missing) |
| `Assets/Scripts/UI/Toolkit/` | `UIRoot`, `UIRouter`, `UIScreen`, `NavigationService` |
| `Assets/Scripts/UI/Screens/` | Per-screen controllers |
| `Assets/Scripts/UI/ViewModels/` | Bindable HUD / progression state |

## Layers (shared PanelSettings, 1080×1920 portrait)

0 HUD · 10 Overlay · 20 Modal · 30 Toast · 100 Transition

Non-interactive roots use `picking-mode="Ignore"` so swipe input reaches gameplay.

## Entry point

`GameBootstrap` / `GameManager` ensure `UIRouter` exists. Scene loads call `NavigationService.OnSceneLoaded` to show the matching modal and disable legacy scene Canvases.
