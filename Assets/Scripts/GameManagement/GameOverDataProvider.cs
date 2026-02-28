using UnityEngine;
using MathRunner.Data;

/// <summary>
/// Static helper that collects all game-over statistics into a single
/// struct for display by the game-over screen.
/// </summary>
public static class GameOverDataProvider
{
    /// <summary>All stats for a completed game session.</summary>
    public struct GameOverData
    {
        public int FinalScore;
        public int HighScore;
        public bool IsNewHighScore;
        public int QuestionsAnswered;
        public int CorrectAnswers;
        public float Accuracy;
        public int BestStreak;
        public int ComboBonus;
        public float TimePlayed;
        public int XPEarned;
        public string Mode;
    }

    /// <summary>
    /// Gathers data from <see cref="GameState"/>, <see cref="ComboSystem"/>,
    /// and <see cref="PlayerStats"/> into a <see cref="GameOverData"/> struct.
    /// </summary>
    public static GameOverData GetGameOverData()
    {
        int finalScore = GameState.GetScore();
        int highScore = GameState.GetHighScore();
        string mode = GameState.GetQuestionType();

        int questionsAnswered = GameState.GetQuestionsAnsweredThisGame();
        int correctAnswers = GameState.GetCorrectAnswersThisGame();
        float accuracy = GameState.GetAccuracyThisGame();
        float timePlayed = GameState.GetGameDuration();

        int bestStreak = 0;
        int comboMultiplier = 1;
        if (ComboSystem.Instance != null)
        {
            bestStreak = ComboSystem.Instance.GetBestStreak();
            comboMultiplier = ComboSystem.Instance.GetMultiplier();
        }
        int comboBonus = Mathf.Max(0, (comboMultiplier - 1) * finalScore / 10);

        int xpEarned = XPSystem.CalculateGameXP(finalScore, accuracy, bestStreak);

        return new GameOverData
        {
            FinalScore = finalScore,
            HighScore = highScore,
            IsNewHighScore = GameState.IsNewHighScore(),
            QuestionsAnswered = questionsAnswered,
            CorrectAnswers = correctAnswers,
            Accuracy = accuracy,
            BestStreak = bestStreak,
            ComboBonus = comboBonus,
            TimePlayed = timePlayed,
            XPEarned = xpEarned,
            Mode = string.IsNullOrEmpty(mode) ? "addition" : mode
        };
    }
}
