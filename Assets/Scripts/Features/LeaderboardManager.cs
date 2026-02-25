using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a local top-10 leaderboard per game mode, persisted in PlayerPrefs
/// as JSON.  Designed with hooks for future online integration.
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    #region Singleton
    /// <summary>Global singleton instance.</summary>
    public static LeaderboardManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    /// <summary>Maximum number of entries stored per mode.</summary>
    private const int MaxEntries = 10;

    /// <summary>A single leaderboard entry.</summary>
    [Serializable]
    public class LeaderboardEntry
    {
        /// <summary>Display name of the player.</summary>
        public string PlayerName;

        /// <summary>Score achieved.</summary>
        public int Score;

        /// <summary>Game mode key (e.g. "addition").</summary>
        public string Mode;

        /// <summary>ISO-8601 date string of when the score was set.</summary>
        public string Date;
    }

    /// <summary>Serializable wrapper so JsonUtility can handle a list.</summary>
    [Serializable]
    private class LeaderboardData
    {
        public List<LeaderboardEntry> Entries = new List<LeaderboardEntry>();
    }

    /// <summary>
    /// Adds a score for the current player and mode. The entry is inserted
    /// in sorted order and excess entries are trimmed.
    /// </summary>
    /// <param name="mode">Game mode key (e.g. "addition").</param>
    /// <param name="score">Score to record.</param>
    /// <param name="playerName">Player display name (defaults to "Player").</param>
    public void AddScore(string mode, int score, string playerName = "Player")
    {
        LeaderboardData data = LoadData(mode);

        LeaderboardEntry entry = new LeaderboardEntry
        {
            PlayerName = playerName,
            Score = score,
            Mode = mode,
            Date = DateTime.UtcNow.ToString("o")
        };

        data.Entries.Add(entry);
        data.Entries.Sort((a, b) => b.Score.CompareTo(a.Score));

        if (data.Entries.Count > MaxEntries)
        {
            data.Entries.RemoveRange(MaxEntries, data.Entries.Count - MaxEntries);
        }

        SaveData(mode, data);

        // TODO: Push score to online leaderboard service when integrated.
    }

    /// <summary>
    /// Returns the top <paramref name="count"/> scores for a given mode.
    /// </summary>
    public List<LeaderboardEntry> GetTopScores(string mode, int count)
    {
        LeaderboardData data = LoadData(mode);
        int take = Mathf.Min(count, data.Entries.Count);
        return data.Entries.GetRange(0, take);
    }

    /// <summary>
    /// Returns the 1-based rank the given score would achieve in a mode's
    /// leaderboard.  Returns <see cref="MaxEntries"/> + 1 if it would not
    /// place at all.
    /// </summary>
    public int GetRank(string mode, int score)
    {
        LeaderboardData data = LoadData(mode);

        for (int i = 0; i < data.Entries.Count; i++)
        {
            if (score >= data.Entries[i].Score)
            {
                return i + 1;
            }
        }

        return data.Entries.Count < MaxEntries ? data.Entries.Count + 1 : MaxEntries + 1;
    }

    #region Persistence Helpers
    private static string PrefsKey(string mode) => "Leaderboard_" + mode;

    private static LeaderboardData LoadData(string mode)
    {
        string json = PlayerPrefs.GetString(PrefsKey(mode), "");
        if (string.IsNullOrEmpty(json))
        {
            return new LeaderboardData();
        }

        try
        {
            return JsonUtility.FromJson<LeaderboardData>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning("LeaderboardManager: Failed to parse leaderboard JSON – " + e.Message);
            return new LeaderboardData();
        }
    }

    private static void SaveData(string mode, LeaderboardData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(PrefsKey(mode), json);
        PlayerPrefs.Save();
    }
    #endregion
}
