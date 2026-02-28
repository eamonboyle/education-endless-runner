using System.Collections;
using UnityEngine;
using TMPro;

public class AnimatedText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText;

    private CanvasGroup canvasGroup;
    private Coroutine activeCoroutine;

    private void Awake()
    {
        if (targetText == null)
        {
            targetText = GetComponent<TextMeshProUGUI>();
        }

        canvasGroup = GetComponent<CanvasGroup>();
    }

    public Coroutine ScalePunch(float scale, float duration)
    {
        StopActiveCoroutine();
        activeCoroutine = StartCoroutine(ScalePunchCoroutine(scale, duration));
        return activeCoroutine;
    }

    public Coroutine FadeIn(float duration)
    {
        StopActiveCoroutine();
        activeCoroutine = StartCoroutine(FadeCoroutine(0f, 1f, duration));
        return activeCoroutine;
    }

    public Coroutine FadeOut(float duration)
    {
        StopActiveCoroutine();
        activeCoroutine = StartCoroutine(FadeCoroutine(1f, 0f, duration));
        return activeCoroutine;
    }

    public Coroutine TypewriterEffect(string text, float charDelay)
    {
        StopActiveCoroutine();
        activeCoroutine = StartCoroutine(TypewriterCoroutine(text, charDelay));
        return activeCoroutine;
    }

    public Coroutine Shake(float intensity, float duration)
    {
        StopActiveCoroutine();
        activeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
        return activeCoroutine;
    }

    private void StopActiveCoroutine()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }
    }

    private IEnumerator ScalePunchCoroutine(float targetScale, float duration)
    {
        Transform t = transform;
        Vector3 originalScale = Vector3.one;
        Vector3 punchedScale = originalScale * targetScale;
        float halfDuration = duration * 0.5f;

        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / halfDuration;
            t.localScale = Vector3.Lerp(originalScale, punchedScale, progress);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / halfDuration;
            t.localScale = Vector3.Lerp(punchedScale, originalScale, progress);
            yield return null;
        }

        t.localScale = originalScale;
        activeCoroutine = null;
    }

    private IEnumerator FadeCoroutine(float from, float to, float duration)
    {
        if (canvasGroup == null && targetText == null)
        {
            activeCoroutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }
            else if (targetText != null)
            {
                Color c = targetText.color;
                c.a = alpha;
                targetText.color = c;
            }

            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = to;
        }
        else if (targetText != null)
        {
            Color c = targetText.color;
            c.a = to;
            targetText.color = c;
        }

        activeCoroutine = null;
    }

    private IEnumerator TypewriterCoroutine(string text, float charDelay)
    {
        if (targetText == null)
        {
            activeCoroutine = null;
            yield break;
        }

        targetText.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            targetText.text += text[i];
            yield return new WaitForSeconds(charDelay);
        }

        activeCoroutine = null;
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        Transform t = transform;
        Vector3 originalPosition = t.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float dampening = 1f - (elapsed / duration);
            float offsetX = Random.Range(-intensity, intensity) * dampening;
            float offsetY = Random.Range(-intensity, intensity) * dampening;
            t.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);
            yield return null;
        }

        t.localPosition = originalPosition;
        activeCoroutine = null;
    }
}
