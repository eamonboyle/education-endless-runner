using MathRunner.UI.Screens;
using UnityEngine;

/// <summary>
/// Bridge: records answered questions for the Toolkit session summary screen.
/// </summary>
public class QuestionHistoryDisplay : MonoBehaviour
{
    public static void RecordQuestion(string text, int playerAnswer, int correctAnswer)
    {
        bool correct = playerAnswer == correctAnswer;
        string mark = correct ? "\u2713" : "\u2717";
        SessionSummaryScreen.RecordQuestion(
            $"{mark} {text}  ({playerAnswer}/{correctAnswer})");
    }

    public static void ClearHistory()
    {
        SessionSummaryScreen.ClearHistory();
    }
}
