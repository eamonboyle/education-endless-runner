using UnityEngine;
using MathRunner.Core;

/// <summary>
/// Spawns non-math obstacles between question sets at random intervals.
/// When Inspector prefabs are missing, builds simple procedural obstacle cubes.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    public enum ObstacleType
    {
        Barrier,
        Gap,
        MovingWall
    }

    [Header("Prefabs")]
    [SerializeField] private GameObject barrierPrefab;
    [SerializeField] private GameObject gapPrefab;
    [SerializeField] private GameObject movingWallPrefab;

    [Header("Settings")]
    [SerializeField] private float spawnChance = 0.15f;
    [SerializeField] private float spawnDistanceAhead = 60f;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform obstacleContainer;
    [SerializeField] private bool enableObstacles = true;

    private static readonly float[] LanePositions =
    {
        GameConstants.LEFT_LANE,
        GameConstants.CENTER_LANE,
        GameConstants.RIGHT_LANE
    };

    private float nextSpawnZ;

    private void Start()
    {
        EnsurePlayer();
        nextSpawnZ = spawnDistanceAhead;
    }

    private void Update()
    {
        if (!enableObstacles) return;
        EnsurePlayer();
        if (!GameState.IsRunning() || player == null) return;

        if (player.transform.position.z + spawnDistanceAhead > nextSpawnZ)
        {
            TrySpawnObstacle();
            nextSpawnZ += GameConstants.QUESTION_SPACING * 0.5f;
        }
    }

    public void TrySpawnObstacle()
    {
        if (Random.value > spawnChance) return;

        ObstacleType type = (ObstacleType)Random.Range(0, 3);
        int laneIndex = Random.Range(0, LanePositions.Length);
        float xPos = LanePositions[laneIndex];
        float zPos = player != null ? player.transform.position.z + spawnDistanceAhead : nextSpawnZ;
        Vector3 spawnPos = new Vector3(xPos, GameConstants.BOX_HEIGHT, zPos);

        GameObject prefab = GetPrefab(type);
        GameObject obstacle;
        if (prefab != null)
        {
            obstacle = Instantiate(prefab, spawnPos, Quaternion.identity,
                obstacleContainer != null ? obstacleContainer : transform);
        }
        else
        {
            obstacle = CreateProceduralObstacle(type, spawnPos);
        }

        Obstacle obstacleComponent = obstacle.GetComponent<Obstacle>();
        if (obstacleComponent != null)
            obstacleComponent.Type = type;
    }

    private GameObject CreateProceduralObstacle(ObstacleType type, Vector3 position)
    {
        PrimitiveType prim = type == ObstacleType.Gap ? PrimitiveType.Cylinder : PrimitiveType.Cube;
        GameObject go = GameObject.CreatePrimitive(prim);
        go.name = "Obstacle_" + type;
        go.transform.SetParent(obstacleContainer != null ? obstacleContainer : transform, false);
        go.transform.position = position;
        go.transform.localScale = type == ObstacleType.MovingWall
            ? new Vector3(0.8f, 1.2f, 0.8f)
            : new Vector3(1.2f, 1.5f, 0.6f);

        var col = go.GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        var renderer = go.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Color c = type == ObstacleType.Gap ? new Color(0.2f, 0.2f, 0.2f, 0.4f)
                : type == ObstacleType.MovingWall ? new Color(0.9f, 0.3f, 0.2f)
                : new Color(0.6f, 0.35f, 0.15f);
            renderer.material.color = c;
        }

        go.AddComponent<Obstacle>().Type = type;
        return go;
    }

    private GameObject GetPrefab(ObstacleType type)
    {
        switch (type)
        {
            case ObstacleType.Barrier: return barrierPrefab;
            case ObstacleType.Gap: return gapPrefab;
            case ObstacleType.MovingWall: return movingWallPrefab;
            default: return barrierPrefab;
        }
    }

    private void EnsurePlayer()
    {
        if (player != null) return;

        GameObject found = GameObject.FindWithTag("Player");
        if (found != null) player = found;
    }
}
