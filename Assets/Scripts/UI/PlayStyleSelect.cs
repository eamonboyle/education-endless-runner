using System.Collections;
using System.Collections.Generic;
using MathRunner.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Second step of the mode-choice flow. Once the player has picked a question
/// type on the ModeChoice screen, this full-screen page asks how they want to
/// play (Classic / Time Attack / Campaign) and at what difficulty, then starts
/// the run.
///
/// The page is built at runtime rather than authored in the scene so it can
/// borrow the font, background and canvas scaling already used by ModeChoice.
/// Surfaces are drawn with a generated rounded sprite instead of the scene's
/// button art: <see cref="Image.color"/> multiplies against its sprite, and the
/// scene sprite is already blue, so tinting it can only ever produce more blue.
/// Spawned by <see cref="ProgressionUIBootstrap"/>.
/// </summary>
public class PlayStyleSelect : MonoBehaviour
{
    /// <summary>Active instance for the loaded ModeChoice scene, if any.</summary>
    public static PlayStyleSelect Instance { get; private set; }

    private enum PlayStyle
    {
        Classic,
        TimeAttack,
        Campaign
    }

    #region Theme

    private static readonly Color SurfaceIdle = new Color32(0x0D, 0x3B, 0x60, 0xFF);
    private static readonly Color SurfaceSelected = new Color32(0x35, 0xC2, 0xFF, 0xFF);
    private static readonly Color BorderIdle = new Color32(0x2A, 0x6A, 0x99, 0xFF);
    private static readonly Color BorderSelected = new Color32(0xFF, 0xFF, 0xFF, 0xFF);

    private static readonly Color TextOnIdle = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
    private static readonly Color TextOnIdleMuted = new Color32(0xA8, 0xCC, 0xE4, 0xFF);
    private static readonly Color TextOnSelected = new Color32(0x06, 0x28, 0x3F, 0xFF);
    private static readonly Color TextOnSelectedMuted = new Color32(0x0A, 0x42, 0x66, 0xFF);

    private static readonly Color PlayFill = new Color32(0xFF, 0xB0, 0x20, 0xFF);
    private static readonly Color PlayBorder = new Color32(0xFF, 0xD9, 0x8A, 0xFF);
    private static readonly Color PlayText = new Color32(0x3A, 0x21, 0x00, 0xFF);

    private static readonly Color BackFill = new Color32(0x0A, 0x2E, 0x4C, 0xFF);
    private static readonly Color BackBorder = new Color32(0x24, 0x5A, 0x82, 0xFF);

    private static readonly Color PageFallback = new Color32(0x07, 0x53, 0x8F, 0xFF);

    /// <summary>Darkens the borrowed background so foreground surfaces separate from it.</summary>
    private static readonly Color BackdropTint = new Color(0.85f, 0.88f, 0.95f, 1f);

    /// <summary>Border thickness, in canvas reference units.</summary>
    private const float BorderThickness = 6f;

    private const float TransitionSeconds = 0.28f;

    #endregion

    /// <summary>A selectable tile: play-style card or difficulty button.</summary>
    private sealed class Chip
    {
        public Image Body;
        public Image Border;
        public Text Title;

        /// <summary>Secondary line. Null on difficulty buttons, which are single-line.</summary>
        public Text Subtitle;
    }

    private readonly Dictionary<PlayStyle, Chip> styleCards = new Dictionary<PlayStyle, Chip>();
    private readonly Dictionary<DifficultyLevel, Chip> difficultyChips = new Dictionary<DifficultyLevel, Chip>();

    private RectTransform page;
    private CanvasGroup pageGroup;
    private Text headerSubtitle;

    /// <summary>The game's display face, used for headings and button labels.</summary>
    private Font displayFont;

    /// <summary>A plain face for small supporting copy, which the display face sets too tightly.</summary>
    private Font bodyFont;

