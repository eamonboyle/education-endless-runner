using System;
using System.Collections.Generic;
using UnityEngine;

namespace MathRunner.Core
{
    /// <summary>
    /// Lightweight local analytics tracker. Stores the most recent events as a
    /// JSON array in PlayerPrefs. No external SDK is required.
    /// </summary>
    public static class AnalyticsManager
    {
        private const string EventsPrefsKey = "Analytics_Events";
        private const int MaxStoredEvents = 100;

        /// <summary>
        /// Represents a single analytics event with a type, timestamp, and
        /// arbitrary key-value data payload.
        /// </summary>
        [Serializable]
        public class AnalyticsEvent
        {
            /// <summary>Event type identifier (e.g. "GameStarted").</summary>
            public string Type;

            /// <summary>ISO-8601 UTC timestamp.</summary>
            public string Timestamp;

            /// <summary>Flat key-value data encoded as parallel lists (JsonUtility limitation).</summary>
            public List<string> DataKeys = new List<string>();

            /// <summary>Values corresponding to <see cref="DataKeys"/>.</summary>
            public List<string> DataValues = new List<string>();
        }

        /// <summary>Wrapper for JsonUtility serialisation of a list.</summary>
        [Serializable]
        private class EventList
        {
            public List<AnalyticsEvent> Events = new List<AnalyticsEvent>();
        }

        /// <summary>
        /// Logs a new analytics event.
        /// </summary>
        /// <param name="type">Event type string.</param>
        /// <param name="data">Optional key-value payload.</param>
        public static void LogEvent(string type, Dictionary<string, string> data = null)
        {
            AnalyticsEvent evt = new AnalyticsEvent
            {
                Type = type,
                Timestamp = DateTime.UtcNow.ToString("o")
            };

            if (data != null)
            {
                foreach (var kvp in data)
                {
                    evt.DataKeys.Add(kvp.Key);
                    evt.DataValues.Add(kvp.Value);
                }
            }

            EventList list = LoadEvents();
            list.Events.Add(evt);

            while (list.Events.Count > MaxStoredEvents)
            {
                list.Events.RemoveAt(0);
            }

            SaveEvents(list);
        }

        /// <summary>
        /// Convenience method: logs a "GameStarted" event.
        /// </summary>
        public static void LogGameStarted()
        {
            LogEvent("GameStarted");
        }

        /// <summary>
        /// Convenience method: logs a "GameEnded" event with score, mode, and duration.
        /// </summary>
        public static void LogGameEnded(int score, string mode, float durationSeconds)
        {
            LogEvent("GameEnded", new Dictionary<string, string>
            {
                { "score", score.ToString() },
                { "mode", mode },
                { "duration", durationSeconds.ToString("F1") }
            });
        }

        /// <summary>
        /// Convenience method: logs a "QuestionAnswered" event.
        /// </summary>
        public static void LogQuestionAnswered(bool correct, string mode)
        {
            LogEvent("QuestionAnswered", new Dictionary<string, string>
            {
                { "correct", correct.ToString() },
                { "mode", mode }
            });
        }

        /// <summary>
        /// Convenience method: logs an "AchievementUnlocked" event.
        /// </summary>
        public static void LogAchievementUnlocked(string achievementId)
        {
            LogEvent("AchievementUnlocked", new Dictionary<string, string>
            {
                { "id", achievementId }
            });
        }

        /// <summary>
        /// Convenience method: logs a "PowerUpCollected" event.
        /// </summary>
        public static void LogPowerUpCollected(string powerUpType)
        {
            LogEvent("PowerUpCollected", new Dictionary<string, string>
            {
                { "type", powerUpType }
            });
        }

        /// <summary>
        /// Convenience method: logs a "LevelCompleted" event.
        /// </summary>
        public static void LogLevelCompleted(int level, int stars)
        {
            LogEvent("LevelCompleted", new Dictionary<string, string>
            {
                { "level", level.ToString() },
                { "stars", stars.ToString() }
            });
        }

        /// <summary>
        /// Returns the most recent <paramref name="count"/> events (newest last).
        /// </summary>
        /// <param name="count">Maximum number of events to return.</param>
        /// <returns>List of recent events.</returns>
        public static List<AnalyticsEvent> GetRecentEvents(int count)
        {
            EventList list = LoadEvents();
            int start = Mathf.Max(0, list.Events.Count - count);
            int length = Mathf.Min(count, list.Events.Count);
            return list.Events.GetRange(start, length);
        }

        /// <summary>
        /// Returns the mode string that appears most frequently in
        /// "GameEnded" events, or <c>"addition"</c> if no data exists.
        /// </summary>
        public static string GetMostPlayedMode()
        {
            EventList list = LoadEvents();
            Dictionary<string, int> modeCounts = new Dictionary<string, int>();

            foreach (AnalyticsEvent evt in list.Events)
            {
                if (evt.Type != "GameEnded") continue;

                string mode = GetEventDataValue(evt, "mode");
                if (string.IsNullOrEmpty(mode)) continue;

                if (!modeCounts.ContainsKey(mode))
                    modeCounts[mode] = 0;
                modeCounts[mode]++;
            }

            string best = "addition";
            int bestCount = 0;
            foreach (var kvp in modeCounts)
            {
                if (kvp.Value > bestCount)
                {
                    bestCount = kvp.Value;
                    best = kvp.Key;
                }
            }

            return best;
        }

        /// <summary>
        /// Returns the average session duration in seconds based on stored
        /// "GameEnded" events. Returns 0 if no data exists.
        /// </summary>
        public static float GetAverageSessionLength()
        {
            EventList list = LoadEvents();
            float total = 0f;
            int count = 0;

            foreach (AnalyticsEvent evt in list.Events)
            {
                if (evt.Type != "GameEnded") continue;

                string durationStr = GetEventDataValue(evt, "duration");
                if (string.IsNullOrEmpty(durationStr)) continue;

                float duration;
                if (float.TryParse(durationStr, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out duration))
                {
                    total += duration;
                    count++;
                }
            }

            return count > 0 ? total / count : 0f;
        }

        #region Helpers

        private static string GetEventDataValue(AnalyticsEvent evt, string key)
        {
            if (evt.DataKeys == null) return null;
            int idx = evt.DataKeys.IndexOf(key);
            if (idx < 0 || idx >= evt.DataValues.Count) return null;
            return evt.DataValues[idx];
        }

        private static EventList LoadEvents()
        {
            string json = PlayerPrefs.GetString(EventsPrefsKey, "");
            if (string.IsNullOrEmpty(json))
            {
                return new EventList();
            }

            try
            {
                EventList list = JsonUtility.FromJson<EventList>(json);
                return list ?? new EventList();
            }
            catch (Exception e)
            {
                Debug.LogWarning("AnalyticsManager: Failed to parse events – " + e.Message);
                return new EventList();
            }
        }

        private static void SaveEvents(EventList list)
        {
            try
            {
                string json = JsonUtility.ToJson(list);
                PlayerPrefs.SetString(EventsPrefsKey, json);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogWarning("AnalyticsManager: Failed to save events – " + e.Message);
            }
        }

        #endregion
    }
}
