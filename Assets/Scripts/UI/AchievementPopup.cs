using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementPopup : MonoBehaviour
{
    [SerializeField] private RectTransform popupPanel;
    [SerializeField] private TextMeshProUGUI achievementNameText;
    [SerializeField] private TextMeshProUGUI achievementDescriptionText;

    [SerializeField] private float slideDistance = 200f;
    [SerializeField] private float slideDuration = 0.4f;
    [SerializeField] private float displayDuration = 3f;

    private Queue<AchievementData> achievementQueue = new Queue<AchievementData>();
    private bool isShowing;

    public struct AchievementData
    {
        public string name;
        public string description;
    }

    private void Start()
    {
        if (popupPanel != null)
        {
            popupPanel.gameObject.SetActive(false);
        }
        isShowing = false;
    }

    public void ShowAchievement(string name, string description)
    {
        AchievementData data = new AchievementData
        {
            name = name,
            description = description
        };

        achievementQueue.Enqueue(data);

        if (!isShowing)
        {
            StartCoroutine(ProcessQueue());
        }
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
        {
            achievementNameText.text = data.name;
        }

        if (achievementDescriptionText != null)
        {
            achievementDescriptionText.text = data.description;
        }

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
}
