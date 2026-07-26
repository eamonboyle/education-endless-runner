using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;

public class DifficultyManager : MonoBehaviour
{
    public float currentSpeed;
    public float speedMultiplier = 10.0f;

    public struct userAttributes { }
    public struct appAttributes { }

    private async void Awake()
    {
        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;

        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            RemoteConfigService.Instance.FetchConfigs<userAttributes, appAttributes>(
                new userAttributes(),
                new appAttributes());
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("DifficultyManager: Remote Config fetch failed: " + e.Message);
        }
    }

    private void OnDestroy()
    {
        RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteSettings;
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
                float remoteMultiplier = RemoteConfigService.Instance.appConfig.GetFloat("speedMultiplier");
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
            float effectiveSpeed = currentSpeed;
            var powerUps = PowerUpSystem.Instance;
            if (powerUps != null)
                effectiveSpeed *= powerUps.GetSpeedMultiplier();
            GameState.SetCharacterSpeed(effectiveSpeed);
        }
    }
}
