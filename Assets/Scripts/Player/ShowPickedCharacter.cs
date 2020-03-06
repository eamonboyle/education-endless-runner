using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPickedCharacter : MonoBehaviour
{
    public GameObject boy;
    public GameObject girl;

    // Start is called before the first frame update
    void Start()
    {
        string player = PlayerPrefs.GetString("character");

        if (player == "girl")
        {
            girl.SetActive(true);
            boy.SetActive(false);
        }
        else
        {
            girl.SetActive(false);
            boy.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
