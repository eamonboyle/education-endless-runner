using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public float currentSpeed = 40.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentSpeed += (Time.deltaTime / 1.7f);

        GameObject.FindWithTag("Player").GetComponent<PlayerControllerTouch>().speed = currentSpeed;
    }
}
