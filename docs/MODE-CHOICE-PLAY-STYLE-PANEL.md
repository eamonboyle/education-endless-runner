# Building the "How do you want to play?" panel by hand

> **Status: done.** The panel is built in `Assets/Scenes/ModeChoice.unity` and the
> runtime version it replaced (`Assets/Scripts/UI/PlayStyleSelect.cs`) has been
> deleted. Steps 1 and 10 are historical and no longer apply. Everything else is
> still the reference for the panel's anchors, colours and wiring — come back
> here when you need to retune it or rebuild a tile.

This built the play-style / difficulty step as real scene objects inside
`Assets/Scenes/ModeChoice.unity`, replacing a version that was generated in code.

A controller script is needed either way — it tracks which tile is selected,
repaints the tiles, and starts the run. What lives in the editor is every
visual: the panel, cards, chips and buttons.

---

## Before you start

### The one gotcha that will waste your afternoon

**Do not use `Assets/Art/MathRunnerButtonBG.png` for these tiles.**

An `Image`'s Color field *multiplies* against its sprite. That sprite is already
blue, so multiplication can only ever push a colour darker and bluer — an amber
tint on it renders dark green, and every card ends up the same blue as the
background. This is exactly what made the first attempt unreadable.

Use Unity's built-in **`UISprite`** instead. It's white and 9-sliced, so colours
come out exactly as you set them and corners keep their radius at any tile size.
Type `UISprite` into the Source Image picker's search box.

### Canvas assumptions

Everything below assumes the existing `Canvas` in `ModeChoice.unity`, which is
already configured as:

| Setting | Value |
| --- | --- |
| UI Scale Mode | Scale With Screen Size |
| Reference Resolution | 800 × 600 |
| Screen Match Mode | Match Width Or Height |
| Match | 0 (width) |
| Reference Pixels Per Unit | 100 |

Because Match is 0, the canvas is 800 units wide at every aspect ratio and its
*height* varies — roughly 1685 units on a tall phone. That's why every position
below is a fractional anchor rather than a pixel offset. Font sizes are in the
same 800-wide reference space.

### How to enter these numbers

Fractional anchors can't be set from the Anchor Presets dropdown. In the
Inspector, expand the **Anchors** foldout on the RectTransform and type Min X/Y
and Max X/Y directly. The position fields above then become **Left / Right /
Top / Bottom** — set all four to `0` unless the table says otherwise.

### Save yourself two thirds of the work

Build **one** card completely (step 4), then duplicate it twice with `Ctrl+D`
and change only the Y anchors and the label text. Same trick for the three
difficulty chips.

---

## Step 1 — Turn off the runtime version *(no longer applies)*

So the generated overlay doesn't sit on top of what you're building.

In `Assets/Scripts/UI/ProgressionUIBootstrap.cs`, comment out the ModeChoice
branch:

```csharp
        else if (name == "ModeChoice" || name == "Mode Choice")
        {
            // EnsureInScene<PlayStyleSelect>(scene, "[PlayStyleSelect]");
        }
```

Uncomment it any time you want to compare against the generated version.

---

## Step 2 — Add the controller script

Create `Assets/Scripts/UI/PlayStylePanel.cs`:

