using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public float currentSpeed;
    public float speedMultiplier = 10.0f;
    public bool useLegacyRemoteConfig;

    private void Awake()
    {
        if (useLegacyRemoteConfig)
        {
            Debug.LogWarning("Legacy Unity Remote Config API has been removed from this project upgrade path. Using local speedMultiplier.");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = GameState.GetCharacterSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.IsRunning())
        {
            currentSpeed += (Time.deltaTime / speedMultiplier);
            GameState.SetCharacterSpeed(currentSpeed);
        }
    }
}
