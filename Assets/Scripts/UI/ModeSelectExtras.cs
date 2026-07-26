using MathRunner.Core;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime ModeChoice extras: Time Attack, Campaign, and Difficulty pickers.
/// Spawned by <see cref="ProgressionUIBootstrap"/> when the ModeChoice scene loads.
/// </summary>
public class ModeSelectExtras : MonoBehaviour
{
    private void Start()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        var canvasGo = new GameObject("ModeSelectExtrasCanvas");
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 60;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        var panel = CreatePanel(canvasGo.transform, new Vector2(0.5f, 0f), new Vector2(0f, 24f), new Vector2(520f, 120f));

        CreateButton(panel, "Time Attack", new Vector2(-170f, 20f), font, () =>
        {
            TimeAttackMode.SetTimeAttack(true);
            PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 0);
            PrefsFlush.Flush();
            FlashStatus("Time Attack ON — pick a mode above");
        });

        CreateButton(panel, "Campaign", new Vector2(0f, 20f), font, () =>
        {
            TimeAttackMode.SetTimeAttack(false);
            int level = CampaignManager.GetCurrentLevel();
            PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 1);
            PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_LEVEL, level);
            PrefsFlush.Flush();
            var config = CampaignManager.GetLevelConfig(level);
            GameState.SetQuestionType(config.MathMode.ToPlayerPrefsString());
            FlashStatus("Campaign L" + level + " — tap Play / Main Menu");
        });

        CreateButton(panel, "Classic", new Vector2(170f, 20f), font, () =>
        {
            TimeAttackMode.SetTimeAttack(false);
            PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 0);
            PrefsFlush.Flush();
            FlashStatus("Classic mode");
        });

        CreateButton(panel, "Easy", new Vector2(-170f, -30f), font, () =>
        {
            DifficultyPresets.SetDifficulty(DifficultyLevel.Easy);
            FlashStatus("Difficulty: Easy");
        });
        CreateButton(panel, "Normal", new Vector2(0f, -30f), font, () =>
        {
            DifficultyPresets.SetDifficulty(DifficultyLevel.Medium);
            FlashStatus("Difficulty: Normal");
        });
        CreateButton(panel, "Hard", new Vector2(170f, -30f), font, () =>
        {
            DifficultyPresets.SetDifficulty(DifficultyLevel.Hard);
            FlashStatus("Difficulty: Hard");
        });
    }

    private Text statusText;

    private void FlashStatus(string message)
    {
        if (statusText == null) return;
        statusText.text = message;
    }

    private RectTransform CreatePanel(Transform parent, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        var go = new GameObject("ExtrasPanel");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        go.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.65f);

        var statusGo = new GameObject("Status");
        statusGo.transform.SetParent(go.transform, false);
        var srt = statusGo.AddComponent<RectTransform>();
        srt.anchorMin = new Vector2(0f, 1f);
        srt.anchorMax = new Vector2(1f, 1f);
        srt.pivot = new Vector2(0.5f, 1f);
        srt.anchoredPosition = new Vector2(0f, 8f);
        srt.sizeDelta = new Vector2(-12f, 22f);
        statusText = statusGo.AddComponent<Text>();
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (statusText.font == null)
            statusText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        statusText.fontSize = 14;
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.color = Color.white;
        statusText.text = "Extras: Time Attack / Campaign / Difficulty";
        return rt;
    }

    private static void CreateButton(Transform parent, string label, Vector2 pos, Font font, UnityEngine.Events.UnityAction onClick)
    {
        var go = new GameObject(label + "Btn");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(150f, 36f);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.45f, 0.75f, 0.95f);
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(onClick);

        var textGo = new GameObject("Label");
        textGo.transform.SetParent(go.transform, false);
        var trt = textGo.AddComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;
        var text = textGo.AddComponent<Text>();
        text.font = font;
        text.fontSize = 16;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.text = label;
    }
}
