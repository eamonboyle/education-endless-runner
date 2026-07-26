using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Queued achievement unlock toast. Builds a simple overlay at runtime when
/// Inspector references are not assigned (bootstrapped via GameBootstrap).
/// </summary>
public class AchievementPopup : MonoBehaviour
{
    public static AchievementPopup Instance { get; private set; }

    [SerializeField] private RectTransform popupPanel;
    [SerializeField] private Text achievementNameText;
    [SerializeField] private Text achievementDescriptionText;

    [SerializeField] private float slideDistance = 200f;
    [SerializeField] private float slideDuration = 0.4f;
    [SerializeField] private float displayDuration = 3f;

    private readonly Queue<AchievementData> achievementQueue = new Queue<AchievementData>();
    private bool isShowing;

    public struct AchievementData
    {
        public string name;
        public string description;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (popupPanel == null)
            BuildRuntimeUI();

        if (popupPanel != null)
            popupPanel.gameObject.SetActive(false);
        isShowing = false;
    }

    public void ShowAchievement(string name, string description)
    {
        achievementQueue.Enqueue(new AchievementData { name = name, description = description });
        if (!isShowing)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        isShowing = true;
        while (achievementQueue.Count > 0)
        {
            AchievementData data = achievementQueue.Dequeue();
            yield return StartCoroutine(ShowPopup(data));
        }
        isShowing = false;
    }

    private IEnumerator ShowPopup(AchievementData data)
    {
        if (popupPanel == null) yield break;

        if (achievementNameText != null)
            achievementNameText.text = data.name;
        if (achievementDescriptionText != null)
            achievementDescriptionText.text = data.description;

        Vector2 hiddenPos = new Vector2(popupPanel.anchoredPosition.x, slideDistance);
        Vector2 visiblePos = new Vector2(popupPanel.anchoredPosition.x, 0f);

        popupPanel.anchoredPosition = hiddenPos;
        popupPanel.gameObject.SetActive(true);

        yield return StartCoroutine(SlideCoroutine(hiddenPos, visiblePos, slideDuration));
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(SlideCoroutine(visiblePos, hiddenPos, slideDuration));

        popupPanel.gameObject.SetActive(false);
    }

    private IEnumerator SlideCoroutine(Vector2 from, Vector2 to, float duration)
    {
        if (popupPanel == null) yield break;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            popupPanel.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }
        popupPanel.anchoredPosition = to;
    }

    private void BuildRuntimeUI()
    {
        var canvasGo = new GameObject("AchievementPopupCanvas");
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        var panelGo = new GameObject("PopupPanel");
        panelGo.transform.SetParent(canvasGo.transform, false);
        popupPanel = panelGo.AddComponent<RectTransform>();
        popupPanel.anchorMin = new Vector2(0.5f, 1f);
        popupPanel.anchorMax = new Vector2(0.5f, 1f);
        popupPanel.pivot = new Vector2(0.5f, 1f);
        popupPanel.anchoredPosition = new Vector2(0f, -20f);
        popupPanel.sizeDelta = new Vector2(420f, 90f);
        var bg = panelGo.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.15f, 0.92f);

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        var nameGo = new GameObject("Name");
        nameGo.transform.SetParent(panelGo.transform, false);
        var nameRt = nameGo.AddComponent<RectTransform>();
        nameRt.anchorMin = new Vector2(0f, 0.45f);
        nameRt.anchorMax = new Vector2(1f, 1f);
        nameRt.offsetMin = new Vector2(12f, 0f);
        nameRt.offsetMax = new Vector2(-12f, -8f);
        achievementNameText = nameGo.AddComponent<Text>();
        achievementNameText.font = font;
        achievementNameText.fontSize = 22;
        achievementNameText.fontStyle = FontStyle.Bold;
        achievementNameText.alignment = TextAnchor.LowerLeft;
        achievementNameText.color = Color.white;

        var descGo = new GameObject("Description");
        descGo.transform.SetParent(panelGo.transform, false);
        var descRt = descGo.AddComponent<RectTransform>();
        descRt.anchorMin = new Vector2(0f, 0f);
        descRt.anchorMax = new Vector2(1f, 0.5f);
        descRt.offsetMin = new Vector2(12f, 8f);
        descRt.offsetMax = new Vector2(-12f, 0f);
        achievementDescriptionText = descGo.AddComponent<Text>();
        achievementDescriptionText.font = font;
        achievementDescriptionText.fontSize = 16;
        achievementDescriptionText.alignment = TextAnchor.UpperLeft;
        achievementDescriptionText.color = new Color(0.85f, 0.85f, 0.9f);
    }
}
