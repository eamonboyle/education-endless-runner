using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Runtime in-game HUD that creates its own Canvas and UI elements.
/// Displays combo multiplier, remaining lives, score, and current speed.
/// Sized for portrait mobile; hides the scene's duplicate ScoreText.
/// </summary>
public class InGameHUD : MonoBehaviour
{
    private Canvas hudCanvas;
    private Text comboText;
    private Text livesText;
    private Text speedText;
    private Text scoreText;
    private bool canvasReady;

    private int currentMultiplier = 1;
    private int currentLives;
    private int maxLives;
    private int displayedScore = -1;
    private int displayedSpeed = -1;
    private float speedRefreshTimer;
    private Text sceneScoreText;

    private void Awake()
    {
        try
        {
            CreateHUDCanvas();
            canvasReady = true;
            HideDuplicateSceneScore();
        }
        catch
        {
            canvasReady = false;
        }
    }

    private void OnEnable()
    {
        var combo = ComboSystem.Instance;
        if (combo != null)
            combo.OnMultiplierChanged += OnMultiplierChanged;

        var lives = LivesSystem.Instance;
        if (lives != null)
            lives.OnLifeLost += OnLifeLost;

        GameState.OnScoreChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        var combo = ComboSystem.Instance;
        if (combo != null)
            combo.OnMultiplierChanged -= OnMultiplierChanged;

        var lives = LivesSystem.Instance;
        if (lives != null)
            lives.OnLifeLost -= OnLifeLost;

        GameState.OnScoreChanged -= OnScoreChanged;
    }

    private void Update()
    {
        // Hide when paused / game over so we don't stack on top of scene PauseUI / GameOverUI.
        bool running = GameState.IsRunning();
        if (hudCanvas != null && hudCanvas.enabled != running)
            hudCanvas.enabled = running;

        if (!running) return;

        if (sceneScoreText == null)
            HideDuplicateSceneScore();
        else if (sceneScoreText.enabled)
            sceneScoreText.enabled = false;

        // Refresh lives once at run start / if Instance appears late.
        var lives = LivesSystem.Instance;
        if (lives != null && (currentLives != lives.GetLives() || maxLives != lives.GetMaxLives()))
        {
            currentLives = lives.GetLives();
            maxLives = lives.GetMaxLives();
            if (canvasReady && livesText != null)
                livesText.text = BuildLivesString(currentLives);
        }

        // Speed changes gradually; refresh a few times per second instead of every frame.
        speedRefreshTimer -= Time.deltaTime;
        if (speedRefreshTimer <= 0f)
        {
            speedRefreshTimer = 0.25f;
            RefreshSpeedText();
        }
    }

    private void OnMultiplierChanged(int multiplier)
    {
        currentMultiplier = multiplier;

        if (canvasReady && comboText != null)
        {
            comboText.text = multiplier > 1 ? "x" + multiplier + "!" : "";
            comboText.enabled = multiplier > 1;
        }

        RefreshScoreText(GameState.GetScore());
    }

    private void OnScoreChanged(int score)
    {
        RefreshScoreText(score);
    }

    private void OnLifeLost(int remaining)
    {
        currentLives = remaining;
        if (canvasReady && livesText != null)
        {
            livesText.text = BuildLivesString(remaining);
            StartCoroutine(FlashLivesText());
        }
    }

    private void RefreshScoreText(int score)
    {
        if (displayedScore == score && scoreText != null) return;
        displayedScore = score;

        if (!canvasReady || scoreText == null) return;

        string scoreStr = score.ToString();
        if (currentMultiplier > 1)
            scoreStr += "  x" + currentMultiplier + "!";
        scoreText.text = scoreStr;
    }

    private void RefreshSpeedText()
    {
        int speed = Mathf.RoundToInt(GameState.GetCharacterSpeed());
        if (speed == displayedSpeed) return;
        displayedSpeed = speed;

        if (canvasReady && speedText != null)
            speedText.text = "Speed " + speed;
    }

    private IEnumerator FlashLivesText()
    {
        if (livesText == null) yield break;
        Color original = livesText.color;
        livesText.color = Color.white;
        yield return new WaitForSeconds(0.15f);
        if (livesText != null)
            livesText.color = original;
    }

