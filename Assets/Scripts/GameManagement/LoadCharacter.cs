using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    public GameObject girlPrefab;
    public GameObject boyPrefab;
    public GameObject playerSpawn;

    // Start is called before the first frame update
    void Start()
    {
        // select from player prefs and choose the right prefab

        InstantiatePlayer(boyPrefab);
    }

    private void InstantiatePlayer(GameObject playerPrefab)
    {
        GameObject player = Instantiate(playerPrefab, new Vector3(0.0f, 0.0f, 6.3f), Quaternion.identity, playerSpawn.transform);
    }
}