```csharp
using System.Collections;
using MathRunner.Core;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the hand-authored "How do you want to play?" panel in the
/// ModeChoice scene. Every visual is a scene object; this tracks the current
/// selection, repaints the tiles, and commits the choice when Play is pressed.
/// </summary>
public class PlayStylePanel : MonoBehaviour
{
    public enum PlayStyle
    {
        Classic,
        TimeAttack,
        Campaign
    }

    /// <summary>One selectable tile. Subtitle is unused on the difficulty chips.</summary>
    [System.Serializable]
    public class Tile
    {
        public Image body;
        public Image border;
        public Text title;
        public Text subtitle;
    }

    [Header("Panel")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private Text headerSubtitle;

    [Header("Play style tiles")]
    [SerializeField] private Tile classicTile;
    [SerializeField] private Tile timeAttackTile;
    [SerializeField] private Tile campaignTile;

    [Header("Difficulty tiles")]
    [SerializeField] private Tile easyTile;
    [SerializeField] private Tile normalTile;
    [SerializeField] private Tile hardTile;

    [Header("Colours")]
    [SerializeField] private Color surfaceIdle = new Color32(0x0D, 0x3B, 0x60, 0xFF);
    [SerializeField] private Color surfaceSelected = new Color32(0x35, 0xC2, 0xFF, 0xFF);
    [SerializeField] private Color borderIdle = new Color32(0x2A, 0x6A, 0x99, 0xFF);
    [SerializeField] private Color borderSelected = Color.white;
    [SerializeField] private Color textOnIdle = Color.white;
    [SerializeField] private Color textOnIdleMuted = new Color32(0xA8, 0xCC, 0xE4, 0xFF);
    [SerializeField] private Color textOnSelected = new Color32(0x06, 0x28, 0x3F, 0xFF);
    [SerializeField] private Color textOnSelectedMuted = new Color32(0x0A, 0x42, 0x66, 0xFF);

    private const float TransitionSeconds = 0.28f;

    private string questionType = "addition";
    private PlayStyle style = PlayStyle.Classic;
    private DifficultyLevel difficulty = DifficultyLevel.Medium;
    private Coroutine transition;
    private bool visible;

    private void Awake()
    {
        panelGroup.alpha = 0f;
        panelGroup.blocksRaycasts = false;
        panel.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Android back / editor escape returns to the question-type step.
        if (visible && Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
    }

    /// <summary>Opens the panel for the question type the player just chose.</summary>
    /// <param name="chosenQuestionType">PlayerPrefs mode string, e.g. "addition".</param>
    public void Show(string chosenQuestionType)
    {
        questionType = string.IsNullOrEmpty(chosenQuestionType) ? "addition" : chosenQuestionType;
        style = LastUsedStyle();
        difficulty = DifficultyPresets.GetDifficulty();
        RefreshSelection();

        visible = true;
        Animate(true);
    }

    #region Button targets

    public void Back()
    {
        if (!visible) return;
        visible = false;
        Animate(false);
    }

    public void SelectClassic() { style = PlayStyle.Classic; RefreshSelection(); }
    public void SelectTimeAttack() { style = PlayStyle.TimeAttack; RefreshSelection(); }
    public void SelectCampaign() { style = PlayStyle.Campaign; RefreshSelection(); }

    public void SelectEasy() { difficulty = DifficultyLevel.Easy; RefreshSelection(); }
    public void SelectNormal() { difficulty = DifficultyLevel.Medium; RefreshSelection(); }
    public void SelectHard() { difficulty = DifficultyLevel.Hard; RefreshSelection(); }

    public void Play()
    {
        switch (style)
        {
            case PlayStyle.TimeAttack:
                TimeAttackMode.SetTimeAttack(true);
                PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 0);
                GameState.SetQuestionType(questionType);
                break;

            case PlayStyle.Campaign:
                TimeAttackMode.SetTimeAttack(false);
                int level = CampaignManager.GetCurrentLevel();
                PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 1);
                PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_LEVEL, level);
                GameState.SetQuestionType(CampaignManager.GetLevelConfig(level).MathMode.ToPlayerPrefsString());
                break;

            case PlayStyle.Classic:
            default:
                TimeAttackMode.SetTimeAttack(false);
                PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 0);
                GameState.SetQuestionType(questionType);
                break;
        }

        DifficultyPresets.SetDifficulty(difficulty);
        PrefsFlush.Flush();

        visible = false;
        GameSession.BeginRun();
    }

    #endregion

    #region Selection

    /// <summary>Pre-selects whatever the player ran last, so repeat runs are one tap.</summary>
    private static PlayStyle LastUsedStyle()
    {
        if (PlayerPrefs.GetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 0) == 1) return PlayStyle.Campaign;
        if (TimeAttackMode.IsTimeAttack()) return PlayStyle.TimeAttack;
        return PlayStyle.Classic;
    }

    private void RefreshSelection()
    {
        Paint(classicTile, style == PlayStyle.Classic);
        Paint(timeAttackTile, style == PlayStyle.TimeAttack);
        Paint(campaignTile, style == PlayStyle.Campaign);

        Paint(easyTile, difficulty == DifficultyLevel.Easy);
        Paint(normalTile, difficulty == DifficultyLevel.Medium);
        Paint(hardTile, difficulty == DifficultyLevel.Hard);

        int level = CampaignManager.GetCurrentLevel();
        if (campaignTile.subtitle != null)
        {
            campaignTile.subtitle.text = "Level " + level + " of " + CampaignManager.TotalLevels;
        }

        headerSubtitle.text = style == PlayStyle.Campaign
            ? "CAMPAIGN PICKS THE QUESTIONS"
            : DisplayName(questionType).ToUpperInvariant() + " QUESTIONS";
    }

    /// <summary>
    /// Selected tiles invert to a bright fill with dark text. The luminance flip
    /// keeps the state readable without relying on hue alone.
    /// </summary>
    private void Paint(Tile tile, bool selected)
    {
        if (tile.body != null) tile.body.color = selected ? surfaceSelected : surfaceIdle;
        if (tile.border != null) tile.border.color = selected ? borderSelected : borderIdle;
        if (tile.title != null) tile.title.color = selected ? textOnSelected : textOnIdle;
        if (tile.subtitle != null) tile.subtitle.color = selected ? textOnSelectedMuted : textOnIdleMuted;
    }

    #endregion

    #region Transition

    private void Animate(bool show)
    {
        if (transition != null) StopCoroutine(transition);

        bool reducedMotion = ReducedMotionManager.Instance != null &&
                             ReducedMotionManager.Instance.IsReducedMotion();

        if (reducedMotion)
        {
            panel.anchoredPosition = Vector2.zero;
            panelGroup.alpha = show ? 1f : 0f;
            panelGroup.blocksRaycasts = show;
            panel.gameObject.SetActive(show);
            return;
        }

        transition = StartCoroutine(AnimateRoutine(show));
    }

    private IEnumerator AnimateRoutine(bool show)
    {
        panel.gameObject.SetActive(true);
        panelGroup.blocksRaycasts = show;

        float travel = panel.rect.width;
        float from = show ? travel : 0f;
        float to = show ? 0f : travel;
        float fromAlpha = show ? 0f : 1f;
        float toAlpha = show ? 1f : 0f;

        for (float elapsed = 0f; elapsed < TransitionSeconds; elapsed += Time.unscaledDeltaTime)
        {
            float t = elapsed / TransitionSeconds;
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            panel.anchoredPosition = new Vector2(Mathf.Lerp(from, to, eased), 0f);
            panelGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, eased);
            yield return null;
        }

        panel.anchoredPosition = new Vector2(to, 0f);
        panelGroup.alpha = toAlpha;
        panel.gameObject.SetActive(show);
        transition = null;
    }

    #endregion

    private static string DisplayName(string playerPrefsMode)
    {
        switch (playerPrefsMode)
        {
            case "addition": return "Addition";
            case "subtraction": return "Subtraction";
            case "multiply": return "Multiplication";
            case "division": return "Division";
            case "mixed": return "Mixed";
            default: return "Maths";
        }
    }
}
```

