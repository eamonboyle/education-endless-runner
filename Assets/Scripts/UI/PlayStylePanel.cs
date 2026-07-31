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