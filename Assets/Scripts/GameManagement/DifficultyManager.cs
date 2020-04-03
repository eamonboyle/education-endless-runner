using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;
using System;

public class DifficultyManager : MonoBehaviour
{
    public float currentSpeed;
    public float speedMultiplier = 10.0f;

    public struct userAttributes
    {
    }

    public struct appAttributes
    {
    }

    private void Awake()
    {
        ConfigManager.FetchCompleted += ApplyRemoteSettings;
        ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
    }

    private void ApplyRemoteSettings(ConfigResponse configResponse)
    {
        switch (configResponse.requestOrigin)
        {
            case ConfigOrigin.Default:
                Debug.Log("No settings loaded this session; using default values.");
                break;
            case ConfigOrigin.Cached:
                Debug.Log("No settings loaded this session; using cached values from a previous session.");
                break;
            case ConfigOrigin.Remote:
                Debug.Log("New settings loaded this session; update values accordingly.");
                Debug.Log("speedMultiplier:" + ConfigManager.appConfig.GetFloat("speedMultiplier"));
                speedMultiplier = ConfigManager.appConfig.GetFloat("speedMultiplier");
                break;
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
