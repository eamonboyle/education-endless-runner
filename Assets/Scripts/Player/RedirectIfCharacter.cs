using UnityEngine;

public class RedirectIfCharacter : MonoBehaviour
{
    private void Start()
    {
        string character = GameState.GetCharacter();

        if (!string.IsNullOrEmpty(character))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.LoadMainMenu();
            }
        }
    }
}