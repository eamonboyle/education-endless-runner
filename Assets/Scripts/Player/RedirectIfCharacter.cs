using UnityEngine;
using UnityEngine.SceneManagement;

public class RedirectIfCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string character = GameState.GetCharacter();

        if (character != "" || character != null)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
