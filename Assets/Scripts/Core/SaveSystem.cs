using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MathRunner.Core
{
    /// <summary>
    /// Static save/load system that serialises all game data to a JSON file
    /// in <see cref="Application.persistentDataPath"/>. Provides backward-
    /// compatible migration from legacy PlayerPrefs data.
    /// </summary>
    public static class SaveSystem
    {
        private const string FileName = "save.json";

        /// <summary>
        /// Root data container that is serialised to disk.
        /// </summary>
        [Serializable]
        public class SaveData
        {
            /// <summary>Player display name.</summary>
            public string PlayerName = "Player";

            /// <summary>Selected character key (e.g. "boy" / "girl").</summary>
            public string SelectedCharacter = "boy";

            /// <summary>List of unlocked character keys.</summary>
            public List<string> UnlockedCharacters = new List<string> { "boy", "girl" };

            /// <summary>List of unlocked theme keys.</summary>
            public List<string> UnlockedThemes = new List<string> { "default" };

            /// <summary>Active theme key.</summary>
            public string ActiveTheme = "default";

            /// <summary>Highest unlocked campaign level (0-based).</summary>
            public int CampaignProgress;

            /// <summary>Star ratings per campaign level.</summary>
            public List<int> CampaignStars = new List<int>();

            /// <summary>Achievement ids that have been unlocked.</summary>
            public List<string> UnlockedAchievements = new List<string>();

            /// <summary>Per-mode high scores keyed by mode string.</summary>
            public SerializableDictionary HighScores = new SerializableDictionary();

            /// <summary>Per-mode games-played counts.</summary>
            public SerializableDictionary GamesPlayed = new SerializableDictionary();

            /// <summary>Per-mode total questions answered.</summary>
            public SerializableDictionary TotalQuestions = new SerializableDictionary();

            /// <summary>Per-mode correct answer counts.</summary>
            public SerializableDictionary CorrectAnswers = new SerializableDictionary();

            /// <summary>Per-mode best streak.</summary>
            public SerializableDictionary BestStreaks = new SerializableDictionary();

            /// <summary>Total play time in seconds.</summary>
            public float TotalTimePlayed;

            /// <summary>Total games played across all modes.</summary>
            public int TotalGamesPlayed;

            /// <summary>Whether the tutorial has been completed.</summary>
            public bool TutorialCompleted;

            /// <summary>Sound enabled flag.</summary>
            public bool SoundEnabled = true;

            /// <summary>Graphics quality name.</summary>
            public string GraphicsQuality = "Medium";

            /// <summary>Selected difficulty level (0=Easy, 1=Medium, 2=Hard).</summary>
            public int Difficulty = 1;

            /// <summary>Selected math mode key.</summary>
            public string SelectedMode = "addition";

            /// <summary>ISO-8601 timestamp of the last save.</summary>
            public string LastSaveDate = "";

            /// <summary>Save-format version for forward compatibility.</summary>
            public int Version = 1;
        }

        /// <summary>
        /// Simple key-value pair list serialisable by <see cref="JsonUtility"/>,
        /// which cannot serialise <c>Dictionary</c> directly.
        /// </summary>
        [Serializable]
        public class SerializableDictionary
        {
            public List<string> Keys = new List<string>();
            public List<int> Values = new List<int>();

            /// <summary>Sets or updates a value for <paramref name="key"/>.</summary>
            public void Set(string key, int value)
            {
                int idx = Keys.IndexOf(key);
                if (idx >= 0)
                {
                    Values[idx] = value;
                }
                else
                {
                    Keys.Add(key);
                    Values.Add(value);
                }
            }

            /// <summary>Returns the value for <paramref name="key"/>, or 0 if not found.</summary>
            public int Get(string key)
            {
                int idx = Keys.IndexOf(key);
                return idx >= 0 ? Values[idx] : 0;
            }
        }

        private static string FilePath
        {
            get { return Path.Combine(Application.persistentDataPath, FileName); }
        }

        /// <summary>
        /// Saves the provided <paramref name="data"/> (or a new default) to disk.
        /// </summary>
        /// <param name="data">Data to persist. Pass <c>null</c> to save defaults.</param>
        public static void Save(SaveData data = null)
        {
            if (data == null)
            {
                data = BuildFromPlayerPrefs();
            }

            data.LastSaveDate = DateTime.UtcNow.ToString("o");

            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError("SaveSystem.Save failed: " + e.Message);
            }
        }

        /// <summary>
        /// Loads game data from disk. Falls back to a PlayerPrefs migration if
        /// no save file exists.
        /// </summary>
        /// <returns>The loaded <see cref="SaveData"/>.</returns>
        public static SaveData Load()
        {
            if (!File.Exists(FilePath))
            {
                Debug.Log("SaveSystem: No save file found, migrating from PlayerPrefs.");
                return BuildFromPlayerPrefs();
            }

            try
            {
                string json = File.ReadAllText(FilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                return data ?? new SaveData();
            }
            catch (Exception e)
            {
                Debug.LogWarning("SaveSystem.Load failed: " + e.Message);
                return new SaveData();
            }
        }

        /// <summary>
        /// Deletes the save file from disk.
        /// </summary>
        public static void DeleteSave()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("SaveSystem.DeleteSave failed: " + e.Message);
            }
        }

        /// <summary>
        /// Returns a pretty-printed JSON string of the save data (for sharing or debugging).
        /// </summary>
        public static string ExportToJson()
        {
            SaveData data = Load();
            return JsonUtility.ToJson(data, true);
        }

        /// <summary>
        /// Imports game data from a JSON string and persists it to disk.
        /// </summary>
        /// <param name="json">A JSON string previously produced by <see cref="ExportToJson"/>.</param>
        /// <returns><c>true</c> if import succeeded.</returns>
        public static bool ImportFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("SaveSystem.ImportFromJson: Empty JSON string.");
                return false;
            }

            try
            {
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                if (data == null)
                {
                    Debug.LogWarning("SaveSystem.ImportFromJson: Deserialisation returned null.");
                    return false;
                }

                Save(data);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("SaveSystem.ImportFromJson failed: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Constructs a <see cref="SaveData"/> by reading existing PlayerPrefs keys,
        /// providing backward compatibility for players upgrading from the legacy
        /// PlayerPrefs-only persistence.
        /// </summary>
        private static SaveData BuildFromPlayerPrefs()
        {
            SaveData data = new SaveData();

            data.SelectedCharacter = PlayerPrefs.GetString(GameConstants.PREF_CHARACTER, "boy");
            data.SelectedMode = PlayerPrefs.GetString(GameConstants.PREF_MODE, "addition");
            data.TutorialCompleted = PlayerPrefs.GetInt(GameConstants.PREF_FIRST_LOAD, 0) == 1;
            data.TotalGamesPlayed = PlayerPrefs.GetInt(GameConstants.PREF_GAMES_PLAYED, 0);
            data.TotalTimePlayed = PlayerPrefs.GetFloat(GameConstants.PREF_TOTAL_TIME_PLAYED, 0f);
            data.SoundEnabled = PlayerPrefs.GetInt(GameConstants.PREF_SOUND, 1) == 1;
            data.GraphicsQuality = PlayerPrefs.GetString(GameConstants.PREF_GRAPHICS, "Medium");
            data.Difficulty = PlayerPrefs.GetInt(GameConstants.PREF_DIFFICULTY, 1);
            data.CampaignProgress = PlayerPrefs.GetInt(GameConstants.PREF_CAMPAIGN_PROGRESS, 0);

            string[] modes = { "addition", "subtraction", "multiply", "division", "mixed" };
            foreach (string mode in modes)
            {
                int hs = PlayerPrefs.GetInt(GameConstants.PREF_HIGH_SCORE_PREFIX + mode, 0);
                if (hs > 0) data.HighScores.Set(mode, hs);

                int gp = PlayerPrefs.GetInt(GameConstants.PREF_GAMES_PLAYED_PREFIX + mode, 0);
                if (gp > 0) data.GamesPlayed.Set(mode, gp);

                int tq = PlayerPrefs.GetInt(GameConstants.PREF_TOTAL_QUESTIONS_PREFIX + mode, 0);
                if (tq > 0) data.TotalQuestions.Set(mode, tq);

                int ca = PlayerPrefs.GetInt(GameConstants.PREF_CORRECT_ANSWERS_PREFIX + mode, 0);
                if (ca > 0) data.CorrectAnswers.Set(mode, ca);

                int bs = PlayerPrefs.GetInt(GameConstants.PREF_BEST_STREAK_PREFIX + mode, 0);
                if (bs > 0) data.BestStreaks.Set(mode, bs);
            }

            return data;
        }
    }
}
