using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks and displays question history on the game-over screen via OnGUI.
/// Call <see cref="RecordQuestion"/> from QuestionBox to log each answered question.
/// </summary>
public class QuestionHistoryDisplay : MonoBehaviour
{
    private struct QuestionRecord
    {
        public string questionText;
        public int playerAnswer;
        public int correctAnswer;
        public bool wasCorrect;
    }

    private static readonly List<QuestionRecord> history = new List<QuestionRecord>();
    private Vector2 scrollPosition;

    /// <summary>
    /// Records a question result for the history display.
    /// </summary>
    public static void RecordQuestion(string text, int playerAnswer, int correctAnswer)
    {
        history.Add(new QuestionRecord
        {
            questionText = text ?? "",
            playerAnswer = playerAnswer,
            correctAnswer = correctAnswer,
            wasCorrect = playerAnswer == correctAnswer
        });
    }

    /// <summary>
    /// Clears all recorded question history. Call at game start.
    /// </summary>
    public static void ClearHistory()
    {
        history.Clear();
    }

    private void OnGUI()
    {
        if (!GameState.IsGameOver()) return;
        if (history.Count == 0) return;

        float panelWidth = 420f;
        float panelHeight = 300f;
        float panelX = Screen.width - panelWidth - 20f;
        float panelY = Screen.height * 0.3f;

        GUI.Box(new Rect(panelX, panelY, panelWidth, panelHeight), "");

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 22,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        titleStyle.normal.textColor = Color.white;

        GUI.Label(new Rect(panelX, panelY + 5, panelWidth, 30), "Question History", titleStyle);

        Rect scrollViewRect = new Rect(panelX + 10, panelY + 40, panelWidth - 20, panelHeight - 50);
        float contentHeight = history.Count * 30f;
        Rect contentRect = new Rect(0, 0, panelWidth - 40, contentHeight);

        scrollPosition = GUI.BeginScrollView(scrollViewRect, scrollPosition, contentRect);

        GUIStyle correctStyle = new GUIStyle(GUI.skin.label) { fontSize = 16 };
        correctStyle.normal.textColor = new Color(0.2f, 0.9f, 0.2f);

        GUIStyle incorrectStyle = new GUIStyle(GUI.skin.label) { fontSize = 16 };
        incorrectStyle.normal.textColor = new Color(0.9f, 0.2f, 0.2f);

        for (int i = 0; i < history.Count; i++)
        {
            var record = history[i];
            string mark = record.wasCorrect ? "\u2713" : "\u2717";
            string line = mark + "  " + record.questionText + " = " + record.playerAnswer;
            if (!record.wasCorrect)
                line += "  (correct: " + record.correctAnswer + ")";

            GUIStyle style = record.wasCorrect ? correctStyle : incorrectStyle;
            GUI.Label(new Rect(5, i * 30, contentRect.width, 28), line, style);
        }

        GUI.EndScrollView();
    }
}
