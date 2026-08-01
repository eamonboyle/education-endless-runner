using System.Collections.Generic;
using UnityEngine;
using MathRunner.Core;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject levelContainer;

    public List<GameObject> floorPieces = new List<GameObject>();
    public float floorWidth = GameConstants.FLOOR_WIDTH;
    public float currentPlace = GameConstants.FLOOR_WIDTH;

    [SerializeField, Tooltip("How many floor tiles to keep alive behind/around the player.")]
    private int maxActiveFloors = 4;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");

        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");

        // Sort floor pieces by their Z coordinate in ascending order
        System.Array.Sort(floors, (a, b) => a.transform.position.z.CompareTo(b.transform.position.z));

        foreach (GameObject piece in floors)
        {
            floorPieces.Add(piece);
        }

        if (GetComponent<FloorVariety>() == null)
            gameObject.AddComponent<FloorVariety>();
    }

    void Update()
    {
        if (player == null) return;

        if (player.transform.position.z > (currentPlace + 4.0f))
        {
            Vector3 floorPlacement = new Vector3(0.0f, 0.0f, currentPlace + (floorWidth * 2) + GameConstants.FLOOR_OFFSET);
            Quaternion floorRotation = Quaternion.identity;

            GameObject nextFloor = Instantiate(floorPrefab, floorPlacement, floorRotation,
                levelContainer != null ? levelContainer.transform : null);

            floorPieces.Add(nextFloor);

            while (floorPieces.Count > maxActiveFloors)
            {
                if (floorPieces[0] != null)
                    Destroy(floorPieces[0]);
                floorPieces.RemoveAt(0);
            }

            currentPlace += floorWidth;
        }
    }
}
