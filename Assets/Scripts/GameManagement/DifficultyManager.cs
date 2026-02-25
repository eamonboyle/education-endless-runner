using UnityEngine;
using Unity.RemoteConfig;

public class DifficultyManager : MonoBehaviour
{
    public float currentSpeed;
    public float speedMultiplier = 10.0f;

    public struct userAttributes { }
    public struct appAttributes { }

    private void Awake()
    {
        ConfigManager.FetchCompleted += ApplyRemoteSettings;

        try
        {
            ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("DifficultyManager: Remote Config fetch failed: " + e.Message);
        }
    }

    private void OnDestroy()
    {
        ConfigManager.FetchCompleted -= ApplyRemoteSettings;
    }

    private void ApplyRemoteSettings(ConfigResponse configResponse)
    {
        switch (configResponse.requestOrigin)
        {
            case ConfigOrigin.Default:
                break;
            case ConfigOrigin.Cached:
                break;
            case ConfigOrigin.Remote:
                float remoteMultiplier = ConfigManager.appConfig.GetFloat("speedMultiplier");
                if (remoteMultiplier > 0f)
                    speedMultiplier = remoteMultiplier;
                break;
        }
    }

    void Start()
    {
        currentSpeed = GameState.GetCharacterSpeed();
    }

    void Update()
    {
        if (GameState.IsRunning())
        {
            currentSpeed += (Time.deltaTime / speedMultiplier);
            GameState.SetCharacterSpeed(currentSpeed);
        }
    }
}