Add the component to the **`Main Camera`** object in `ModeChoice.unity`, next to
the existing `ModeSelect` and `SceneSwitcher`. It has to live on an object that
stays enabled, because the panel itself gets deactivated while hidden.

---

## Step 3 — Create the panel root

Right-click `Canvas` → **Create Empty**, name it `PlayStylePanel`, and drag it to
the **bottom** of the Canvas's children. Sibling order is render order in Unity
UI, so last means on top.

| Property | Value |
| --- | --- |
| Anchor Min | `0, 0` |
| Anchor Max | `1, 1` |
| Left / Right / Top / Bottom | `0` |

Add these components:

**Canvas Group** — leave defaults; the script drives Alpha and Blocks Raycasts.

**Image**

| Property | Value |
| --- | --- |
| Source Image | `MathRunnerBG` (`Assets/Art/MathRunnerBG.png`) |
| Color | `D9E0F2` |
| Raycast Target | **on** |

That colour is a light multiply that darkens your existing gradient slightly, so
the tiles in front of it separate. Raycast Target must stay on — this is what
stops taps reaching the four question buttons underneath.

Finally, **uncheck the GameObject's active checkbox** so it starts hidden.
Re-enable it while you build the children, and remember to disable it again
before you save.

---

## Step 4 — Build one card

This is the piece to duplicate. Create it under `PlayStylePanel`.

### `ClassicCard` (Create Empty)

| Property | Value |
| --- | --- |
| Anchor Min | `0.06, 0.605` |
| Anchor Max | `0.94, 0.723` |
| Left / Right / Top / Bottom | `0` |

No components beyond the RectTransform — it's just a frame for the two children.

### `ClassicCard/Border` (UI → Image)

Must be the **first** child so it renders behind the body and shows only as a
ring around the edge.

