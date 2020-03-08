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

    int floorCount = 0;

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

            int floorDegreeRandom = Random.Range(0, 2);

            Vector3 floorPlacement = new Vector3(0.0f, 0.0f, currentPlace + (floorWidth * 2) + 18.44f);
            //Quaternion floorRotation = Quaternion.Euler(0.0f, floorDegreeRandom == 1 ? 180.0f : 0.0f, 0.0f);
            Quaternion floorRotation = Quaternion.identity;

            GameObject nextFloor = Instantiate(floorPrefab, floorPlacement, floorRotation, levelContainer.transform);

            floorPieces.Add(nextFloor);

            if (floorCount == 2)
            {
                Destroy(floorPieces[0]);
                floorPieces.RemoveAt(0);

                floorCount = 0;
            }

            currentPlace += floorWidth;
        }
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        Debug.Log("REMOVE FLOOR");
        yield return new WaitForSeconds(time);

        Destroy(floorPieces[0]);
        floorPieces.RemoveAt(0);
    }
}
