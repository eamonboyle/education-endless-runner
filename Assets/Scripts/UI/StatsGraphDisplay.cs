using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MathRunner.Data;

/// <summary>
/// MonoBehaviour that renders simple bar-chart and trend-line graphs for
/// player statistics. Uses UI <see cref="Image"/> components to visualise
/// per-day scores and accuracy over configurable time windows (7 or 30 days).
/// </summary>
public class StatsGraphDisplay : MonoBehaviour
{
    [Header("Graph Container")]
    [SerializeField, Tooltip("Parent RectTransform that holds the generated bars.")]
    private RectTransform graphContainer;

    [SerializeField, Tooltip("Prefab for a single bar in the chart. Must have an Image component.")]
    private GameObject barPrefab;

    [SerializeField, Tooltip("Prefab for a small dot used in the accuracy trend line.")]
    private GameObject dotPrefab;

    [Header("Labels")]
    [SerializeField, Tooltip("TextMeshProUGUI for the graph title.")]
    private TextMeshProUGUI titleText;

    [Header("Settings")]
    [SerializeField, Tooltip("Number of days to display (7 or 30).")]
    private int daysToShow = 7;

    [SerializeField, Tooltip("Colour for score bars.")]
    private Color barColor = new Color(0.2f, 0.6f, 1f, 0.9f);

    [SerializeField, Tooltip("Colour for the accuracy trend dots.")]
    private Color trendColor = new Color(1f, 0.4f, 0.2f, 1f);

    [SerializeField, Tooltip("Height of the tallest bar in local units.")]
    private float maxBarHeight = 200f;

    private readonly List<GameObject> spawnedElements = new List<GameObject>();

    private void OnEnable()
    {
        Refresh();
    }

    /// <summary>
    /// Rebuilds the graph by reading per-day score and accuracy data from
    /// <see cref="PlayerPrefs"/>. Existing bars and dots are destroyed
    /// before new ones are created.
    /// </summary>
    public void Refresh()
    {
        ClearGraph();

        if (graphContainer == null) return;

        int[] scores = GetDailyScores(daysToShow);
        float[] accuracies = GetDailyAccuracies(daysToShow);

        int maxScore = 1;
        foreach (int s in scores)
        {
            if (s > maxScore) maxScore = s;
        }

        float containerWidth = graphContainer.rect.width;
        float barWidth = containerWidth / daysToShow;

        for (int i = 0; i < daysToShow; i++)
        {
            float normalisedScore = (float)scores[i] / maxScore;
            float barHeight = normalisedScore * maxBarHeight;
            float xPos = i * barWidth + barWidth * 0.5f;

            CreateBar(xPos, barHeight, barWidth * 0.7f);

            if (accuracies[i] >= 0f)
            {
                float dotY = accuracies[i] * maxBarHeight;
                CreateDot(xPos, dotY);
            }
        }

        if (titleText != null)
        {
            titleText.text = "Last " + daysToShow + " Days";
        }
    }

    /// <summary>
    /// Sets the number of days displayed and refreshes the graph.
    /// </summary>
    /// <param name="days">Number of days (typically 7 or 30).</param>
    public void SetDaysToShow(int days)
    {
        daysToShow = Mathf.Clamp(days, 1, 90);
        Refresh();
    }

    #region Graph Building

    private void CreateBar(float xPos, float height, float width)
    {
        GameObject bar = barPrefab != null
            ? Instantiate(barPrefab, graphContainer)
            : CreateFallbackRect(graphContainer);

        RectTransform rt = bar.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(0f, 0f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(xPos, 0f);
            rt.sizeDelta = new Vector2(width, Mathf.Max(height, 2f));
        }

        Image img = bar.GetComponent<Image>();
        if (img != null)
        {
            img.color = barColor;
        }

        spawnedElements.Add(bar);
    }

    private void CreateDot(float xPos, float yPos)
    {
        GameObject dot = dotPrefab != null
            ? Instantiate(dotPrefab, graphContainer)
            : CreateFallbackRect(graphContainer);

        RectTransform rt = dot.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(0f, 0f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(xPos, yPos);
            rt.sizeDelta = new Vector2(8f, 8f);
        }

        Image img = dot.GetComponent<Image>();
        if (img != null)
        {
            img.color = trendColor;
        }

        spawnedElements.Add(dot);
    }

    private static GameObject CreateFallbackRect(RectTransform parent)
    {
        GameObject go = new GameObject("GraphElement");
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>();
        return go;
    }

    private void ClearGraph()
    {
        foreach (GameObject obj in spawnedElements)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedElements.Clear();
    }

    #endregion

    #region Data Retrieval

    private static int[] GetDailyScores(int days)
    {
        int[] scores = new int[days];

        for (int i = 0; i < days; i++)
        {
            string dateKey = DateTime.UtcNow.AddDays(-(days - 1 - i)).ToString("yyyy-MM-dd");
            scores[i] = PlayerPrefs.GetInt("dailyScore_" + dateKey, 0);
        }

        return scores;
    }

    private static float[] GetDailyAccuracies(int days)
    {
        float[] accuracies = new float[days];

        for (int i = 0; i < days; i++)
        {
            string dateKey = DateTime.UtcNow.AddDays(-(days - 1 - i)).ToString("yyyy-MM-dd");
            int total = PlayerPrefs.GetInt("dailyQuestions_" + dateKey, 0);
            int correct = PlayerPrefs.GetInt("dailyCorrect_" + dateKey, 0);

            accuracies[i] = total > 0 ? (float)correct / total : -1f;
        }

        return accuracies;
    }

    #endregion
}