| Property | Value |
| --- | --- |
| Anchor Min | `0, 0` |
| Anchor Max | `1, 1` |
| Left / Right / Top / Bottom | `-6` (all four negative) |
| Source Image | `UISprite` |
| Image Type | Sliced |
| Color | `2A6A99` |
| Raycast Target | **off** |

### `ClassicCard/Body` (UI → Image)

| Property | Value |
| --- | --- |
| Anchor Min | `0, 0` |
| Anchor Max | `1, 1` |
| Left / Right / Top / Bottom | `0` |
| Source Image | `UISprite` |
| Image Type | Sliced |
| Color | `0D3B60` |
| Raycast Target | **on** |

Add a **Button** component to `Body`. Set Target Graphic to its own Image, and
set the colours so the tint doesn't fight the script's fills:

| Button colour | Value |
| --- | --- |
| Normal | `FFFFFF` |
| Highlighted | `FFFFFF` |
| Pressed | `BFBFBF` |
| Selected | `FFFFFF` |
| Fade Duration | `0.08` |

White Normal means the tint multiplies to no change, leaving the Image's own
colour visible, while Pressed still darkens for touch feedback.

### `ClassicCard/Body/Title` (UI → Legacy → Text)

| Property | Value |
| --- | --- |
| Anchor Min | `0.05, 0.50` |
| Anchor Max | `0.96, 0.88` |
| Left / Right / Top / Bottom | `0` |
| Text | `CLASSIC` |
| Font | `Biber Beard` (`Assets/Fonts/Biber Beard.ttf`) |
| Font Size | `50` |
| Alignment | Middle Left |
| Horizontal Overflow | Wrap |
| Vertical Overflow | Truncate |
| Best Fit | **on**, Min Size `10`, Max Size `50` |
| Color | `FFFFFF` |
| Raycast Target | **off** |

### `ClassicCard/Body/Subtitle` (UI → Legacy → Text)

| Property | Value |
| --- | --- |
| Anchor Min | `0.05, 0.13` |
| Anchor Max | `0.96, 0.46` |
| Left / Right / Top / Bottom | `0` |
| Text | `Endless run` |
| Font | `LegacyRuntime` (the built-in sans) |
| Font Size | `32` |
| Alignment | Middle Left |
| Best Fit | **on**, Min Size `10`, Max Size `32` |
| Color | `A8CCE4` |
| Raycast Target | **off** |

Biber Beard is a display face that sets too tightly to read at subtitle size,
which is why the supporting copy uses the plain built-in font. If you'd rather
keep one typeface throughout, use Biber Beard here too and bump Max Size to 36.

---

## Step 5 — Duplicate the card twice

Select `ClassicCard`, press `Ctrl+D` twice, and change only these:

| Card | Anchor Min | Anchor Max | Title | Subtitle |
| --- | --- | --- | --- | --- |
| `ClassicCard` | `0.06, 0.605` | `0.94, 0.723` | `CLASSIC` | `Endless run` |
| `TimeAttackCard` | `0.06, 0.470` | `0.94, 0.588` | `TIME ATTACK` | `Race a 60 second clock` |
| `CampaignCard` | `0.06, 0.335` | `0.94, 0.453` | `CAMPAIGN` | *(leave empty)* |

The campaign subtitle is filled in at runtime with the player's current level.

---

## Step 6 — Difficulty chips

Same Border + Body + Label structure as a card, minus the subtitle. Build
`EasyChip`, then duplicate it twice.

| Chip | Anchor Min | Anchor Max | Label |
| --- | --- | --- | --- |
| `EasyChip` | `0.060, 0.180` | `0.353, 0.262` | `EASY` |
| `NormalChip` | `0.353, 0.180` | `0.647, 0.262` | `NORMAL` |
| `HardChip` | `0.647, 0.180` | `0.940, 0.262` | `HARD` |

On each chip root, set **Left `8`** and **Right `8`** (Top and Bottom stay `0`).
That's the gutter between neighbours; the anchors themselves are edge to edge.

`Border` and `Body` are identical to the card versions. The label:

### `<Chip>/Body/Label` (UI → Legacy → Text)

| Property | Value |
| --- | --- |
| Anchor Min | `0.06, 0.12` |
| Anchor Max | `0.94, 0.88` |
| Font | `Biber Beard` |
| Font Size | `40` |
| Alignment | Middle Center |
| Best Fit | **on**, Min Size `10`, Max Size `40` |
| Color | `FFFFFF` |
| Raycast Target | **off** |

