using UnityEngine;

public class RedirectIfCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        string character = GameState.GetCharacter();

        if (character != "" || character != null)
        {
            GameManager.instance.LoadMainMenu();
        }
    }
}