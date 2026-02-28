using System;
using System.Collections;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float defaultFadeDuration = 0.5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public Coroutine FadeIn(float duration)
    {
        return StartCoroutine(FadeInCoroutine(duration));
    }

    public Coroutine FadeOut(float duration)
    {
        return StartCoroutine(FadeOutCoroutine(duration));
    }

    public IEnumerator FadeInCoroutine(float duration)
    {
        if (canvasGroup == null) yield break;

        canvasGroup.blocksRaycasts = true;
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeOutCoroutine(float duration)
    {
        if (canvasGroup == null) yield break;

        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void TransitionTo(Action loadAction)
    {
        StartCoroutine(TransitionCoroutine(loadAction, defaultFadeDuration));
    }

    public void TransitionTo(Action loadAction, float duration)
    {
        StartCoroutine(TransitionCoroutine(loadAction, duration));
    }

    private IEnumerator TransitionCoroutine(Action loadAction, float duration)
    {
        yield return StartCoroutine(FadeInCoroutine(duration));

        if (loadAction != null)
        {
            loadAction.Invoke();
        }

        yield return new WaitForSecondsRealtime(0.1f);

        yield return StartCoroutine(FadeOutCoroutine(duration));
    }
}
