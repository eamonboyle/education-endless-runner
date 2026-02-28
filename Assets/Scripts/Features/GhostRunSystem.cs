using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Records and plays back "ghost run" data so the player can race
/// against their own best performance. Ghost frames are sampled at
/// a fixed interval and serialised to PlayerPrefs per mode.
/// </summary>
public class GhostRunSystem : MonoBehaviour
{
    #region Data Classes

    /// <summary>A single snapshot of the player's position during a run.</summary>
    [Serializable]
    public class GhostFrame
    {
        /// <summary>Seconds since recording began.</summary>
        public float Timestamp;

        /// <summary>Lane index: -1 = left, 0 = center, 1 = right.</summary>
        public int LanePosition;

        /// <summary>World-space Z position of the player.</summary>
        public float ZPosition;
    }

    /// <summary>Complete ghost run recording for a single mode.</summary>
    [Serializable]
    public class GhostData
    {
        /// <summary>Game mode key this ghost was recorded in.</summary>
        public string Mode;

        /// <summary>Score achieved during the recorded run.</summary>
        public int Score;

        /// <summary>Ordered list of position frames.</summary>
        public List<GhostFrame> Frames = new List<GhostFrame>();
    }

    #endregion

    [SerializeField, Tooltip("Prefab used to visualise the ghost runner. Should be semi-transparent.")]
    private GameObject ghostPrefab;

    private const float SampleInterval = 0.1f;
    private const string GhostPrefsPrefix = "GhostRun_";

    private bool isRecording;
    private float recordStartTime;
    private float nextSampleTime;
    private GhostData currentRecording;

    private GameObject spawnedGhost;
    private GhostData playbackData;
    private float playbackStartTime;
    private int playbackIndex;

    /// <summary>
    /// Begins recording ghost frames. Call at game start.
    /// </summary>
    public void StartRecording()
    {
        currentRecording = new GhostData();
        isRecording = true;
        recordStartTime = Time.time;
        nextSampleTime = 0f;
    }

    /// <summary>
    /// Stops the current recording session.
    /// </summary>
    public void StopRecording()
    {
        isRecording = false;
    }

    /// <summary>
    /// Persists the current recording if it beats the stored ghost for the
    /// specified <paramref name="mode"/>.
    /// </summary>
    /// <param name="mode">Game mode key.</param>
    public void SaveGhost(string mode)
    {
        if (currentRecording == null || currentRecording.Frames.Count == 0) return;

        currentRecording.Mode = mode;
        currentRecording.Score = GameState.GetScore();

        GhostData existing = LoadGhost(mode);
        if (existing != null && existing.Score >= currentRecording.Score)
        {
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(currentRecording);
            PlayerPrefs.SetString(GhostPrefsPrefix + mode, json);
            PlayerPrefs.Save();
        }
        catch (Exception e)
        {
            Debug.LogWarning("GhostRunSystem.SaveGhost failed: " + e.Message);
        }
    }

    /// <summary>
    /// Loads the stored ghost data for <paramref name="mode"/>.
    /// Returns <c>null</c> if no ghost is stored.
    /// </summary>
    /// <param name="mode">Game mode key.</param>
    /// <returns>The stored <see cref="GhostData"/> or <c>null</c>.</returns>
    public GhostData LoadGhost(string mode)
    {
        string json = PlayerPrefs.GetString(GhostPrefsPrefix + mode, "");
        if (string.IsNullOrEmpty(json)) return null;

        try
        {
            return JsonUtility.FromJson<GhostData>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning("GhostRunSystem.LoadGhost failed: " + e.Message);
            return null;
        }
    }

    /// <summary>
    /// Instantiates the ghost prefab and begins playback of the stored
    /// ghost for <paramref name="mode"/>.
    /// </summary>
    /// <param name="mode">Game mode key.</param>
    public void SpawnGhost(string mode)
    {
        playbackData = LoadGhost(mode);
        if (playbackData == null || playbackData.Frames.Count == 0)
        {
            Debug.Log("GhostRunSystem: No ghost data for mode " + mode);
            return;
        }

        if (ghostPrefab == null)
        {
            Debug.LogWarning("GhostRunSystem: Ghost prefab is not assigned.");
            return;
        }

        if (spawnedGhost != null)
        {
            Destroy(spawnedGhost);
        }

        spawnedGhost = Instantiate(ghostPrefab);
        playbackStartTime = Time.time;
        playbackIndex = 0;
    }

    private void Update()
    {
        if (isRecording)
        {
            SampleFrame();
        }

        if (spawnedGhost != null && playbackData != null)
        {
            UpdatePlayback();
        }
    }

    private void SampleFrame()
    {
        float elapsed = Time.time - recordStartTime;
        if (elapsed < nextSampleTime) return;
        nextSampleTime = elapsed + SampleInterval;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        PlayerMovement pm = player.GetComponent<PlayerMovement>();

        int lane = 0;
        if (pm != null)
        {
            switch (pm.currentLane)
            {
                case PlayerMovement.Lane.Left:   lane = -1; break;
                case PlayerMovement.Lane.Center:  lane = 0;  break;
                case PlayerMovement.Lane.Right:   lane = 1;  break;
            }
        }

        GhostFrame frame = new GhostFrame
        {
            Timestamp = elapsed,
            LanePosition = lane,
            ZPosition = player.transform.position.z
        };

        currentRecording.Frames.Add(frame);
    }

    private void UpdatePlayback()
    {
        float elapsed = Time.time - playbackStartTime;

        while (playbackIndex < playbackData.Frames.Count - 1 &&
               playbackData.Frames[playbackIndex + 1].Timestamp <= elapsed)
        {
            playbackIndex++;
        }

        GhostFrame frame = playbackData.Frames[playbackIndex];

        float laneX;
        switch (frame.LanePosition)
        {
            case -1: laneX = MathRunner.Core.GameConstants.LEFT_LANE;   break;
            case 1:  laneX = MathRunner.Core.GameConstants.RIGHT_LANE;  break;
            default: laneX = MathRunner.Core.GameConstants.CENTER_LANE; break;
        }

        Vector3 pos = spawnedGhost.transform.position;
        pos.x = laneX;
        pos.z = frame.ZPosition;
        spawnedGhost.transform.position = pos;

        if (playbackIndex >= playbackData.Frames.Count - 1)
        {
            Destroy(spawnedGhost);
            spawnedGhost = null;
            playbackData = null;
        }
    }
}
