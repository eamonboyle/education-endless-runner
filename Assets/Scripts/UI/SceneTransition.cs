using System.Collections;
using MathRunner.UI.Toolkit;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Full-screen fade for scene transitions via Toolkit TransitionScreen.
/// Legacy CanvasGroup path retained as fallback.
/// </summary>
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;

    [SerializeField] private UnityEngine.CanvasGroup canvasGroup;
    [SerializeField] private float defaultFadeDuration = 0.35f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void TransitionTo(System.Action onMidpoint)
    {
        StartCoroutine(TransitionCoroutine(onMidpoint, defaultFadeDuration));
    }

    private IEnumerator TransitionCoroutine(System.Action onMidpoint, float duration)
    {
        if (UIRouter.Instance != null)
        {
            UIRouter.Instance.FadeOut(duration);
            yield return new WaitForSecondsRealtime(duration);
            onMidpoint?.Invoke();
            UIRouter.Instance.FadeIn(duration);
            yield break;
        }

        yield return FadeOutCoroutine(duration);
        onMidpoint?.Invoke();
        yield return FadeInCoroutine(duration);
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        if (canvasGroup == null) yield break;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / duration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        if (canvasGroup == null) yield break;
        float t = 0f;
        float start = canvasGroup.alpha;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, 0f, Mathf.Clamp01(t / duration));
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
