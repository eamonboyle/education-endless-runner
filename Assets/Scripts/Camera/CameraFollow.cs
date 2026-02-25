using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private float offsetZ;
    private bool initialized;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            offsetZ = transform.position.z - player.transform.position.z;
            initialized = true;
        }
        else
        {
            Debug.LogWarning("CameraFollow: Player not found. Camera will not follow.");
        }
    }

    private void LateUpdate()
    {
        if (!initialized || player == null) return;

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            player.transform.position.z + offsetZ
        );
    }
}