---

## Step 7 — Header, difficulty label and buttons

All direct children of `PlayStylePanel`.

### `Title` (UI → Legacy → Text)

| Property | Value |
| --- | --- |
| Anchor Min | `0.05, 0.815` |
| Anchor Max | `0.95, 0.908` |
| Text | `HOW DO YOU WANT TO PLAY?` |
| Font | `Biber Beard` |
| Alignment | Middle Center |
| Best Fit | **on**, Min Size `10`, Max Size `62` |
| Color | `FFFFFF` |
| Raycast Target | **off** |

### `QuestionsPill` (UI → Image)

| Property | Value |
| --- | --- |
| Anchor Min | `0.18, 0.748` |
| Anchor Max | `0.82, 0.800` |
| Source Image | `UISprite`, Image Type Sliced |
| Color | `0D3B60` |
| Raycast Target | **off** |

### `QuestionsPill/Label` (UI → Legacy → Text)

| Property | Value |
| --- | --- |
| Anchor Min | `0.04, 0.10` |
| Anchor Max | `0.96, 0.90` |
| Text | *(leave empty — set at runtime)* |
| Font | `LegacyRuntime` |
| Alignment | Middle Center |
| Best Fit | **on**, Min Size `10`, Max Size `34` |
| Color | `A8CCE4` |
| Raycast Target | **off** |

### `DifficultyLabel` (UI → Legacy → Text)

| Property | Value |
| --- | --- |
| Anchor Min | `0.06, 0.274` |
| Anchor Max | `0.94, 0.314` |
| Text | `DIFFICULTY` |
| Font | `LegacyRuntime` |
| Alignment | Middle Center |
| Best Fit | **on**, Min Size `10`, Max Size `32` |
| Color | `A8CCE4` |
| Raycast Target | **off** |

### `BackButton` and `PlayButton`

Both use the same Border + Body + Label structure as a card. Duplicate a chip
and adjust:

| | `BackButton` | `PlayButton` |
| --- | --- | --- |
| Anchor Min | `0.055, 0.928` | `0.16, 0.055` |
| Anchor Max | `0.330, 0.974` | `0.84, 0.152` |
| Left / Right | `0` | `0` |
| Border Color | `245A82` | `FFD98A` |
| Body Color | `0A2E4C` | `FFB020` |
| Label Text | `< BACK` | `PLAY` |
| Label Color | `FFFFFF` | `3A2100` |
| Label Max Size | `34` | `62` |

These two aren't repainted by the script, so their colours are set once here and
stay put. Amber against the blue is what makes Play read as the primary action.

---

## Step 8 — Wire it up

### Inspector references on `PlayStylePanel` (the component on `Main Camera`)

| Field | Drag in |
| --- | --- |
| Panel | `PlayStylePanel` (the GameObject) |
| Panel Group | `PlayStylePanel` (its Canvas Group) |
| Header Subtitle | `QuestionsPill/Label` |
| Classic Tile → body / border / title / subtitle | `ClassicCard/Body`, `ClassicCard/Border`, `.../Title`, `.../Subtitle` |
| Time Attack Tile | the same four from `TimeAttackCard` |
| Campaign Tile | the same four from `CampaignCard` |
| Easy Tile → body / border / title | `EasyChip/Body`, `EasyChip/Border`, `EasyChip/Body/Label` |
| Normal Tile | the same three from `NormalChip` |
| Hard Tile | the same three from `HardChip` |

Leave `subtitle` empty on the three difficulty tiles.

### OnClick events

For each Button, add one entry pointing at `Main Camera`, then pick the method:

| Button | Method |
| --- | --- |
| `ClassicCard/Body` | `PlayStylePanel.SelectClassic` |
| `TimeAttackCard/Body` | `PlayStylePanel.SelectTimeAttack` |
| `CampaignCard/Body` | `PlayStylePanel.SelectCampaign` |
| `EasyChip/Body` | `PlayStylePanel.SelectEasy` |
| `NormalChip/Body` | `PlayStylePanel.SelectNormal` |
| `HardChip/Body` | `PlayStylePanel.SelectHard` |
| `BackButton/Body` | `PlayStylePanel.Back` |
| `PlayButton/Body` | `PlayStylePanel.Play` |

### Hand off from the question buttons