    private void HideDuplicateSceneScore()
    {
        // Scene InGameUI already draws a raw score number; keep one score display.
        GameObject scoreObj = GameObject.Find("ScoreText");
        if (scoreObj == null) return;

        sceneScoreText = scoreObj.GetComponent<Text>();
        if (sceneScoreText != null)
            sceneScoreText.enabled = false;
    }

    private void CreateHUDCanvas()
    {
        GameObject canvasObj = new GameObject("[InGameHUD_Canvas]");
        canvasObj.transform.SetParent(transform);

        hudCanvas = canvasObj.AddComponent<Canvas>();
        hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        hudCanvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        // Portrait phone reference — landscape 1920x1080 made HUD text tiny on mobile.
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // Lives — top-left, large hearts with outline for sky contrast.
        livesText = CreateUIText(
            canvasObj.transform,
            "LivesText",
            anchorMin: new Vector2(0f, 1f),
            anchorMax: new Vector2(0f, 1f),
            pivot: new Vector2(0f, 1f),
            anchoredPos: new Vector2(36f, -28f),
            size: new Vector2(520f, 100f),
            fontSize: 72,
            color: new Color(1f, 0.25f, 0.35f),
            alignment: TextAnchor.UpperLeft,
            bold: true);

        // Score — top-left under lives (single score source).
        scoreText = CreateUIText(
            canvasObj.transform,
            "ScoreText",
            anchorMin: new Vector2(0f, 1f),
            anchorMax: new Vector2(0f, 1f),
            pivot: new Vector2(0f, 1f),
            anchoredPos: new Vector2(36f, -120f),
            size: new Vector2(520f, 90f),
            fontSize: 64,
            color: Color.white,
            alignment: TextAnchor.UpperLeft,
            bold: true);

        // Combo — below score when active.
        comboText = CreateUIText(
            canvasObj.transform,
            "ComboText",
            anchorMin: new Vector2(0f, 1f),
            anchorMax: new Vector2(0f, 1f),
            pivot: new Vector2(0f, 1f),
            anchoredPos: new Vector2(36f, -210f),
            size: new Vector2(400f, 80f),
            fontSize: 56,
            color: new Color(1f, 0.84f, 0f),
            alignment: TextAnchor.UpperLeft,
            bold: true);
        comboText.enabled = false;

        // Speed — bottom-right (difficulty label uses bottom-left).
        speedText = CreateUIText(
            canvasObj.transform,
            "SpeedText",
            anchorMin: new Vector2(1f, 0f),
            anchorMax: new Vector2(1f, 0f),
            pivot: new Vector2(1f, 0f),
            anchoredPos: new Vector2(-36f, 36f),
            size: new Vector2(360f, 50f),
            fontSize: 36,
            color: new Color(1f, 1f, 1f, 0.85f),
            alignment: TextAnchor.LowerRight,
            bold: true);
    }

    private static Text CreateUIText(
        Transform parent,
        string name,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPos,
        Vector2 size,
        int fontSize,
        Color color,
        TextAnchor alignment,
        bool bold)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;

        Text text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (text.font == null)
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = fontSize;
        text.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;

        // Dark outline so white/red text stays readable on bright sky.
        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.85f);
        outline.effectDistance = new Vector2(3f, -3f);

        Shadow shadow = textObj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.55f);
        shadow.effectDistance = new Vector2(2f, -2f);

        return text;
    }

    private string BuildLivesString(int lives)
    {
        string hearts = "";
        int max = maxLives > 0 ? maxLives : 3;
        for (int i = 0; i < max; i++)
        {
            hearts += i < lives ? "\u2665 " : "\u2661 ";
        }
        return hearts.TrimEnd();
    }

    private void OnGUI()
    {
        if (canvasReady || !GameState.IsRunning()) return;

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(Screen.height * 0.045f),
            fontStyle = FontStyle.Bold
        };

        style.normal.textColor = new Color(1f, 0.25f, 0.35f);
        GUI.Label(new Rect(24, 24, Screen.width * 0.6f, Screen.height * 0.06f),
            BuildLivesString(currentLives), style);

        style.normal.textColor = Color.white;
        style.fontSize = Mathf.RoundToInt(Screen.height * 0.04f);
        string scoreDisplay = GameState.GetScore().ToString();
        if (currentMultiplier > 1)
            scoreDisplay += "  x" + currentMultiplier + "!";
        GUI.Label(new Rect(24, 24 + Screen.height * 0.06f, Screen.width * 0.5f, Screen.height * 0.05f),
            scoreDisplay, style);
    }
}
