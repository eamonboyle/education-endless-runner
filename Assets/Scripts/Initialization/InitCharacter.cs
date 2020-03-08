using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!GameState.IsFirstLoad())
        {
            Debug.Log("NOT FIRST LOAD");
        }
    }
}