The four question buttons already call `ModeSelect.Choose(string)` and don't need
rewiring. Change `ModeSelect` so it opens the panel instead of loading the menu:

```csharp
using UnityEngine;
using MathRunner.Core;

public class ModeSelect : MonoBehaviour
{
    public GameObject homeButton;
    public PlayStylePanel playStylePanel;

    private void Start()
    {
        homeButton.SetActive(!GameState.IsFirstLoad());
    }

    /// <summary>
    /// Wired to the four question-type buttons in the ModeChoice scene.
    /// </summary>
    public void Choose(string mode)
    {
        GameState.SetQuestionType(mode);

        // First-timers go straight to the tutorial on Classic defaults; asking a
        // new player about Time Attack or Campaign before they have run once is
        // noise they cannot make sense of yet.
        if (GameState.IsFirstLoad())
        {
            TimeAttackMode.SetTimeAttack(false);
            PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 0);
            GameManager.instance.LoadTutorial();
            return;
        }

        if (playStylePanel != null)
        {
            playStylePanel.Show(mode);
            return;
        }

        GameManager.instance.LoadMainMenu();
    }
}
```

Then drag `Main Camera` into the new **Play Style Panel** field on the
`ModeSelect` component.

---

## Step 9 — Test

Play from `Persistent Scene`. `ModeChoice` won't behave correctly if you press
Play with only `ModeChoice.unity` open, because `GameManager` lives in the
persistent scene.

- [ ] Tapping any question button slides the panel in from the right
- [ ] Exactly one card and one difficulty chip look selected (bright cyan fill, dark text, white ring)
- [ ] Tapping a different card moves the selection
- [ ] Selecting Campaign changes the pill to `CAMPAIGN PICKS THE QUESTIONS` and shows your level on the card
- [ ] Selecting anything else shows e.g. `ADDITION QUESTIONS` in the pill
- [ ] Back slides the panel out and the four question buttons work again
- [ ] Play starts a run with the mode and difficulty you picked
- [ ] Nothing behind the panel is tappable while it's open
- [ ] Rotate the Game view between a tall phone and a squat tablet — nothing overlaps

Then check the tuning knobs:

- Text too small anywhere? Raise that Text's **Max Size**. Best Fit will only
  grow up to that number, which is the single most common cause of tiny labels.
- Tiles feel cramped? The card Y anchors in step 5 are the only numbers to move.

### Reset your saved state while testing

The panel pre-selects whatever you played last. To test a fresh install, use
**Edit → Clear All PlayerPrefs**. That also resets first-run, so the next
question tap will send you to the tutorial rather than the panel.

---

## Step 10 — Remove the runtime version *(already done)*

Once the scene version works:

1. Delete `Assets/Scripts/UI/PlayStyleSelect.cs` (and its `.meta`).
2. In `ProgressionUIBootstrap.cs`, delete the whole ModeChoice branch you
   commented out in step 1, and the matching
   `DestroyAllOfType<PlayStyleSelect>();` line in the gameplay-scene cleanup.
3. Update the ModeChoice line in `Assets/Scripts/Features/README.md` to say the
   panel is authored in the scene and driven by `PlayStylePanel`.

Nothing else references `PlayStyleSelect`.

---

## Colour reference

| Role | Hex | Used on |
| --- | --- | --- |
| Backdrop tint | `D9E0F2` | panel background image |
| Surface idle | `0D3B60` | unselected tile body, pill |
| Surface selected | `35C2FF` | selected tile body |
| Border idle | `2A6A99` | unselected tile border |
| Border selected | `FFFFFF` | selected tile border |
| Text on idle | `FFFFFF` | title, unselected labels |
| Text on idle, muted | `A8CCE4` | subtitles, pill, difficulty label |
| Text on selected | `06283F` | selected tile title |
| Text on selected, muted | `0A4266` | selected tile subtitle |
| Play fill | `FFB020` | Play body |
| Play border | `FFD98A` | Play border |
| Play label | `3A2100` | Play label |
| Back fill | `0A2E4C` | Back body |
| Back border | `245A82` | Back border |

Every foreground/background pair here clears WCAG AA (4.5:1); the weakest is the
selected subtitle at 5.2:1 and the strongest is the unselected title at 11.6:1.
If you recolour, keep the selected state *brighter* and the idle state *darker*
than the background — the luminance flip is what makes the selection obvious
without depending on hue, which matters for colour-blind players.
