using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpDisplay : MonoBehaviour
{
    [System.Serializable]
    public class PowerUpSlot
    {
        public Image iconImage;
        public TextMeshProUGUI timerText;
        public GameObject slotRoot;
    }

    [SerializeField] private List<PowerUpSlot> slots = new List<PowerUpSlot>();
    [SerializeField] private float flashThreshold = 3f;
    [SerializeField] private float flashInterval = 0.25f;

    private Dictionary<int, Coroutine> activeTimers = new Dictionary<int, Coroutine>();

    private void Start()
    {
        HideAllSlots();
    }

    public void ActivatePowerUp(int slotIndex, Sprite icon, float duration)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;

        PowerUpSlot slot = slots[slotIndex];
        if (slot == null || slot.slotRoot == null) return;

        if (activeTimers.ContainsKey(slotIndex))
        {
            StopCoroutine(activeTimers[slotIndex]);
            activeTimers.Remove(slotIndex);
        }

        slot.slotRoot.SetActive(true);

        if (slot.iconImage != null && icon != null)
        {
            slot.iconImage.sprite = icon;
            slot.iconImage.color = Color.white;
        }

        activeTimers[slotIndex] = StartCoroutine(CountdownCoroutine(slotIndex, duration));
    }

    public void DeactivatePowerUp(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;

        if (activeTimers.ContainsKey(slotIndex))
        {
            StopCoroutine(activeTimers[slotIndex]);
            activeTimers.Remove(slotIndex);
        }

        PowerUpSlot slot = slots[slotIndex];
        if (slot != null && slot.slotRoot != null)
        {
            slot.slotRoot.SetActive(false);
        }
    }

    private IEnumerator CountdownCoroutine(int slotIndex, float duration)
    {
        PowerUpSlot slot = slots[slotIndex];
        float remaining = duration;

        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;

            if (slot.timerText != null)
            {
                slot.timerText.text = Mathf.CeilToInt(remaining).ToString();
            }

            if (remaining <= flashThreshold && slot.iconImage != null)
            {
                float flash = Mathf.PingPong(Time.time / flashInterval, 1f);
                Color c = slot.iconImage.color;
                c.a = Mathf.Lerp(0.3f, 1f, flash);
                slot.iconImage.color = c;
            }

            yield return null;
        }

        DeactivatePowerUp(slotIndex);
    }

    private void HideAllSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] != null && slots[i].slotRoot != null)
            {
                slots[i].slotRoot.SetActive(false);
            }
        }
    }
}
