using UnityEngine;
using MathRunner.Core;

/// <summary>
/// Spawns non-math obstacles between question sets at random intervals.
/// Obstacle prefabs are assigned in the Inspector and are instantiated
/// in one of the three lanes using positions from <see cref="GameConstants"/>.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    /// <summary>Types of non-math obstacles.</summary>
    public enum ObstacleType
    {
        /// <summary>Static barrier the player must dodge by being in a different lane.</summary>
        Barrier,
        /// <summary>Visual-only gap with no collision (atmospheric effect).</summary>
        Gap,
        /// <summary>Obstacle that slowly moves across lanes.</summary>
        MovingWall
    }

    [Header("Prefabs")]
    [SerializeField, Tooltip("Prefab for the Barrier obstacle.")]
    private GameObject barrierPrefab;

    [SerializeField, Tooltip("Prefab for the Gap obstacle (visual only).")]
    private GameObject gapPrefab;

    [SerializeField, Tooltip("Prefab for the MovingWall obstacle.")]
    private GameObject movingWallPrefab;

    [Header("Settings")]
    [SerializeField, Tooltip("Chance (0–1) of spawning an obstacle between questions.")]
    private float spawnChance = 0.2f;

    [SerializeField, Tooltip("Z offset ahead of the player where obstacles are spawned.")]
    private float spawnDistanceAhead = 60f;

    [SerializeField, Tooltip("Reference to the player GameObject.")]
    private GameObject player;

    [SerializeField, Tooltip("Parent transform for spawned obstacles (optional).")]
    private Transform obstacleContainer;

    private static readonly float[] LanePositions =
    {
        GameConstants.LEFT_LANE,
        GameConstants.CENTER_LANE,
        GameConstants.RIGHT_LANE
    };

    private float nextSpawnZ;

    private void Start()
    {
        if (player == null)
        {
            GameObject found = GameObject.FindWithTag("Player");
            if (found != null) player = found;
        }

        nextSpawnZ = spawnDistanceAhead;
    }

    private void Update()
    {
        if (!GameState.IsRunning() || player == null) return;

        if (player.transform.position.z + spawnDistanceAhead > nextSpawnZ)
        {
            TrySpawnObstacle();
            nextSpawnZ += GameConstants.QUESTION_SPACING * 0.5f;
        }
    }

    /// <summary>
    /// Attempts to spawn an obstacle based on <see cref="spawnChance"/>.
    /// Called automatically between question sets.
    /// </summary>
    public void TrySpawnObstacle()
    {
        if (Random.value > spawnChance) return;

        ObstacleType type = (ObstacleType)Random.Range(0, 3);
        GameObject prefab = GetPrefab(type);

        if (prefab == null) return;

        int laneIndex = Random.Range(0, LanePositions.Length);
        float xPos = LanePositions[laneIndex];
        float zPos = player != null ? player.transform.position.z + spawnDistanceAhead : nextSpawnZ;

        Vector3 spawnPos = new Vector3(xPos, GameConstants.BOX_HEIGHT, zPos);

        GameObject obstacle = Instantiate(prefab, spawnPos, Quaternion.identity,
            obstacleContainer != null ? obstacleContainer : transform);

        Obstacle obstacleComponent = obstacle.GetComponent<Obstacle>();
        if (obstacleComponent != null)
        {
            obstacleComponent.Type = type;
        }
    }

    private GameObject GetPrefab(ObstacleType type)
    {
        switch (type)
        {
            case ObstacleType.Barrier:    return barrierPrefab;
            case ObstacleType.Gap:        return gapPrefab;
            case ObstacleType.MovingWall: return movingWallPrefab;
            default:                      return barrierPrefab;
        }
    }
}
