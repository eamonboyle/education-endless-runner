using MathRunner.Data;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the current weekly challenge. Builds a panel at runtime when refs are missing.
/// </summary>
public class WeeklyChallengeDisplay : MonoBehaviour
{
    [SerializeField] private Text challengeDescriptionText;
    [SerializeField] private Image progressBarFill;
    [SerializeField] private Text progressText;
    [SerializeField] private GameObject completedBadge;
    [SerializeField] private Text completedText;

    private void Start()
    {
        if (challengeDescriptionText == null)
            BuildRuntimeUI();
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        WeeklyChallenge challenge = WeeklyChallengeData.GetThisWeekChallenge();
        if (challenge == null || challengeDescriptionText == null) return;

        challengeDescriptionText.text = "Weekly: " + challenge.Description;

        int current = challenge.CurrentCount;
        int target = challenge.TargetCount;
        bool isCompleted = challenge.IsComplete;

        if (progressBarFill != null)
            progressBarFill.fillAmount = target > 0 ? Mathf.Clamp01((float)current / target) : 0f;

        if (progressText != null)
            progressText.text = current + " / " + target;

        if (completedBadge != null)
            completedBadge.SetActive(isCompleted);

        if (completedText != null)
        {
            completedText.text = isCompleted ? "Completed!" : "In Progress";
            completedText.color = isCompleted ? Color.green : Color.yellow;
        }
    }

    private void BuildRuntimeUI()
    {
        var canvasGo = new GameObject("WeeklyChallengeCanvas");
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 42;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        var panel = new GameObject("WeeklyPanel");
        panel.transform.SetParent(canvasGo.transform, false);
        var rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-12f, -120f);
        rt.sizeDelta = new Vector2(300f, 100f);
        panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        challengeDescriptionText = CreateText(panel.transform, "Desc", new Vector2(0f, -8f), 70f, font, 13);
        progressText = CreateText(panel.transform, "Progress", new Vector2(0f, -48f), 24f, font, 14);
        completedText = CreateText(panel.transform, "Status", new Vector2(0f, -72f), 24f, font, 14);

        var fillGo = new GameObject("Fill");
        fillGo.transform.SetParent(panel.transform, false);
        var fillRt = fillGo.AddComponent<RectTransform>();
        fillRt.anchorMin = new Vector2(0f, 0f);
        fillRt.anchorMax = new Vector2(1f, 0f);
        fillRt.pivot = new Vector2(0.5f, 0f);
        fillRt.anchoredPosition = new Vector2(0f, 6f);
        fillRt.sizeDelta = new Vector2(-16f, 8f);
        progressBarFill = fillGo.AddComponent<Image>();
        progressBarFill.color = new Color(0.9f, 0.7f, 0.2f, 0.9f);
        progressBarFill.type = Image.Type.Filled;
        progressBarFill.fillMethod = Image.FillMethod.Horizontal;
    }

    private static Text CreateText(Transform parent, string name, Vector2 pos, float height, Font font, int size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(-16f, height);
        var text = go.AddComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.color = Color.white;
        text.alignment = TextAnchor.UpperLeft;
        return text;
    }
}
