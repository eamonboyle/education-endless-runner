using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!GameState.IsFirstLoad())
        {
            //SceneManager.LoadScene("MainMenu");
        }
    }
}
