using UnityEngine;

/// <summary>
/// Displays an end-of-session summary overlay for a set duration.
/// Call <see cref="ShowSummary"/> from game-over logic to populate and display.
/// </summary>
public class SessionSummary : MonoBehaviour
{
    private static bool isShowing;
    private static float showTimer;
    private static int summaryScore;
    private static int summaryQuestions;
    private static float summaryAccuracy;
    private static int summaryXP;

    [SerializeField]
    private float displayDuration = 5f;

    /// <summary>
    /// Triggers the summary overlay with the provided session statistics.
    /// </summary>
    public static void ShowSummary(int score, int questions, float accuracy, int xp)
    {
        summaryScore = score;
        summaryQuestions = questions;
        summaryAccuracy = accuracy;
        summaryXP = xp;
        showTimer = 5f;
        isShowing = true;
    }

    private void Update()
    {
        if (!isShowing) return;

        showTimer -= Time.unscaledDeltaTime;
        if (showTimer <= 0f)
        {
            isShowing = false;
        }
    }

    private void OnGUI()
    {
        if (!isShowing) return;

        float boxWidth = 400f;
        float boxHeight = 260f;
        Rect boxRect = new Rect(
            (Screen.width - boxWidth) / 2f,
            (Screen.height - boxHeight) / 2f,
            boxWidth,
            boxHeight
        );

        GUI.Box(boxRect, "");

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 28,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        titleStyle.normal.textColor = new Color(1f, 0.84f, 0f);

        GUIStyle statStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleLeft
        };
        statStyle.normal.textColor = Color.white;

        float y = boxRect.y + 15f;
        float padding = 30f;

        GUI.Label(new Rect(boxRect.x, y, boxWidth, 40), "SESSION SUMMARY", titleStyle);
        y += 50f;

        GUI.Label(new Rect(boxRect.x + padding, y, boxWidth - padding * 2, 30),
            "Score: " + summaryScore, statStyle);
        y += 35f;

        GUI.Label(new Rect(boxRect.x + padding, y, boxWidth - padding * 2, 30),
            "Questions: " + summaryQuestions, statStyle);
        y += 35f;

        GUI.Label(new Rect(boxRect.x + padding, y, boxWidth - padding * 2, 30),
            "Accuracy: " + summaryAccuracy.ToString("F1") + "%", statStyle);
        y += 35f;

        GUI.Label(new Rect(boxRect.x + padding, y, boxWidth - padding * 2, 30),
            "XP Earned: +" + summaryXP, statStyle);
    }
}
