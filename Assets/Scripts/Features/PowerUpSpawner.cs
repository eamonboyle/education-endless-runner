using MathRunner.Core;
using UnityEngine;

/// <summary>
/// Spawns power-up collectibles in random lanes during gameplay.
/// Uses a Resources prefab when available, otherwise builds pickups procedurally.
/// </summary>
public class PowerUpSpawner : MonoBehaviour
{
    private const string ResourcesPrefabPath = "PowerUpPickup";

    [SerializeField, Tooltip("Prefab with a PowerUpCollectible component. Optional — falls back to Resources then procedural.")]
    private GameObject powerUpPrefab;

    [SerializeField, Tooltip("Parent transform for spawned power-ups (keeps hierarchy tidy).")]
    private Transform spawnParent;

    [SerializeField, Tooltip("Chance (0-1) of spawning a power-up after a correct answer.")]
    private float spawnChance = 0.15f;

    [SerializeField, Tooltip("How far ahead of the answer position to place the power-up (Z offset).")]
    private float spawnAheadDistance = 25f;

    private static readonly float[] LanePositions =
    {
        GameConstants.LEFT_LANE,
        GameConstants.CENTER_LANE,
        GameConstants.RIGHT_LANE
    };

    private void Awake()
    {
        if (powerUpPrefab == null)
            powerUpPrefab = Resources.Load<GameObject>(ResourcesPrefabPath);
    }

    /// <summary>
    /// Chance-based spawn ahead of the given world position (typically a answered question box).
    /// </summary>
    public void TrySpawnPowerUp(Vector3 position)
    {
        if (!GameState.IsRunning()) return;
        if (Random.value > spawnChance) return;

        float laneX = LanePositions[Random.Range(0, LanePositions.Length)];
        float spawnZ = position.z + spawnAheadDistance;
        Vector3 spawnPos = new Vector3(laneX, GameConstants.BOX_HEIGHT, spawnZ);

        PowerUpType randomType = (PowerUpType)Random.Range(0, System.Enum.GetValues(typeof(PowerUpType)).Length);
        SpawnAt(spawnPos, randomType);
    }

    private void SpawnAt(Vector3 position, PowerUpType type)
    {
        GameObject instance;
        if (powerUpPrefab != null)
        {
            instance = Instantiate(powerUpPrefab, position, Quaternion.identity, spawnParent);
        }
        else
        {
            instance = PowerUpFactory.CreatePickup(type, position, spawnParent);
        }

        PowerUpCollectible collectible = instance.GetComponent<PowerUpCollectible>();
        if (collectible == null)
        {
            Debug.LogWarning("PowerUpSpawner: Pickup is missing a PowerUpCollectible component.");
            Destroy(instance);
            return;
        }

        collectible.SetType(type);
    }
}
