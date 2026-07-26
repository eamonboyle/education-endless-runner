using System;
using System.Collections.Generic;
using UnityEngine;
using MathRunner.Core;

namespace MathRunner.Data
{
    /// <summary>
    /// Represents a single daily challenge with progress tracking.
    /// </summary>
    [Serializable]
    public class Challenge
    {
        /// <summary>Human-readable description of the challenge objective.</summary>
        public string Description { get; private set; }

        /// <summary>Number of actions required to complete the challenge.</summary>
        public int TargetCount { get; private set; }

        /// <summary>
        /// Required math mode, or <c>null</c> if any mode qualifies.
        /// Uses the PlayerPrefs mode string (e.g. <c>"addition"</c>).
        /// </summary>
        public string Mode { get; private set; }

        private readonly string dateKey;

        /// <summary>
        /// Creates a new challenge definition.
        /// </summary>
        /// <param name="description">Player-facing description.</param>
        /// <param name="targetCount">Actions required to complete.</param>
        /// <param name="mode">Required mode string, or null for any.</param>
        /// <param name="dateKey">Date string used for PlayerPrefs storage.</param>
        public Challenge(string description, int targetCount, string mode, string dateKey)
        {
            Description = description;
            TargetCount = targetCount;
            Mode = mode;
            this.dateKey = dateKey;
        }

        /// <summary>Current progress toward <see cref="TargetCount"/>.</summary>
        public int CurrentCount
        {
            get { return PlayerPrefs.GetInt(ProgressKey, 0); }
            private set { PlayerPrefs.SetInt(ProgressKey, value); }
        }

        /// <summary>Whether the challenge objective has been reached.</summary>
        public bool IsComplete
        {
            get { return CurrentCount >= TargetCount; }
        }

        /// <summary>
        /// Increments progress by <paramref name="amount"/>.
        /// No-op if the challenge is already complete.
        /// </summary>
        /// <param name="amount">Amount to add (default 1).</param>
        public void AddProgress(int amount = 1)
        {
            if (IsComplete) return;
            CurrentCount = Mathf.Min(CurrentCount + amount, TargetCount);
            MathRunner.Core.PrefsFlush.MarkDirty();
        }

        private string ProgressKey
        {
            get { return GameConstants.PREF_DAILY_CHALLENGE_PREFIX + dateKey; }
        }
    }

    /// <summary>
    /// Generates and manages a deterministic daily challenge based on the
    /// current date. Progress is persisted in PlayerPrefs.
    /// </summary>
    public static class DailyChallengeData
    {
        private static Challenge cachedChallenge;
        private static string cachedDateKey;

        /// <summary>
        /// Returns today's challenge. The challenge is deterministically
        /// generated from the current UTC date so all players receive the same
        /// challenge on the same day.
        /// </summary>
        /// <returns>Today's <see cref="Challenge"/>.</returns>
        public static Challenge GetTodayChallenge()
        {
            string today = TodayKey();
            if (cachedChallenge != null && cachedDateKey == today)
            {
                return cachedChallenge;
            }

            cachedDateKey = today;
            cachedChallenge = GenerateChallenge(today);
            return cachedChallenge;
        }

        /// <summary>
        /// Records progress toward today's challenge. Only increments if the
        /// provided <paramref name="mode"/> matches the challenge requirement
        /// (or the challenge accepts any mode).
        /// </summary>
        /// <param name="mode">The current mode string.</param>
        /// <param name="amount">Amount of progress to add (default 1).</param>
        public static void RecordProgress(string mode, int amount = 1)
        {
            Challenge c = GetTodayChallenge();
            if (c.IsComplete) return;

            if (c.Mode == null || string.Equals(c.Mode, mode, StringComparison.OrdinalIgnoreCase))
            {
                c.AddProgress(amount);
            }
        }

        /// <summary>
        /// Returns whether today's challenge has been completed.
        /// </summary>
        public static bool IsComplete()
        {
            return GetTodayChallenge().IsComplete;
        }

        /// <summary>
        /// Returns whether the completion reward for today has already been
        /// claimed.
        /// </summary>
        public static bool GetRewardClaimed()
        {
            return PlayerPrefs.GetInt(GameConstants.PREF_DAILY_REWARD_PREFIX + TodayKey(), 0) == 1;
        }

        /// <summary>
        /// Marks today's reward as claimed.
        /// </summary>
        public static void ClaimReward()
        {
            PlayerPrefs.SetInt(GameConstants.PREF_DAILY_REWARD_PREFIX + TodayKey(), 1);
            PlayerPrefs.Save();
        }

        #region Generation

        private static readonly ChallengeTemplate[] templates = new ChallengeTemplate[]
        {
            new ChallengeTemplate("Answer {0} addition questions",       10, 30, "addition"),
            new ChallengeTemplate("Answer {0} subtraction questions",    10, 30, "subtraction"),
            new ChallengeTemplate("Answer {0} multiplication questions", 10, 25, "multiply"),
            new ChallengeTemplate("Answer {0} division questions",       10, 25, "division"),
            new ChallengeTemplate("Answer {0} questions in any mode",    15, 40, null),
            new ChallengeTemplate("Get a streak of {0}",                  3, 10, null),
            new ChallengeTemplate("Score {0} in any mode",              100, 500, null),
            new ChallengeTemplate("Play {0} games",                       2,  5, null),
        };

        private static Challenge GenerateChallenge(string dateKey)
        {
            int seed = dateKey.GetHashCode();
            System.Random rng = new System.Random(seed);

            int templateIndex = rng.Next(0, templates.Length);
            ChallengeTemplate tmpl = templates[templateIndex];

            int target = rng.Next(tmpl.MinTarget, tmpl.MaxTarget + 1);

            if (tmpl.DescriptionFormat.Contains("streak"))
            {
                target = Mathf.Clamp(target, tmpl.MinTarget, tmpl.MaxTarget);
            }
            else if (tmpl.DescriptionFormat.Contains("Score"))
            {
                target = Mathf.RoundToInt(target / 50f) * 50;
                if (target < tmpl.MinTarget) target = tmpl.MinTarget;
            }

            string description = string.Format(tmpl.DescriptionFormat, target);
            return new Challenge(description, target, tmpl.Mode, dateKey);
        }

        private static string TodayKey()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd");
        }

        private struct ChallengeTemplate
        {
            public string DescriptionFormat;
            public int MinTarget;
            public int MaxTarget;
            public string Mode;

            public ChallengeTemplate(string descriptionFormat, int minTarget, int maxTarget, string mode)
            {
                DescriptionFormat = descriptionFormat;
                MinTarget = minTarget;
                MaxTarget = maxTarget;
                Mode = mode;
            }
        }

        #endregion
    }
}
