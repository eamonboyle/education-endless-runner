using System;
using System.Globalization;
using UnityEngine;
using MathRunner.Core;

namespace MathRunner.Data
{
    /// <summary>
    /// Represents a single weekly challenge with progress tracking.
    /// </summary>
    [Serializable]
    public class WeeklyChallenge
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

        private readonly string weekKey;

        /// <summary>
        /// Creates a new weekly challenge definition.
        /// </summary>
        /// <param name="description">Player-facing description.</param>
        /// <param name="targetCount">Actions required to complete.</param>
        /// <param name="mode">Required mode string, or null for any.</param>
        /// <param name="weekKey">ISO week key used for PlayerPrefs storage.</param>
        public WeeklyChallenge(string description, int targetCount, string mode, string weekKey)
        {
            Description = description;
            TargetCount = targetCount;
            Mode = mode;
            this.weekKey = weekKey;
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
            PlayerPrefs.Save();
        }

        private string ProgressKey
        {
            get { return "weeklyChallenge_" + weekKey; }
        }
    }

    /// <summary>
    /// Generates and manages a deterministic weekly challenge based on the
    /// current ISO week number. Progress is persisted in PlayerPrefs.
    /// Targets are larger than daily challenges.
    /// </summary>
    public static class WeeklyChallengeData
    {
        private static WeeklyChallenge cachedChallenge;
        private static string cachedWeekKey;

        /// <summary>
        /// Returns this week's challenge. The challenge is deterministically
        /// generated from the current ISO year and week number so all
        /// players receive the same challenge during the same week.
        /// </summary>
        /// <returns>This week's <see cref="WeeklyChallenge"/>.</returns>
        public static WeeklyChallenge GetThisWeekChallenge()
        {
            string key = CurrentWeekKey();
            if (cachedChallenge != null && cachedWeekKey == key)
            {
                return cachedChallenge;
            }

            cachedWeekKey = key;
            cachedChallenge = GenerateChallenge(key);
            return cachedChallenge;
        }

        /// <summary>
        /// Records progress toward this week's challenge. Only increments
        /// if the provided <paramref name="mode"/> matches the challenge
        /// requirement (or the challenge accepts any mode).
        /// </summary>
        /// <param name="mode">The current mode string.</param>
        /// <param name="amount">Amount of progress to add (default 1).</param>
        public static void RecordProgress(string mode, int amount = 1)
        {
            WeeklyChallenge c = GetThisWeekChallenge();
            if (c.IsComplete) return;

            if (c.Mode == null || string.Equals(c.Mode, mode, StringComparison.OrdinalIgnoreCase))
            {
                c.AddProgress(amount);
            }
        }

        /// <summary>
        /// Returns whether this week's challenge has been completed.
        /// </summary>
        /// <returns>True if complete.</returns>
        public static bool IsComplete()
        {
            return GetThisWeekChallenge().IsComplete;
        }

        #region Generation

        private static readonly WeeklyTemplate[] templates = new WeeklyTemplate[]
        {
            new WeeklyTemplate("Score {0} total points",                  500,  2000, null),
            new WeeklyTemplate("Answer {0} questions in any mode",        100,  300,  null),
            new WeeklyTemplate("Get {0} correct answers in addition",      30,  100,  "addition"),
            new WeeklyTemplate("Get {0} correct answers in subtraction",   30,  100,  "subtraction"),
            new WeeklyTemplate("Get {0} correct answers in multiplication",30,  100,  "multiply"),
            new WeeklyTemplate("Get {0} correct answers in division",      30,  100,  "division"),
            new WeeklyTemplate("Play {0} games",                           10,   30,  null),
            new WeeklyTemplate("Achieve a streak of {0}",                   5,   15,  null),
        };

        private static WeeklyChallenge GenerateChallenge(string weekKey)
        {
            int seed = weekKey.GetHashCode();
            System.Random rng = new System.Random(seed);

            int templateIndex = rng.Next(0, templates.Length);
            WeeklyTemplate tmpl = templates[templateIndex];

            int target = rng.Next(tmpl.MinTarget, tmpl.MaxTarget + 1);

            if (tmpl.DescriptionFormat.Contains("Score"))
            {
                target = Mathf.RoundToInt(target / 100f) * 100;
                if (target < tmpl.MinTarget) target = tmpl.MinTarget;
            }
            else if (tmpl.DescriptionFormat.Contains("streak"))
            {
                target = Mathf.Clamp(target, tmpl.MinTarget, tmpl.MaxTarget);
            }

            string description = string.Format(tmpl.DescriptionFormat, target);
            return new WeeklyChallenge(description, target, tmpl.Mode, weekKey);
        }

        private static string CurrentWeekKey()
        {
            DateTime now = DateTime.UtcNow;
            int isoWeek = ISOWeek(now);
            int isoYear = ISOYear(now);
            return isoYear.ToString() + "-W" + isoWeek.ToString("D2");
        }

        private static int ISOWeek(DateTime date)
        {
            Calendar cal = CultureInfo.InvariantCulture.Calendar;
            DayOfWeek day = cal.GetDayOfWeek(date);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                date = date.AddDays(3);
            }
            return cal.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private static int ISOYear(DateTime date)
        {
            DateTime thursday = date.AddDays(DayOfWeek.Thursday - date.DayOfWeek);
            return thursday.Year;
        }

        private struct WeeklyTemplate
        {
            public string DescriptionFormat;
            public int MinTarget;
            public int MaxTarget;
            public string Mode;

            public WeeklyTemplate(string descriptionFormat, int minTarget, int maxTarget, string mode)
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
