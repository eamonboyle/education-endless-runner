using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime in-game HUD that creates its own Canvas and UI elements.
/// Displays combo multiplier, remaining lives, and current speed.
/// Falls back to OnGUI if Canvas creation fails.
/// </summary>
public class InGameHUD : MonoBehaviour
{
    private Canvas hudCanvas;
    private Text comboText;
    private Text livesText;
    private Text speedText;
    private bool canvasReady;

    private int currentMultiplier = 1;
    private int currentLives;
    private int maxLives;

    private void Awake()
    {
        try
        {
            CreateHUDCanvas();
            canvasReady = true;
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
    }

    private void OnDisable()
    {
        var combo = ComboSystem.Instance;
        if (combo != null)
            combo.OnMultiplierChanged -= OnMultiplierChanged;

        var lives = LivesSystem.Instance;
        if (lives != null)
            lives.OnLifeLost -= OnLifeLost;
    }

    private void Update()
    {
        if (!GameState.IsRunning()) return;

        var lives = LivesSystem.Instance;
        if (lives != null)
        {
            currentLives = lives.GetLives();
            maxLives = lives.GetMaxLives();
        }

        float speed = GameState.GetCharacterSpeed();

        if (canvasReady)
        {
            UpdateCanvasHUD(speed);
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
    }

    private void OnLifeLost(int remaining)
    {
        currentLives = remaining;
        if (canvasReady && livesText != null)
        {
            livesText.text = BuildLivesString(remaining);
        }
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
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        comboText = CreateUIText(canvasObj.transform, "ComboText",
            new Vector2(0.5f, 0.85f), 48, new Color(1f, 0.84f, 0f));
        comboText.enabled = false;

        livesText = CreateUIText(canvasObj.transform, "LivesText",
            new Vector2(0.05f, 0.95f), 32, Color.red);

        speedText = CreateUIText(canvasObj.transform, "SpeedText",
            new Vector2(0.95f, 0.05f), 20, new Color(1f, 1f, 1f, 0.5f));
    }

    private Text CreateUIText(Transform parent, string name, Vector2 anchorPos, int fontSize, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(400, 60);

        Text text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (text.font == null)
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;

        return text;
    }

    private void UpdateCanvasHUD(float speed)
    {
        if (livesText != null)
            livesText.text = BuildLivesString(currentLives);

        if (speedText != null)
            speedText.text = "Speed: " + Mathf.RoundToInt(speed);
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
            fontSize = 28,
            fontStyle = FontStyle.Bold
        };

        if (currentMultiplier > 1)
        {
            style.normal.textColor = new Color(1f, 0.84f, 0f);
            GUI.Label(new Rect(Screen.width / 2f - 50, Screen.height * 0.1f, 100, 40),
                "x" + currentMultiplier + "!", style);
        }

        style.normal.textColor = Color.red;
        style.fontSize = 24;
        GUI.Label(new Rect(20, 20, 300, 40), BuildLivesString(currentLives), style);

        style.normal.textColor = new Color(1f, 1f, 1f, 0.5f);
        style.fontSize = 18;
        float speed = GameState.GetCharacterSpeed();
        GUI.Label(new Rect(Screen.width - 170, Screen.height - 40, 150, 30),
            "Speed: " + Mathf.RoundToInt(speed), style);
    }
}
