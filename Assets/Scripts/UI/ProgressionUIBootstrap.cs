using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Spawns progression UI overlays on menu scenes only. Never during gameplay.
/// Objects are moved into the loaded menu scene so additive unload removes them.
/// </summary>
public class ProgressionUIBootstrap : MonoBehaviour
{
    private static bool created;

    private void Awake()
    {
        if (created)
        {
            Destroy(gameObject);
            return;
        }
        created = true;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string name = scene.name;

        // Gameplay / tutorial: strip any leftover menu overlays.
        if (IsGameplayScene(name))
        {
            DestroyAllOfType<DailyChallengeDisplay>();
            DestroyAllOfType<WeeklyChallengeDisplay>();
            DestroyAllOfType<StatsDisplay>();
            DestroyAllOfType<PlayStyleSelect>();
            DestroyAllOfType<AccessibilitySettingsUI>();
            return;
        }

        if (name == "MainMenu" || name == "Main Menu")
        {
            EnsureInScene<DailyChallengeDisplay>(scene, "[DailyChallengeDisplay]");
            EnsureInScene<WeeklyChallengeDisplay>(scene, "[WeeklyChallengeDisplay]");
            EnsureInScene<StatsDisplay>(scene, "[StatsDisplay]");
        }
        else if (name == "Settings")
        {
            EnsureInScene<StatsDisplay>(scene, "[StatsDisplay]");
            EnsureInScene<AccessibilitySettingsUI>(scene, "[AccessibilitySettingsUI]");
        }
        else if (name == "ModeChoice" || name == "Mode Choice")
        {
            EnsureInScene<PlayStyleSelect>(scene, "[PlayStyleSelect]");
        }
    }

    private static bool IsGameplayScene(string name)
    {
        return name == "Game" || name == "Tutorial";
    }

    private static void EnsureInScene<T>(Scene scene, string objectName) where T : Component
    {
        T existing = Object.FindAnyObjectByType<T>();
        if (existing != null)
        {
            if (existing.gameObject.scene != scene)
                SceneManager.MoveGameObjectToScene(existing.gameObject, scene);
            return;
        }

        var go = new GameObject(objectName);
        SceneManager.MoveGameObjectToScene(go, scene);
        go.AddComponent<T>();
    }

    private static void DestroyAllOfType<T>() where T : Component
    {
        T[] found = Object.FindObjectsByType<T>(FindObjectsInactive.Include);
        for (int i = 0; i < found.Length; i++)
        {
            if (found[i] != null)
                Object.Destroy(found[i].gameObject);
        }
    }
}
