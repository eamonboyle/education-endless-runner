using UnityEngine;

/// <summary>
/// Attach to a power-up prefab placed in a lane.  When the player runs
/// through it the matching power-up is activated and this object is destroyed.
/// The object rotates slowly for visual flair.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PowerUpCollectible : MonoBehaviour
{
    [SerializeField, Tooltip("Which power-up this collectible grants.")]
    private PowerUpType powerUpType;

    [SerializeField, Tooltip("Rotation speed in degrees per second.")]
    private float rotationSpeed = 90f;

    /// <summary>The power-up type exposed for external reads (e.g. UI tooltips).</summary>
    public PowerUpType PowerUpType => powerUpType;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (PowerUpSystem.Instance != null)
        {
            PowerUpSystem.Instance.ActivatePowerUp(powerUpType);
        }
        else
        {
            Debug.LogWarning("PowerUpCollectible: PowerUpSystem.Instance is null. Power-up not activated.");
        }

        Destroy(gameObject);
    }
}
