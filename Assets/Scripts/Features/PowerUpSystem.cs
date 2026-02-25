using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Available power-up types in Math Runner.</summary>
public enum PowerUpType
{
    /// <summary>Survive one wrong answer without game over.</summary>
    Shield,
    /// <summary>Reduce character speed by 50 % for a limited time.</summary>
    SlowMotion,
    /// <summary>Double all score gains for a limited time.</summary>
    DoublePoints
}

/// <summary>
/// Manages active power-ups, their durations, and gameplay effects.
/// </summary>
public class PowerUpSystem : MonoBehaviour
{
    #region Singleton
    /// <summary>Global singleton instance.</summary>
    public static PowerUpSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    #region Events
    /// <summary>Fired when a power-up becomes active.</summary>
    public event Action<PowerUpType> OnPowerUpActivated;

    /// <summary>Fired when a power-up expires or is consumed.</summary>
    public event Action<PowerUpType> OnPowerUpExpired;
    #endregion

    [SerializeField, Tooltip("Duration of the SlowMotion power-up in seconds.")]
    private float slowMotionDuration = 10f;

    [SerializeField, Tooltip("Duration of the DoublePoints power-up in seconds.")]
    private float doublePointsDuration = 10f;

    [SerializeField, Tooltip("Speed reduction factor while SlowMotion is active (0.5 = 50 %).")]
    private float slowMotionFactor = 0.5f;

    private readonly Dictionary<PowerUpType, float> activeTimers = new Dictionary<PowerUpType, float>();

    private float originalSpeed;
    private bool slowMotionActive;

    private void Update()
    {
        if (!GameState.IsRunning()) return;

        List<PowerUpType> expired = null;

        // Tick down duration-based power-ups
        var keys = new List<PowerUpType>(activeTimers.Keys);
        foreach (PowerUpType type in keys)
        {
            float remaining = activeTimers[type] - Time.deltaTime;
            if (remaining <= 0f)
            {
                if (expired == null) expired = new List<PowerUpType>();
                expired.Add(type);
            }
            else
            {
                activeTimers[type] = remaining;
            }
        }

        if (expired != null)
        {
            foreach (PowerUpType type in expired)
            {
                DeactivatePowerUp(type);
            }
        }
    }

    /// <summary>
    /// Activates the given power-up. Shield has no timer; SlowMotion
    /// and DoublePoints run for their configured durations.
    /// </summary>
    public void ActivatePowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Shield:
                activeTimers[type] = float.MaxValue;
                break;

            case PowerUpType.SlowMotion:
                if (!slowMotionActive)
                {
                    originalSpeed = GameState.GetCharacterSpeed();
                    GameState.SetCharacterSpeed(originalSpeed * slowMotionFactor);
                    slowMotionActive = true;
                }
                activeTimers[type] = slowMotionDuration;
                break;

            case PowerUpType.DoublePoints:
                activeTimers[type] = doublePointsDuration;
                break;
        }

        OnPowerUpActivated?.Invoke(type);
    }

    /// <summary>Returns true if the specified power-up is currently active.</summary>
    public bool HasActivePowerUp(PowerUpType type)
    {
        return activeTimers.ContainsKey(type);
    }

    /// <summary>
    /// Immediately deactivates a power-up and reverses its gameplay effect.
    /// </summary>
    public void DeactivatePowerUp(PowerUpType type)
    {
        if (!activeTimers.ContainsKey(type)) return;

        activeTimers.Remove(type);

        if (type == PowerUpType.SlowMotion && slowMotionActive)
        {
            GameState.SetCharacterSpeed(originalSpeed);
            slowMotionActive = false;
        }

        OnPowerUpExpired?.Invoke(type);
    }

    /// <summary>
    /// Attempts to consume the Shield power-up.
    /// Returns true if a shield was available and consumed, false otherwise.
    /// </summary>
    public bool TryConsumeShield()
    {
        if (!HasActivePowerUp(PowerUpType.Shield)) return false;

        DeactivatePowerUp(PowerUpType.Shield);
        return true;
    }

    /// <summary>
    /// Returns the score multiplier contribution from power-ups (2 if DoublePoints
    /// is active, 1 otherwise).  Combine with <see cref="ComboSystem.GetMultiplier"/>
    /// for the total multiplier.
    /// </summary>
    public int GetScoreMultiplier()
    {
        return HasActivePowerUp(PowerUpType.DoublePoints) ? 2 : 1;
    }
}
