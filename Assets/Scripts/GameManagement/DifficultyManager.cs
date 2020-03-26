using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public float currentSpeed;
    public float speedMultiplier = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = GameState.GetCharacterSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.IsRunning())
        {
            currentSpeed += (Time.deltaTime / speedMultiplier);
            GameState.SetCharacterSpeed(currentSpeed);
        }
    }
}
