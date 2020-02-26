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
    public float currentPlace = 48.0f;

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
            Debug.Log("GENERATE NEW FLOOR TILE");

            GameObject nextFloor = Instantiate(floorPrefab, new Vector3(0.0f, 0.0f, (currentPlace + floorWidth + 18.44f)), Quaternion.identity, levelContainer.transform);

            floorPieces.Add(nextFloor);

            // delete last one
            Destroy(floorPieces[0]);
            floorPieces.RemoveAt(0);

            currentPlace += floorWidth;
        }
    }
}
