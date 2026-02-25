using UnityEngine;

/// <summary>
/// Spawns power-up collectibles in random lanes during gameplay.
/// Attach to a manager GameObject in the game scene.
/// </summary>
public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("Prefab with a PowerUpCollectible component.")]
    private GameObject powerUpPrefab;

    [SerializeField, Tooltip("Parent transform for spawned power-ups (keeps hierarchy tidy).")]
    private Transform spawnParent;

    [SerializeField, Tooltip("Chance (0-1) of spawning a power-up each time a question is answered.")]
    private float spawnChance = 0.15f;

    [SerializeField, Tooltip("How far ahead of the player to place the power-up (Z offset).")]
    private float spawnAheadDistance = 25f;

    private static readonly float[] LanePositions =
    {
        GameConstants.LEFT_LANE,
        GameConstants.CENTER_LANE,
        GameConstants.RIGHT_LANE
    };

    /// <summary>
    /// Call after each question is answered to potentially spawn a power-up.
    /// Only spawns when the game is actively running.
    /// </summary>
    public void OnQuestionAnswered()
    {
        if (!GameState.IsRunning()) return;
        if (powerUpPrefab == null) return;

        if (Random.value > spawnChance) return;

        SpawnRandomPowerUp();
    }

    private void SpawnRandomPowerUp()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("PowerUpSpawner: No GameObject tagged 'Player' found.");
            return;
        }

        float laneX = LanePositions[Random.Range(0, LanePositions.Length)];
        float spawnZ = player.transform.position.z + spawnAheadDistance;
        Vector3 position = new Vector3(laneX, GameConstants.BOX_HEIGHT, spawnZ);

        GameObject instance = Instantiate(powerUpPrefab, position, Quaternion.identity, spawnParent);

        PowerUpCollectible collectible = instance.GetComponent<PowerUpCollectible>();
        if (collectible == null)
        {
            Debug.LogWarning("PowerUpSpawner: Prefab is missing a PowerUpCollectible component.");
            Destroy(instance);
            return;
        }

        int typeCount = System.Enum.GetValues(typeof(PowerUpSystem.PowerUpType)).Length;
        PowerUpSystem.PowerUpType randomType = (PowerUpSystem.PowerUpType)Random.Range(0, typeCount);

        // The PowerUpCollectible's serialized field is set via the prefab variant;
        // if a single generic prefab is used, override the type at runtime through reflection
        // or use separate prefabs per type.  For now the prefab's default type is kept.
    }
}
