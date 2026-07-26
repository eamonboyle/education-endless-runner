using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows active power-up icons and remaining timers.
/// Builds its own HUD slot row at runtime when no slots are assigned in the Inspector.
/// </summary>
public class PowerUpDisplay : MonoBehaviour
{
    [System.Serializable]
    public class PowerUpSlot
    {
        public Image iconImage;
        public Text timerText;
        public GameObject slotRoot;
        public PowerUpType boundType;
    }

    [SerializeField] private List<PowerUpSlot> slots = new List<PowerUpSlot>();
    [SerializeField] private float flashThreshold = 3f;
    [SerializeField] private float flashInterval = 0.25f;

    private readonly Dictionary<PowerUpType, Coroutine> activeTimers = new Dictionary<PowerUpType, Coroutine>();
    private readonly Dictionary<PowerUpType, Sprite> iconCache = new Dictionary<PowerUpType, Sprite>();
    private bool subscribed;

    private void Start()
    {
        if (slots == null || slots.Count == 0)
            BuildRuntimeSlots();

        HideAllSlots();
        TrySubscribe();
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void TrySubscribe()
    {
        if (subscribed) return;
        var system = PowerUpSystem.Instance;
        if (system == null) return;

        system.OnPowerUpActivated += HandleActivated;
        system.OnPowerUpExpired += HandleExpired;
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!subscribed) return;
        var system = PowerUpSystem.Instance;
        if (system != null)
        {
            system.OnPowerUpActivated -= HandleActivated;
            system.OnPowerUpExpired -= HandleExpired;
        }
        subscribed = false;
    }

    private void HandleActivated(PowerUpType type)
    {
        PowerUpSlot slot = FindOrAssignSlot(type);
        if (slot == null) return;

        if (activeTimers.TryGetValue(type, out Coroutine running) && running != null)
            StopCoroutine(running);

        slot.boundType = type;
        slot.slotRoot.SetActive(true);

        if (slot.iconImage != null)
        {
            slot.iconImage.sprite = GetIcon(type);
            slot.iconImage.color = Color.white;
        }

        float duration = PowerUpSystem.Instance != null
            ? PowerUpSystem.Instance.GetRemainingDuration(type)
            : 10f;

        if (float.IsInfinity(duration) || duration >= float.MaxValue * 0.5f)
        {
            if (slot.timerText != null)
                slot.timerText.text = "SH";
            activeTimers[type] = null;
            return;
        }

        activeTimers[type] = StartCoroutine(CountdownCoroutine(type, duration));
    }

    private void HandleExpired(PowerUpType type)
    {
        DeactivatePowerUp(type);
    }

    public void DeactivatePowerUp(PowerUpType type)
    {
        if (activeTimers.TryGetValue(type, out Coroutine running) && running != null)
            StopCoroutine(running);
        activeTimers.Remove(type);

        PowerUpSlot slot = FindSlot(type);
        if (slot != null && slot.slotRoot != null)
        {
            slot.slotRoot.SetActive(false);
            slot.boundType = (PowerUpType)(-1);
        }
    }

    private IEnumerator CountdownCoroutine(PowerUpType type, float duration)
    {
        PowerUpSlot slot = FindSlot(type);
        float remaining = duration;

        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;

            if (slot != null && slot.timerText != null)
                slot.timerText.text = Mathf.CeilToInt(Mathf.Max(0f, remaining)).ToString();

            if (remaining <= flashThreshold && slot != null && slot.iconImage != null)
            {
                float flash = Mathf.PingPong(Time.time / flashInterval, 1f);
                Color c = slot.iconImage.color;
                c.a = Mathf.Lerp(0.3f, 1f, flash);
                slot.iconImage.color = c;
            }

            yield return null;
        }

        DeactivatePowerUp(type);
    }

    private PowerUpSlot FindSlot(PowerUpType type)
    {
        if (slots == null) return null;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] != null && slots[i].boundType == type)
                return slots[i];
        }
        return null;
    }

    private PowerUpSlot FindOrAssignSlot(PowerUpType type)
    {
        PowerUpSlot existing = FindSlot(type);
        if (existing != null) return existing;

        if (slots == null) return null;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null || slots[i].slotRoot == null) continue;
            if (!slots[i].slotRoot.activeSelf || slots[i].boundType < 0)
            {
                slots[i].boundType = type;
                return slots[i];
            }
        }
        return slots.Count > 0 ? slots[0] : null;
    }

    private Sprite GetIcon(PowerUpType type)
    {
        if (iconCache.TryGetValue(type, out Sprite cached) && cached != null)
            return cached;
        Sprite created = PowerUpFactory.CreateIconSprite(type);
        iconCache[type] = created;
        return created;
    }

    private void HideAllSlots()
    {
        if (slots == null) return;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] != null && slots[i].slotRoot != null)
            {
                slots[i].slotRoot.SetActive(false);
                slots[i].boundType = (PowerUpType)(-1);
            }
        }
    }

    private void BuildRuntimeSlots()
    {
        slots = new List<PowerUpSlot>();

        var canvasGo = new GameObject("PowerUpDisplayCanvas");
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        var row = new GameObject("Slots");
        row.transform.SetParent(canvasGo.transform, false);
        var rowRt = row.AddComponent<RectTransform>();
        rowRt.anchorMin = new Vector2(1f, 1f);
        rowRt.anchorMax = new Vector2(1f, 1f);
        rowRt.pivot = new Vector2(1f, 1f);
        rowRt.anchoredPosition = new Vector2(-20f, -120f);
        rowRt.sizeDelta = new Vector2(200f, 64f);
        var layout = row.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 8f;
        layout.childAlignment = TextAnchor.MiddleRight;
        layout.childControlWidth = false;
        layout.childControlHeight = false;

        for (int i = 0; i < 3; i++)
        {
            var slotRoot = new GameObject("Slot_" + i);
            slotRoot.transform.SetParent(row.transform, false);
            var slotRt = slotRoot.AddComponent<RectTransform>();
            slotRt.sizeDelta = new Vector2(56f, 56f);

            var iconGo = new GameObject("Icon");
            iconGo.transform.SetParent(slotRoot.transform, false);
            var iconRt = iconGo.AddComponent<RectTransform>();
            iconRt.anchorMin = Vector2.zero;
            iconRt.anchorMax = Vector2.one;
            iconRt.offsetMin = Vector2.zero;
            iconRt.offsetMax = Vector2.zero;
            var icon = iconGo.AddComponent<Image>();

            var timerGo = new GameObject("Timer");
            timerGo.transform.SetParent(slotRoot.transform, false);
            var timerRt = timerGo.AddComponent<RectTransform>();
            timerRt.anchorMin = new Vector2(0f, 0f);
            timerRt.anchorMax = new Vector2(1f, 0.4f);
            timerRt.offsetMin = Vector2.zero;
            timerRt.offsetMax = Vector2.zero;
            var timer = timerGo.AddComponent<Text>();
            timer.alignment = TextAnchor.MiddleCenter;
            timer.fontSize = 14;
            timer.color = Color.white;
            timer.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (timer.font == null)
                timer.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            slots.Add(new PowerUpSlot
            {
                slotRoot = slotRoot,
                iconImage = icon,
                timerText = timer,
                boundType = (PowerUpType)(-1)
            });
        }
    }
}
