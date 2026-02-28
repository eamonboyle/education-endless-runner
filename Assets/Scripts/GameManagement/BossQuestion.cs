using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a two-step "boss" math question that appears every Nth question.
/// Generates compound expressions such as <c>(3 + 5) x 2</c> and awards
/// a higher score reward (5x normal). Follows the same data shape as
/// <see cref="Question"/> for easy integration with existing spawning logic.
/// </summary>
public class BossQuestion
{
    /// <summary>Default question interval at which a boss question spawns.</summary>
    public const int DefaultBossInterval = 10;

    /// <summary>Score multiplier applied to boss question rewards.</summary>
    public const int ScoreMultiplier = 5;

    /// <summary>The full expression text, e.g. <c>"(3 + 5) x 2"</c>.</summary>
    public string Text { get; private set; }

    /// <summary>The correct numerical answer.</summary>
    public int Answer { get; private set; }

    /// <summary>First wrong answer option.</summary>
    public int Wrong1 { get; private set; }

    /// <summary>Second wrong answer option.</summary>
    public int Wrong2 { get; private set; }

    /// <summary>
    /// Shuffled list of <see cref="Answer"/>, <see cref="Wrong1"/>, and
    /// <see cref="Wrong2"/> for lane placement.
    /// </summary>
    public List<int> Numbers { get; private set; }

    /// <summary>Always <c>true</c> for boss questions.</summary>
    public bool IsBoss => true;

    /// <summary>
    /// Creates a new boss question with a randomly generated two-step expression.
    /// </summary>
    public BossQuestion()
    {
        Generate();
    }

    /// <summary>
    /// Determines whether question number <paramref name="questionNumber"/>
    /// should be a boss question.
    /// </summary>
    /// <param name="questionNumber">One-based index of the current question.</param>
    /// <returns><c>true</c> every <see cref="DefaultBossInterval"/> questions.</returns>
    public static bool ShouldSpawnBoss(int questionNumber)
    {
        return questionNumber > 0 && questionNumber % DefaultBossInterval == 0;
    }

    /// <summary>
    /// Determines whether question number <paramref name="questionNumber"/>
    /// should be a boss question using a custom interval.
    /// </summary>
    /// <param name="questionNumber">One-based index of the current question.</param>
    /// <param name="interval">Custom boss interval.</param>
    /// <returns><c>true</c> every <paramref name="interval"/> questions.</returns>
    public static bool ShouldSpawnBoss(int questionNumber, int interval)
    {
        if (interval <= 0) return false;
        return questionNumber > 0 && questionNumber % interval == 0;
    }

    private void Generate()
    {
        int a = Random.Range(1, 13);
        int b = Random.Range(1, 13);
        int c = Random.Range(2, 7);

        int opIndex = Random.Range(0, 4);
        int intermediateResult;
        string innerText;

        switch (opIndex)
        {
            case 0:
                intermediateResult = a + b;
                innerText = $"({a} + {b})";
                break;
            case 1:
                if (a < b) { int tmp = a; a = b; b = tmp; }
                intermediateResult = a - b;
                innerText = $"({a} - {b})";
                break;
            case 2:
                intermediateResult = a * b;
                innerText = $"({a} x {b})";
                break;
            default:
                b = Mathf.Max(b, 1);
                a = b * Random.Range(2, 10);
                intermediateResult = a / b;
                innerText = $"({a} ÷ {b})";
                break;
        }

        int outerOp = Random.Range(0, 2);
        if (outerOp == 0)
        {
            Answer = intermediateResult * c;
            Text = $"{innerText} x {c}";
        }
        else
        {
            Answer = intermediateResult + c;
            Text = $"{innerText} + {c}";
        }

        GenerateWrongAnswers();
        Numbers = ShuffleAnswers(new List<int> { Answer, Wrong1, Wrong2 });
    }

    private void GenerateWrongAnswers()
    {
        int range = Mathf.Max(5, Mathf.Abs(Answer) / 3);
        int safeMin = Mathf.Max(0, Answer - range);
        int safeMax = Answer + range + 1;

        int attempts = 0;
        do
        {
            Wrong1 = Random.Range(safeMin, safeMax);
            attempts++;
            if (attempts > 50) { Wrong1 = Answer + 1; break; }
        } while (Wrong1 == Answer);

        attempts = 0;
        do
        {
            Wrong2 = Random.Range(safeMin, safeMax);
            attempts++;
            if (attempts > 50) { Wrong2 = Answer + 2; break; }
        } while (Wrong2 == Answer || Wrong2 == Wrong1);
    }

    private static List<int> ShuffleAnswers(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        return list;
    }
}
