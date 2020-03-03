using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject player;
    public GameObject levelContainer;
    public List<GameObject> floorPieces = new List<GameObject>();
    public float floorWidth = 36.0f;
    public float currentPlace = 36.0f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");

        foreach (GameObject piece in floors)
        {
            floorPieces.Add(piece);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.z > (currentPlace + 4.0f))
        {
            int floorDegreeRandom = Random.Range(0, 2);

            Vector3 floorPlacement = new Vector3(0.0f, 0.0f, currentPlace + (floorWidth * 2) + 18.44f);
            Quaternion floorRotation = Quaternion.Euler(0.0f, floorDegreeRandom == 1 ? 180.0f : 0.0f, 0.0f);

            GameObject nextFloor = Instantiate(floorPrefab, floorPlacement, floorRotation, levelContainer.transform);

            floorPieces.Add(nextFloor);

            // delete last one
            Destroy(floorPieces[0]);
            floorPieces.RemoveAt(0);

            currentPlace += floorWidth;
        }
    }
}
