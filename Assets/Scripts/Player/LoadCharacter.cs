using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    public GameObject girlPrefab;
    public GameObject boyPrefab;
    public GameObject playerSpawn;

    private string character;

    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // select from player prefs and choose the right prefab
        character = GameState.GetCharacter();

        if (character == "" || character == null)
        {
            character = "boy";
        }

        ChooseCharacter();

        // set the ad count to 2
        GameState.ResetAdCount();
    }

    private void ChooseCharacter()
    {
        switch (character)
        {
            case "boy":
                InstantiatePlayer(boyPrefab);
                break;
            case "girl":
                InstantiatePlayer(girlPrefab);
                break;
            default:
                break;
        }
    }

    private void InstantiatePlayer(GameObject playerPrefab)
    {
        GameObject player = Instantiate(playerPrefab, new Vector3(0.0f, 0.0f, 6.3f), Quaternion.identity, playerSpawn.transform);
        player.name = "PlayerObject";
    }
}
