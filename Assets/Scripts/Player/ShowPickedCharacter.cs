using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowPickedCharacter : MonoBehaviour
{
    public GameObject boy;
    public GameObject girl;

    // Start is called before the first frame update
    void Start()
    {
        if (GameState.IsFirstLoad())
        {
            SceneManager.LoadScene("CharacterSelect");
        }

        string player = GameState.GetCharacter();

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
}
