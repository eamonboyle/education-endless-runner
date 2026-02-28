using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MonoBehaviour for reward and unlock animations. Provides coroutine-based
/// coin showers, chest-open sequences, star bursts, and XP-bar fills.
/// Static convenience methods allow fire-and-forget calls from any script.
/// All spawned objects self-destruct after their animations complete.
/// </summary>
public class RewardAnimation : MonoBehaviour
{
    #region Singleton

    /// <summary>Global singleton instance.</summary>
    public static RewardAnimation Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #endregion

    [Header("Prefab References")]
    [SerializeField, Tooltip("Coin sprite prefab used by CoinShower.")]
    private GameObject coinPrefab;

    [SerializeField, Tooltip("Chest sprite prefab used by ChestOpen.")]
    private GameObject chestPrefab;

    [SerializeField, Tooltip("Star sprite prefab used by StarBurst.")]
    private GameObject starPrefab;

    [Header("XP Bar")]
    [SerializeField, Tooltip("Image component used for the XP fill bar.")]
    private Image xpBarFill;

    [Header("Settings")]
    [SerializeField, Tooltip("Canvas transform under which UI elements are spawned.")]
    private RectTransform spawnParent;

    [SerializeField] private int coinCount = 15;
    [SerializeField] private float coinDuration = 1.5f;
    [SerializeField] private int starCount = 12;
    [SerializeField] private float starDuration = 1.0f;

    /// <summary>
    /// Spawns a shower of falling coin sprites.
    /// Uses <c>coinPrefab</c> if assigned; otherwise creates runtime quads.
    /// </summary>
    public static void PlayCoinShower()
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.CoinShowerCoroutine());
        }
    }

    /// <summary>
    /// Plays a chest-open animation sequence.
    /// Uses <c>chestPrefab</c> if assigned; otherwise creates a runtime sprite.
    /// </summary>
    public static void PlayChestOpen()
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.ChestOpenCoroutine());
        }
    }

    /// <summary>
    /// Spawns a radial burst of star sprites.
    /// Uses <c>starPrefab</c> if assigned; otherwise creates runtime quads.
    /// </summary>
    public static void PlayStarBurst()
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.StarBurstCoroutine());
        }
    }

    /// <summary>
    /// Animates the XP bar from its current fill to <paramref name="targetFill"/>
    /// over <paramref name="duration"/> seconds.
    /// </summary>
    /// <param name="targetFill">Target fill amount (0–1).</param>
    /// <param name="duration">Animation duration in seconds.</param>
    public static void PlayXPBarFill(float targetFill, float duration = 0.6f)
    {
        if (Instance != null && Instance.xpBarFill != null)
        {
            Instance.StartCoroutine(Instance.XPBarFillCoroutine(targetFill, duration));
        }
    }

    #region Coroutines

    private IEnumerator CoinShowerCoroutine()
    {
        RectTransform parent = GetSpawnParent();

        for (int i = 0; i < coinCount; i++)
        {
            GameObject coin = CreateUIElement(coinPrefab, parent, new Color(1f, 0.84f, 0f));
            StartCoroutine(FallAndFade(coin, coinDuration));
            yield return new WaitForSeconds(coinDuration / coinCount);
        }
    }

    private IEnumerator ChestOpenCoroutine()
    {
        RectTransform parent = GetSpawnParent();
        GameObject chest = CreateUIElement(chestPrefab, parent, new Color(0.55f, 0.27f, 0.07f));

        if (chest == null) yield break;

        RectTransform rt = chest.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(100f, 100f);
        }

        float elapsed = 0f;
        float duration = 0.8f;
        Vector3 startScale = Vector3.one * 0.5f;
        Vector3 endScale = Vector3.one * 1.2f;

        chest.transform.localScale = startScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            chest.transform.localScale = Vector3.Lerp(startScale, endScale, eased);
            yield return null;
        }

        yield return new WaitForSeconds(0.4f);

        StartCoroutine(FadeAndDestroy(chest, 0.3f));
    }

    private IEnumerator StarBurstCoroutine()
    {
        RectTransform parent = GetSpawnParent();

        for (int i = 0; i < starCount; i++)
        {
            GameObject star = CreateUIElement(starPrefab, parent, Color.yellow);
            if (star == null) continue;

            float angle = (360f / starCount) * i;
            StartCoroutine(RadialMoveAndFade(star, angle, starDuration));
        }

        yield return null;
    }

    private IEnumerator XPBarFillCoroutine(float targetFill, float duration)
    {
        float startFill = xpBarFill.fillAmount;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            xpBarFill.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            yield return null;
        }

        xpBarFill.fillAmount = targetFill;
    }

    private IEnumerator FallAndFade(GameObject obj, float duration)
    {
        if (obj == null) yield break;

        RectTransform rt = obj.GetComponent<RectTransform>();
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();

        float startX = Random.Range(-200f, 200f);
        float startY = 300f;
        if (rt != null) rt.anchoredPosition = new Vector2(startX, startY);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (rt != null)
            {
                rt.anchoredPosition = new Vector2(
                    startX + Mathf.Sin(t * Mathf.PI * 2f) * 30f,
                    Mathf.Lerp(startY, -400f, t)
                );
            }

            cg.alpha = 1f - t;
            yield return null;
        }

        Destroy(obj);
    }

    private IEnumerator RadialMoveAndFade(GameObject obj, float angleDeg, float duration)
    {
        if (obj == null) yield break;

        RectTransform rt = obj.GetComponent<RectTransform>();
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();

        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        float maxDist = 200f;

        if (rt != null) rt.anchoredPosition = Vector2.zero;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (rt != null)
            {
                rt.anchoredPosition = direction * maxDist * t;
            }
            rt.localScale = Vector3.one * (1f - t * 0.5f);
            cg.alpha = 1f - t;
            yield return null;
        }

        Destroy(obj);
    }

    private IEnumerator FadeAndDestroy(GameObject obj, float duration)
    {
        if (obj == null) yield break;

        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = 1f - Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        Destroy(obj);
    }

    #endregion

    #region Helpers

    private RectTransform GetSpawnParent()
    {
        if (spawnParent != null) return spawnParent;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null) return canvas.GetComponent<RectTransform>();

        return GetComponent<RectTransform>();
    }

    private static GameObject CreateUIElement(GameObject prefab, RectTransform parent, Color fallbackColor)
    {
        if (prefab != null && parent != null)
        {
            GameObject instance = Instantiate(prefab, parent);
            return instance;
        }

        if (parent == null) return null;

        GameObject go = new GameObject("RewardElement");
        go.transform.SetParent(parent, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(30f, 30f);

        Image img = go.AddComponent<Image>();
        img.color = fallbackColor;

        return go;
    }

    #endregion
}
