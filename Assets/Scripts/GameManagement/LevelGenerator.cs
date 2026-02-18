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
    [SerializeField] private bool useFloorPooling = true;

    int floorCount = 0;
    private readonly Queue<GameObject> pooledFloorPieces = new Queue<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

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
            //Debug.Log("GENERATE FLOOR");

            floorCount++;

            Vector3 floorPlacement = new Vector3(0.0f, 0.0f, currentPlace + (floorWidth * 2) + 18.44f);
            Quaternion floorRotation = Quaternion.identity;

            GameObject nextFloor = GetFloorPiece();
            nextFloor.transform.SetParent(levelContainer.transform, true);
            nextFloor.transform.SetPositionAndRotation(floorPlacement, floorRotation);
            nextFloor.SetActive(true);

            floorPieces.Add(nextFloor);

            if (floorCount == 2)
            {
                ReleaseFloorPiece(floorPieces[0]);
                floorPieces.RemoveAt(0);

                floorCount = 0;
            }

            currentPlace += floorWidth;
        }
    }

    private GameObject GetFloorPiece()
    {
        if (useFloorPooling && pooledFloorPieces.Count > 0)
        {
            return pooledFloorPieces.Dequeue();
        }

        return Instantiate(floorPrefab, levelContainer.transform);
    }

    private void ReleaseFloorPiece(GameObject floorPiece)
    {
        if (!useFloorPooling)
        {
            Destroy(floorPiece);
            return;
        }

        floorPiece.SetActive(false);
        floorPiece.transform.SetParent(levelContainer.transform, true);
        pooledFloorPieces.Enqueue(floorPiece);
    }
}
