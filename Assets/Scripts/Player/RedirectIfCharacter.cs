using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RedirectIfCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string character = PlayerPrefs.GetString("character");

        if (character != "" || character != null)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
