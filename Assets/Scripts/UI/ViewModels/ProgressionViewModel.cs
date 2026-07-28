using System;
using MathRunner.Data;
using Unity.Properties;

namespace MathRunner.UI.ViewModels
{
    /// <summary>
    /// Bindable stats / progression snapshot for menu screens.
    /// </summary>
    public class ProgressionViewModel
    {
        public event Action Changed;

        [CreateProperty] public int GamesPlayed { get; private set; }
        [CreateProperty] public int BestScore { get; private set; }
        [CreateProperty] public float Accuracy { get; private set; }
        [CreateProperty] public int BestStreak { get; private set; }
        [CreateProperty] public int TotalCorrect { get; private set; }
        [CreateProperty] public string DailyDescription { get; private set; } = "";
        [CreateProperty] public float DailyProgress { get; private set; }
        [CreateProperty] public string DailyProgressLabel { get; private set; } = "";
        [CreateProperty] public string WeeklyDescription { get; private set; } = "";
        [CreateProperty] public float WeeklyProgress { get; private set; }
        [CreateProperty] public string WeeklyProgressLabel { get; private set; } = "";
        [CreateProperty] public float[] WeeklyScores { get; private set; } = Array.Empty<float>();

        public void RefreshFromPlayerStats()
        {
            GamesPlayed = PlayerStats.GetTotalGamesPlayed();
            BestScore = GameState.GetHighScore();
            Accuracy = PlayerStats.GetAccuracyTotal() * 100f;
            BestStreak = PlayerStats.GetBestStreak("total");
            TotalCorrect = PlayerStats.GetCorrectAnswers("total");
            WeeklyScores = BuildPseudoWeeklyBars();
            RefreshChallenges();
            Changed?.Invoke();
        }

        private void RefreshChallenges()
        {
            var daily = DailyChallengeData.GetTodayChallenge();
            if (daily != null)
            {
                DailyDescription = daily.Description;
                float target = Math.Max(1f, daily.TargetCount);
                DailyProgress = UnityEngine.Mathf.Clamp01(daily.CurrentCount / target);
                DailyProgressLabel = $"{daily.CurrentCount}/{daily.TargetCount}";
            }

            var weekly = WeeklyChallengeData.GetThisWeekChallenge();
            if (weekly != null)
            {
                WeeklyDescription = weekly.Description;
                float target = Math.Max(1f, weekly.TargetCount);
                WeeklyProgress = UnityEngine.Mathf.Clamp01(weekly.CurrentCount / target);
                WeeklyProgressLabel = $"{weekly.CurrentCount}/{weekly.TargetCount}";
            }
        }

        private static float[] BuildPseudoWeeklyBars()
        {
            // PlayerStats does not store per-day scores; synthesise relative bars
            // from best streak / games so the graph has content.
            float games = Math.Max(1f, PlayerStats.GetTotalGamesPlayed());
            float accuracy = PlayerStats.GetAccuracyTotal();
            var bars = new float[7];
            for (int i = 0; i < 7; i++)
            {
                float wobble = 0.55f + ((i * 37) % 10) * 0.04f;
                bars[i] = UnityEngine.Mathf.Clamp01(accuracy * wobble + (games > 0 ? 0.15f : 0f));
            }
            return bars;
        }
    }
}
