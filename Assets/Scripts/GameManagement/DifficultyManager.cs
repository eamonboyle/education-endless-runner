using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public float currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = GameState.GetCharacterSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        currentSpeed += (Time.deltaTime / 1.7f);
        GameState.SetCharacterSpeed(currentSpeed);
    }
}