    private string questionType = "addition";
    private PlayStyle style = PlayStyle.Classic;
    private DifficultyLevel difficulty = DifficultyLevel.Medium;
    private Coroutine transition;
    private bool built;
    private bool visible;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        EnsureBuilt();
    }

    private void Update()
    {
        // Android back / editor escape backs out to the question-type step.
        if (visible && Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    /// <summary>
    /// Opens the page for the question type the player just chose.
    /// </summary>
    /// <param name="chosenQuestionType">PlayerPrefs mode string, e.g. "addition".</param>
    public void Show(string chosenQuestionType)
    {
        EnsureBuilt();

        questionType = string.IsNullOrEmpty(chosenQuestionType) ? "addition" : chosenQuestionType;
        style = LastUsedStyle();
        difficulty = DifficultyPresets.GetDifficulty();
        RefreshSelection();

        visible = true;
        Animate(true);
    }

    /// <summary>Closes the page and returns to the question-type step.</summary>
    public void Hide()
    {
        if (!visible) return;
        visible = false;
        Animate(false);
    }

    #region Selection

    /// <summary>Pre-selects whatever the player ran last, so repeat runs are one tap.</summary>
    private static PlayStyle LastUsedStyle()
    {
        if (PlayerPrefs.GetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 0) == 1) return PlayStyle.Campaign;
        if (TimeAttackMode.IsTimeAttack()) return PlayStyle.TimeAttack;
        return PlayStyle.Classic;
    }

    private void SelectStyle(PlayStyle chosen)
    {
        style = chosen;
        RefreshSelection();
    }

    private void SelectDifficulty(DifficultyLevel chosen)
    {
        difficulty = chosen;
        RefreshSelection();
    }

    private void RefreshSelection()
    {
        foreach (KeyValuePair<PlayStyle, Chip> entry in styleCards)
        {
            ApplyChipState(entry.Value, entry.Key == style);
        }

        foreach (KeyValuePair<DifficultyLevel, Chip> entry in difficultyChips)
        {
            ApplyChipState(entry.Value, entry.Key == difficulty);
        }

        int level = CampaignManager.GetCurrentLevel();
        styleCards[PlayStyle.Campaign].Subtitle.text = "Level " + level + " of " + CampaignManager.TotalLevels;

        headerSubtitle.text = style == PlayStyle.Campaign
            ? "CAMPAIGN PICKS THE QUESTIONS"
            : DisplayName(questionType).ToUpperInvariant() + " QUESTIONS";
    }

    /// <summary>
    /// Selected tiles invert to a bright fill with dark text. The luminance flip
    /// keeps the state readable without relying on hue alone.
    /// </summary>
    private static void ApplyChipState(Chip chip, bool selected)
    {
        chip.Body.color = selected ? SurfaceSelected : SurfaceIdle;
        chip.Border.color = selected ? BorderSelected : BorderIdle;
        chip.Title.color = selected ? TextOnSelected : TextOnIdle;

        if (chip.Subtitle != null)
        {
            chip.Subtitle.color = selected ? TextOnSelectedMuted : TextOnIdleMuted;
        }
    }

    #endregion

    #region Commit

    private void StartRun()
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

    #region Transition

    private void Animate(bool show)
    {
        if (transition != null) StopCoroutine(transition);

        bool reducedMotion = ReducedMotionManager.Instance != null &&
                             ReducedMotionManager.Instance.IsReducedMotion();

        if (reducedMotion)
        {
            page.anchoredPosition = Vector2.zero;
            pageGroup.alpha = show ? 1f : 0f;
            pageGroup.blocksRaycasts = show;
            page.gameObject.SetActive(show);
            return;
        }

        transition = StartCoroutine(AnimateRoutine(show));
    }

    private IEnumerator AnimateRoutine(bool show)
    {
        page.gameObject.SetActive(true);
        pageGroup.blocksRaycasts = show;

        float travel = page.rect.width;
        float from = show ? travel : 0f;
        float to = show ? 0f : travel;
        float fromAlpha = show ? 0f : 1f;
        float toAlpha = show ? 1f : 0f;

        for (float elapsed = 0f; elapsed < TransitionSeconds; elapsed += Time.unscaledDeltaTime)
        {
            float t = elapsed / TransitionSeconds;
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            page.anchoredPosition = new Vector2(Mathf.Lerp(from, to, eased), 0f);
            pageGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, eased);
            yield return null;
        }

        page.anchoredPosition = new Vector2(to, 0f);
        pageGroup.alpha = toAlpha;
        page.gameObject.SetActive(show);
        transition = null;
    }

    #endregion

    #region Construction

    private void EnsureBuilt()
    {
        if (built) return;
        built = true;

        HarvestSceneTheme(out displayFont, out Sprite backgroundSprite, out CanvasScaler sceneScaler);
        bodyFont = BuiltinFont();

        var canvasGo = new GameObject("PlayStyleSelectCanvas");
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 60;
        CopyScaler(canvasGo.AddComponent<CanvasScaler>(), sceneScaler);
        canvasGo.AddComponent<GraphicRaycaster>();

        page = CreateRect(canvasGo.transform, "Page", Vector2.zero, Vector2.one);
        pageGroup = page.gameObject.AddComponent<CanvasGroup>();
        pageGroup.alpha = 0f;
        pageGroup.blocksRaycasts = false;

        // Opaque backdrop: doubles as the raycast blocker for the screen below.
        var backdrop = page.gameObject.AddComponent<Image>();
        backdrop.sprite = backgroundSprite;
        backdrop.color = backgroundSprite != null ? BackdropTint : PageFallback;

        BuildHeader();
        BuildStyleCards();
        BuildDifficultyRow();
        BuildStartButton();

        page.gameObject.SetActive(false);
    }

    private void BuildHeader()
    {
        CreateButton("Back", new Vector4(0.055f, 0.928f, 0.330f, 0.974f),
            "< BACK", 34, BackFill, BackBorder, TextOnIdle, Hide);

        CreateText(page, "Title", new Vector4(0.05f, 0.815f, 0.95f, 0.908f),
            "HOW DO YOU WANT TO PLAY?", 62, TextAnchor.MiddleCenter, TextOnIdle, displayFont);

        RectTransform pill = CreateRect(page, "QuestionsPill", new Vector2(0.18f, 0.748f), new Vector2(0.82f, 0.800f));
        Image pillImage = pill.gameObject.AddComponent<Image>();
        ApplySurface(pillImage, SurfaceIdle);
        pillImage.raycastTarget = false;

        headerSubtitle = CreateText(pill, "Label", new Vector4(0.04f, 0.1f, 0.96f, 0.9f),
            string.Empty, 34, TextAnchor.MiddleCenter, TextOnIdleMuted, bodyFont);
    }

    private void BuildStyleCards()
    {
        styleCards[PlayStyle.Classic] = CreateCard(
            "Classic", 0.605f, 0.723f, "CLASSIC", "Endless run",
            () => SelectStyle(PlayStyle.Classic));

        styleCards[PlayStyle.TimeAttack] = CreateCard(
            "TimeAttack", 0.470f, 0.588f, "TIME ATTACK", "Race a 60 second clock",
            () => SelectStyle(PlayStyle.TimeAttack));

        styleCards[PlayStyle.Campaign] = CreateCard(
            "Campaign", 0.335f, 0.453f, "CAMPAIGN", string.Empty,
            () => SelectStyle(PlayStyle.Campaign));
    }

    private void BuildDifficultyRow()
    {
        CreateText(page, "DifficultyLabel", new Vector4(0.06f, 0.274f, 0.94f, 0.314f),
            "DIFFICULTY", 32, TextAnchor.MiddleCenter, TextOnIdleMuted, bodyFont);

        difficultyChips[DifficultyLevel.Easy] = CreateDifficultyChip("EASY", 0.060f, 0.353f, DifficultyLevel.Easy);
        difficultyChips[DifficultyLevel.Medium] = CreateDifficultyChip("NORMAL", 0.353f, 0.647f, DifficultyLevel.Medium);
        difficultyChips[DifficultyLevel.Hard] = CreateDifficultyChip("HARD", 0.647f, 0.940f, DifficultyLevel.Hard);
    }

    private void BuildStartButton()
    {
        CreateButton("Play", new Vector4(0.16f, 0.055f, 0.84f, 0.152f),
            "PLAY", 62, PlayFill, PlayBorder, PlayText, StartRun);
    }

    private Chip CreateCard(string name, float yMin, float yMax, string title, string subtitle, UnityAction onClick)
    {
        RectTransform card = CreateRect(page, name + "Card", new Vector2(0.06f, yMin), new Vector2(0.94f, yMax));

        Image border = CreateBorder(card);
        Image body = CreateBody(card, onClick);

        Text titleText = CreateText(body.rectTransform, "Title", new Vector4(0.05f, 0.50f, 0.96f, 0.88f),
            title, 50, TextAnchor.MiddleLeft, TextOnIdle, displayFont);
        Text subtitleText = CreateText(body.rectTransform, "Subtitle", new Vector4(0.05f, 0.13f, 0.96f, 0.46f),
            subtitle, 32, TextAnchor.MiddleLeft, TextOnIdleMuted, bodyFont);

        return new Chip { Body = body, Border = border, Title = titleText, Subtitle = subtitleText };
    }

    private Chip CreateDifficultyChip(string label, float xMin, float xMax, DifficultyLevel level)
    {
        RectTransform holder = CreateRect(page, label + "Chip", new Vector2(xMin, 0.180f), new Vector2(xMax, 0.262f));
        // Gutter between neighbouring chips, applied in units so it stays even.
        holder.offsetMin = new Vector2(8f, 0f);
        holder.offsetMax = new Vector2(-8f, 0f);

        Image border = CreateBorder(holder);
        Image body = CreateBody(holder, () => SelectDifficulty(level));

        Text labelText = CreateText(body.rectTransform, "Label", new Vector4(0.06f, 0.12f, 0.94f, 0.88f),
            label, 40, TextAnchor.MiddleCenter, TextOnIdle, displayFont);

        return new Chip { Body = body, Border = border, Title = labelText, Subtitle = null };
    }

    /// <summary>
    /// Outlined backing plate, declared before the body so it renders underneath
    /// and shows only as a ring around the edge.
    /// </summary>
    private static Image CreateBorder(RectTransform parent)
    {
        RectTransform rect = CreateRect(parent, "Border", Vector2.zero, Vector2.one);
        rect.offsetMin = new Vector2(-BorderThickness, -BorderThickness);
        rect.offsetMax = new Vector2(BorderThickness, BorderThickness);

        var image = rect.gameObject.AddComponent<Image>();
        ApplySurface(image, BorderIdle);
        image.raycastTarget = false;
        return image;
    }

    private static Image CreateBody(RectTransform parent, UnityAction onClick)
    {
        RectTransform rect = CreateRect(parent, "Body", Vector2.zero, Vector2.one);

        var image = rect.gameObject.AddComponent<Image>();
        ApplySurface(image, SurfaceIdle);

        AttachButton(rect.gameObject, image, onClick);
        return image;
    }

    private void CreateButton(string name, Vector4 anchors, string label, int fontSize,
        Color fill, Color border, Color labelColour, UnityAction onClick)
    {
        RectTransform holder = CreateRect(page, name,
            new Vector2(anchors.x, anchors.y), new Vector2(anchors.z, anchors.w));

        Image borderImage = CreateBorder(holder);
        borderImage.color = border;

        Image body = CreateBody(holder, onClick);
        body.color = fill;

        CreateText(body.rectTransform, "Label", new Vector4(0.06f, 0.12f, 0.94f, 0.88f),
            label, fontSize, TextAnchor.MiddleCenter, labelColour, displayFont);
    }

    /// <summary>
    /// Adds a <see cref="Button"/> whose colour tint multiplies the graphic's own
    /// colour, so selection tinting and press feedback stay independent.
    /// </summary>
    private static void AttachButton(GameObject target, Graphic graphic, UnityAction onClick)
    {
        var button = target.AddComponent<Button>();
        button.targetGraphic = graphic;
        button.transition = Selectable.Transition.ColorTint;

        ColorBlock colours = ColorBlock.defaultColorBlock;
        colours.normalColor = Color.white;
        colours.highlightedColor = Color.white;
        colours.selectedColor = Color.white;
        colours.pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
        colours.fadeDuration = 0.08f;
        button.colors = colours;

        button.onClick.AddListener(onClick);
    }

    private static void ApplySurface(Image image, Color colour)
    {
        image.sprite = RoundedSprite();
        image.type = Image.Type.Sliced;
        image.color = colour;
    }

    private static Text CreateText(RectTransform parent, string name, Vector4 anchors, string content,
        int maxFontSize, TextAnchor alignment, Color colour, Font face)
    {
        RectTransform rect = CreateRect(parent, name,
            new Vector2(anchors.x, anchors.y), new Vector2(anchors.z, anchors.w));

        var text = rect.gameObject.AddComponent<Text>();
        text.font = face;
        text.fontSize = maxFontSize;
        text.alignment = alignment;
        text.color = colour;
        text.text = content;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 10;
        text.resizeTextMaxSize = maxFontSize;
        return text;
    }

    private static RectTransform CreateRect(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        var rect = (RectTransform)go.transform;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }

    #endregion

    #region Generated sprite

    private static Sprite roundedSprite;

    /// <summary>
    /// A white rounded rectangle with nine-slice borders, generated once and
    /// shared. White so <see cref="Image.color"/> reproduces tints faithfully,
    /// and sliced so corners keep their radius on tiles of any aspect ratio.
    /// </summary>
    private static Sprite RoundedSprite()
    {
        if (roundedSprite != null) return roundedSprite;

        const int size = 64;
        const float radius = 16f;
        const float half = size * 0.5f;
        const float inner = half - radius;

        // Kept alive by the static field rather than HideFlags.DontSave, which
        // makes the editor report a leaked object every time play mode exits.
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        var pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = Mathf.Abs(x + 0.5f - half) - inner;
                float dy = Mathf.Abs(y + 0.5f - half) - inner;
                float outside = Mathf.Sqrt(Mathf.Max(dx, 0f) * Mathf.Max(dx, 0f) +
                                           Mathf.Max(dy, 0f) * Mathf.Max(dy, 0f));
                float distance = outside + Mathf.Min(Mathf.Max(dx, dy), 0f) - radius;
                pixels[y * size + x] = new Color32(255, 255, 255, (byte)(Mathf.Clamp01(0.5f - distance) * 255f));
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();

        roundedSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f),
            100f, 0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
        return roundedSprite;
    }

    #endregion

    #region Scene theme

    /// <summary>
    /// Reads the font, background and canvas scaling from the ModeChoice scene so
    /// the generated page sits in the same visual world as the screen behind it.
    /// </summary>
    private static void HarvestSceneTheme(out Font sceneFont, out Sprite backgroundSprite, out CanvasScaler scaler)
    {
        sceneFont = null;
        backgroundSprite = null;
        scaler = Object.FindAnyObjectByType<CanvasScaler>();

        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            if (button == null || button.name != "Addition") continue;

            var label = button.GetComponentInChildren<Text>(true);
            if (label != null) sceneFont = label.font;
            break;
        }

        Image[] images = Object.FindObjectsByType<Image>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Image image in images)
        {
            if (image != null && image.name == "BG")
            {
                backgroundSprite = image.sprite;
                break;
            }
        }

        if (sceneFont == null) sceneFont = BuiltinFont();
    }

    private static Font BuiltinFont()
    {
        return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
               ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private static void CopyScaler(CanvasScaler target, CanvasScaler source)
    {
        target.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        if (source != null && source.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            target.referenceResolution = source.referenceResolution;
            target.screenMatchMode = source.screenMatchMode;
            target.matchWidthOrHeight = source.matchWidthOrHeight;
            return;
        }

        target.referenceResolution = new Vector2(800f, 600f);
        target.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        target.matchWidthOrHeight = 0f;
    }

    #endregion

    #region Display names

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

    #endregion
}
