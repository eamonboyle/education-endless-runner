using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject loadingScreen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SceneManager.LoadSceneAsync((int)SceneIndexes.MAIN_MENU, LoadSceneMode.Additive);
        }
    }

    private List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

    public void LoadGame()
    {
        ShowLoadingScreen();
        UnloadScenes();

        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.GAME, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public void LoadMainMenu()
    {
        ShowLoadingScreen();
        UnloadScenes();

        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.MAIN_MENU, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public void LoadModeSelect()
    {
        ShowLoadingScreen();
        UnloadScenes();

        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.MODE_CHOICE, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public void LoadCharacterSelection()
    {
        ShowLoadingScreen();
        UnloadScenes();

        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.CHARACTER_SELECT, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public void LoadSettings()
    {
        ShowLoadingScreen();
        UnloadScenes();

        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.SETTINGS, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public void LoadTutorial()
    {
        ShowLoadingScreen();
        UnloadScenes();

        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.TUTORIAL, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public IEnumerator GetSceneLoadProgress()
    {
        for (int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
                yield return null;
            }
        }

        HideLoadingScreen();
    }

    private void UnloadScenes()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.buildIndex != (int)SceneIndexes.MANAGER)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }

    private void ShowLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(true);
    }

    private void HideLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(false);
    }
}