using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton library of particle effect prefabs. Provides methods to
/// spawn named effects at world positions. When a prefab reference is
/// not assigned, a simple runtime-generated particle system is used
/// as a fallback.
/// </summary>
public class ParticleEffectLibrary : MonoBehaviour
{
    #region Singleton

    /// <summary>Global singleton instance.</summary>
    public static ParticleEffectLibrary Instance { get; private set; }

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

    [Header("Effect Prefabs")]
    [SerializeField, Tooltip("Confetti burst played on a correct answer.")]
    private GameObject correctAnswerConfettiPrefab;

    [SerializeField, Tooltip("Red burst played on a wrong answer.")]
    private GameObject wrongAnswerBurstPrefab;

    [SerializeField, Tooltip("Fireworks played at streak milestones (5, 10, 15, …).")]
    private GameObject streakFireworksPrefab;

    [SerializeField, Tooltip("Glow effect played when a power-up activates.")]
    private GameObject powerUpGlowPrefab;

    [SerializeField, Tooltip("Special effect for boss questions.")]
    private GameObject bossQuestionEffectPrefab;

    [SerializeField, Tooltip("Celebration effect played on level completion.")]
    private GameObject levelCompleteCelebrationPrefab;

    [Header("Additional Prefabs")]
    [SerializeField, Tooltip("Extra named effect prefabs. Name them via the GameObject name.")]
    private GameObject[] extraEffectPrefabs = new GameObject[0];

    private readonly Dictionary<string, GameObject> effectMap = new Dictionary<string, GameObject>();

    private void Start()
    {
        RegisterEffect("correct_confetti", correctAnswerConfettiPrefab);
        RegisterEffect("wrong_burst", wrongAnswerBurstPrefab);
        RegisterEffect("streak_fireworks", streakFireworksPrefab);
        RegisterEffect("powerup_glow", powerUpGlowPrefab);
        RegisterEffect("boss_effect", bossQuestionEffectPrefab);
        RegisterEffect("level_complete", levelCompleteCelebrationPrefab);

        if (extraEffectPrefabs != null)
        {
            foreach (GameObject prefab in extraEffectPrefabs)
            {
                if (prefab != null)
                {
                    RegisterEffect(prefab.name, prefab);
                }
            }
        }
    }

    /// <summary>
    /// Spawns the named particle effect at the given world position.
    /// Falls back to a runtime-generated particle system when the prefab
    /// is not assigned.
    /// </summary>
    /// <param name="effectName">Registered effect name.</param>
    /// <param name="position">World-space position to spawn at.</param>
    public void PlayEffect(string effectName, Vector3 position)
    {
        if (string.IsNullOrEmpty(effectName)) return;

        GameObject prefab;
        if (effectMap.TryGetValue(effectName, out prefab) && prefab != null)
        {
            GameObject instance = Instantiate(prefab, position, Quaternion.identity);
            DestroyAfterParticles(instance);
        }
        else
        {
            SpawnFallbackParticle(position, GetFallbackColor(effectName));
        }
    }

    /// <summary>
    /// Plays the streak milestone fireworks effect. The effect is only
    /// triggered at multiples of 5 (streak 5, 10, 15, …). Larger streaks
    /// produce bigger bursts.
    /// </summary>
    /// <param name="streak">Current streak count.</param>
    /// <param name="position">World-space position to spawn at.</param>
    public void PlayStreakEffect(int streak, Vector3 position)
    {
        if (streak <= 0 || streak % 5 != 0) return;

        if (streakFireworksPrefab != null)
        {
            GameObject instance = Instantiate(streakFireworksPrefab, position, Quaternion.identity);
            ScaleParticles(instance, 1f + (streak / 10f));
            DestroyAfterParticles(instance);
        }
        else
        {
            float scale = 1f + (streak / 10f);
            SpawnFallbackParticle(position, Color.yellow, scale);
        }
    }

    private void RegisterEffect(string name, GameObject prefab)
    {
        if (string.IsNullOrEmpty(name)) return;
        effectMap[name] = prefab;
    }

    private static void DestroyAfterParticles(GameObject instance)
    {
        if (instance == null) return;

        ParticleSystem ps = instance.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            if (!main.loop)
            {
                Destroy(instance, main.duration + main.startLifetime.constantMax);
                return;
            }
        }

        Destroy(instance, 5f);
    }

    private static void ScaleParticles(GameObject instance, float scale)
    {
        if (instance == null) return;

        instance.transform.localScale *= scale;

        ParticleSystem ps = instance.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.startSpeedMultiplier *= scale;
            main.startSizeMultiplier *= scale;
        }
    }

    private static void SpawnFallbackParticle(Vector3 position, Color color, float scale = 1f)
    {
        GameObject go = new GameObject("FallbackParticle");
        go.transform.position = position;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        // AddComponent starts the system with playOnAwake; stop before mutating main module.
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 0.5f;
        main.startLifetime = 1f;
        main.startSpeed = 3f * scale;
        main.startSize = 0.15f * scale;
        main.startColor = color;
        main.maxParticles = Mathf.RoundToInt(30 * scale);

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, (short)Mathf.Max(1, Mathf.RoundToInt(20 * scale)))
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.3f * scale;

        ps.Play();

        Object.Destroy(go, main.duration + main.startLifetime.constantMax + 0.5f);
    }

    private static Color GetFallbackColor(string effectName)
    {
        if (string.IsNullOrEmpty(effectName)) return Color.white;

        if (effectName.Contains("correct") || effectName.Contains("confetti"))
            return Color.green;
        if (effectName.Contains("wrong") || effectName.Contains("burst"))
            return Color.red;
        if (effectName.Contains("streak") || effectName.Contains("firework"))
            return Color.yellow;
        if (effectName.Contains("powerup") || effectName.Contains("glow"))
            return Color.cyan;
        if (effectName.Contains("boss"))
            return new Color(0.6f, 0.2f, 0.9f);
        if (effectName.Contains("level") || effectName.Contains("complete") || effectName.Contains("celebration"))
            return new Color(1f, 0.8f, 0f);

        return Color.white;
    }
}
