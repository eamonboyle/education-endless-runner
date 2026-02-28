using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extends the local <see cref="LeaderboardManager"/> with online submission
/// and retrieval stubs. Currently simulates network calls using local data
/// with an artificial delay.
/// </summary>
public class OnlineLeaderboard : MonoBehaviour
{
    #region Singleton

    /// <summary>Global singleton instance.</summary>
    public static OnlineLeaderboard Instance { get; private set; }

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

    /// <summary>A leaderboard entry with online-specific metadata.</summary>
    [Serializable]
    public class OnlineLeaderboardEntry
    {
        /// <summary>Player display name.</summary>
        public string PlayerName;

        /// <summary>Score achieved.</summary>
        public int Score;

        /// <summary>Game mode key.</summary>
        public string Mode;

        /// <summary>ISO-8601 date string.</summary>
        public string Date;

        /// <summary>1-based global rank.</summary>
        public int Rank;
    }

    [SerializeField, Tooltip("Simulated network latency in seconds.")]
    private float simulatedDelay = 1.0f;

    /// <summary>
    /// Submits a score for the given mode. Currently delegates to the local
    /// <see cref="LeaderboardManager"/> with a simulated delay.
    /// </summary>
    /// <param name="mode">Game mode key.</param>
    /// <param name="score">Score to submit.</param>
    /// <param name="callback">
    /// Invoked with <c>true</c> on success, <c>false</c> on failure.
    /// </param>
    public void SubmitScore(string mode, int score, Action<bool> callback)
    {
        if (!IsOnline())
        {
            Debug.LogWarning("OnlineLeaderboard: No internet connection. Score saved locally only.");
            SaveLocally(mode, score);
            callback?.Invoke(false);
            return;
        }

        StartCoroutine(SubmitScoreCoroutine(mode, score, callback));
    }

    /// <summary>
    /// Fetches the top scores for a mode. Currently returns local data after
    /// a simulated delay.
    /// </summary>
    /// <param name="mode">Game mode key.</param>
    /// <param name="count">Number of entries to retrieve.</param>
    /// <param name="callback">Invoked with the results list.</param>
    public void FetchTopScores(string mode, int count, Action<List<OnlineLeaderboardEntry>> callback)
    {
        if (!IsOnline())
        {
            Debug.LogWarning("OnlineLeaderboard: No internet connection. Returning local scores.");
            callback?.Invoke(ConvertLocalScores(mode, count));
            return;
        }

        StartCoroutine(FetchTopScoresCoroutine(mode, count, callback));
    }

    private IEnumerator SubmitScoreCoroutine(string mode, int score, Action<bool> callback)
    {
        yield return new WaitForSecondsRealtime(simulatedDelay);

        // TODO: Replace with Firebase / Unity Gaming Services REST call.
        SaveLocally(mode, score);
        callback?.Invoke(true);
    }

    private IEnumerator FetchTopScoresCoroutine(string mode, int count, Action<List<OnlineLeaderboardEntry>> callback)
    {
        yield return new WaitForSecondsRealtime(simulatedDelay);

        // TODO: Replace with Firebase / Unity Gaming Services REST call.
        List<OnlineLeaderboardEntry> results = ConvertLocalScores(mode, count);
        callback?.Invoke(results);
    }

    private static void SaveLocally(string mode, int score)
    {
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.AddScore(mode, score);
        }
    }

    private static List<OnlineLeaderboardEntry> ConvertLocalScores(string mode, int count)
    {
        List<OnlineLeaderboardEntry> entries = new List<OnlineLeaderboardEntry>();

        if (LeaderboardManager.Instance == null) return entries;

        var localEntries = LeaderboardManager.Instance.GetTopScores(mode, count);
        for (int i = 0; i < localEntries.Count; i++)
        {
            entries.Add(new OnlineLeaderboardEntry
            {
                PlayerName = localEntries[i].PlayerName,
                Score = localEntries[i].Score,
                Mode = localEntries[i].Mode,
                Date = localEntries[i].Date,
                Rank = i + 1
            });
        }

        return entries;
    }

    /// <summary>
    /// Returns whether the device currently has internet connectivity.
    /// </summary>
    private static bool IsOnline()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
}
