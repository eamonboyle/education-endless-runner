using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI streakCountText;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private GameObject comboContainer;

    [SerializeField] private float punchScale = 1.5f;
    [SerializeField] private float punchDuration = 0.3f;

    private int currentStreak;
    private Coroutine punchCoroutine;

    private void Start()
    {
        currentStreak = 0;
        Hide();
    }

    public void UpdateStreak(int streak)
    {
        int previousStreak = currentStreak;
        currentStreak = streak;

        if (currentStreak <= 0)
        {
            Hide();
            return;
        }

        Show();

        if (streakCountText != null)
        {
            streakCountText.text = currentStreak.ToString();
        }

        int multiplier = GetMultiplier(currentStreak);
        if (multiplierText != null)
        {
            multiplierText.text = "x" + multiplier + "!";
            multiplierText.color = GetMultiplierColor(multiplier);
        }

        if (currentStreak > previousStreak)
        {
            PlayPunchAnimation();
        }
    }

    private int GetMultiplier(int streak)
    {
        if (streak >= 20) return 5;
        if (streak >= 15) return 4;
        if (streak >= 10) return 3;
        if (streak >= 5) return 2;
        return 1;
    }

    private Color GetMultiplierColor(int multiplier)
    {
        switch (multiplier)
        {
            case 2: return Color.green;
            case 3: return Color.blue;
            case 4: return new Color(0.5f, 0f, 0.5f); // purple
            case 5: return new Color(1f, 0.84f, 0f);   // gold
            default: return Color.white;
        }
    }

    private void PlayPunchAnimation()
    {
        if (punchCoroutine != null)
        {
            StopCoroutine(punchCoroutine);
        }
        punchCoroutine = StartCoroutine(ScalePunchCoroutine());
    }

    private IEnumerator ScalePunchCoroutine()
    {
        Transform target = (streakCountText != null) ? streakCountText.transform : transform;
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = originalScale * punchScale;

        float halfDuration = punchDuration * 0.5f;
        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            target.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            target.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        target.localScale = originalScale;
        punchCoroutine = null;
    }

    private void Show()
    {
        if (comboContainer != null)
        {
            comboContainer.SetActive(true);
        }
    }

    private void Hide()
    {
        if (comboContainer != null)
        {
            comboContainer.SetActive(false);
        }
    }
}
