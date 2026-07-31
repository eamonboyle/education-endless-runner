using MathRunner.Data;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows lifetime player stats. Uses <see cref="PlayerStats"/> APIs
/// (not raw PlayerPrefs keys). Builds a simple panel at runtime if refs are unset.
/// </summary>
public class StatsDisplay : MonoBehaviour
{
    [Header("Stat Text References")]
    [SerializeField] private Text totalGamesPlayedText;
    [SerializeField] private Text totalCorrectAnswersText;
    [SerializeField] private Text accuracyText;
    [SerializeField] private Text timePlayedText;

    [Header("Best Streak Per Mode")]
    [SerializeField] private Text bestStreakAdditionText;
    [SerializeField] private Text bestStreakSubtractionText;
    [SerializeField] private Text bestStreakMultiplicationText;
    [SerializeField] private Text bestStreakDivisionText;

    private void Start()
    {
        if (totalGamesPlayedText == null)
            BuildRuntimeUI();
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (totalGamesPlayedText == null) return;

        int gamesPlayed = PlayerStats.GetTotalGamesPlayed();
        int correctAnswers = PlayerStats.GetCorrectAnswers("total");
        float accuracy = PlayerStats.GetAccuracyTotal() * 100f;
        float timePlayed = PlayerStats.GetTotalTimePlayed();

        totalGamesPlayedText.text = "Games: " + gamesPlayed;
        if (totalCorrectAnswersText != null)
            totalCorrectAnswersText.text = "Correct: " + correctAnswers;
        if (accuracyText != null)
            accuracyText.text = "Accuracy: " + accuracy.ToString("F1") + "%";
        if (timePlayedText != null)
            timePlayedText.text = "Time: " + FormatTime(timePlayed);

        SetStreak(bestStreakAdditionText, "Add streak: ", "addition");
        SetStreak(bestStreakSubtractionText, "Sub streak: ", "subtraction");
        SetStreak(bestStreakMultiplicationText, "Mul streak: ", "multiply");
        SetStreak(bestStreakDivisionText, "Div streak: ", "division");
    }

    private static void SetStreak(Text text, string prefix, string mode)
    {
        if (text == null) return;
        text.text = prefix + PlayerStats.GetBestStreak(mode);
    }

    private string FormatTime(float totalSeconds)
    {
        int hours = Mathf.FloorToInt(totalSeconds / 3600f);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        if (hours > 0) return string.Format("{0}h {1}m {2}s", hours, minutes, seconds);
        if (minutes > 0) return string.Format("{0}m {1}s", minutes, seconds);
        return string.Format("{0}s", seconds);
    }

    private void BuildRuntimeUI()
    {
        var canvasGo = new GameObject("StatsDisplayCanvas");
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 40;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        var panel = CreatePanel(canvasGo.transform, "StatsPanel", new Vector2(0f, 1f), new Vector2(12f, -12f), new Vector2(280f, 200f));
        Font font = GetFont();

        totalGamesPlayedText = CreateLabel(panel, "Games", new Vector2(0f, -8f), font);
        totalCorrectAnswersText = CreateLabel(panel, "Correct", new Vector2(0f, -36f), font);
        accuracyText = CreateLabel(panel, "Accuracy", new Vector2(0f, -64f), font);
        timePlayedText = CreateLabel(panel, "Time", new Vector2(0f, -92f), font);
        bestStreakAdditionText = CreateLabel(panel, "Add", new Vector2(0f, -120f), font);
        bestStreakSubtractionText = CreateLabel(panel, "Sub", new Vector2(0f, -144f), font);
        bestStreakMultiplicationText = CreateLabel(panel, "Mul", new Vector2(0f, -168f), font);
        bestStreakDivisionText = CreateLabel(panel, "Div", new Vector2(0f, -192f), font);
    }

    private static RectTransform CreatePanel(Transform parent, string name, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        var img = go.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.55f);
        return rt;
    }

    private static Text CreateLabel(Transform parent, string name, Vector2 pos, Font font)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(-16f, 24f);
        var text = go.AddComponent<Text>();
        text.font = font;
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleLeft;
        return text;
    }

    private static Font GetFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return font != null ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
    }
}
