using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string ScreenshotName = "math-runner-share-score-screenshot.png";

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
        GameState.Init();
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

    private void OnApplicationPause(bool pause)
    {
        print("ApplicationPause: " + pause);

        if (GameState.IsRunning())
        {
            GameState.ShowPauseUI();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        print("ApplicationFocus: " + focus);

        if (GameState.IsRunning())
        {
            GameState.ShowPauseUI();
        }
    }

    private void OnApplicationQuit()
    {
        print("Application quit");
    }


    public void ShareScreenshotWithText(string text)
    {
        string screenShotPath = Application.persistentDataPath + "/" + ScreenshotName;
        if (File.Exists(screenShotPath)) File.Delete(screenShotPath);

        ScreenCapture.CaptureScreenshot(ScreenshotName);

        StartCoroutine(delayedShare(screenShotPath, text));
    }

    //CaptureScreenshot runs asynchronously, so you'll need to either capture the screenshot early and wait a fixed time
    //for it to save, or set a unique image name and check if the file has been created yet before sharing.
    IEnumerator delayedShare(string screenShotPath, string text)
    {
        while (!File.Exists(screenShotPath))
        {
            yield return new WaitForSeconds(.05f);
        }

        text = "I scored " + GameState.GetScore() + " at " + GameState.GetQuestionType() + " on Math Runner, can you beat my score?";
        string subject = "My score on Math Runner app";

        new NativeShare().AddFile(screenShotPath).SetSubject(subject).SetText(text).Share();
        //NativeShare.Share(text, screenShotPath, "", "", "image/png", true, "");
    }

    //---------- Helper Variables ----------//
    private float width
    {
        get
        {
            return Screen.width;
        }
    }

    private float height
    {
        get
        {
            return Screen.height;
        }
    }


    //---------- Screenshot ----------//
    public void Screenshot()
    {
        // Short way
        StartCoroutine(GetScreenshot());
    }

    //---------- Get Screenshot ----------//
    public IEnumerator GetScreenshot()
    {
        yield return new WaitForEndOfFrame();

        // Get Screenshot
        Texture2D screenshot;
        screenshot = new Texture2D((int)width, (int)height, TextureFormat.ARGB32, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
        screenshot.Apply();

        // Save Screenshot
        Save_Screenshot(screenshot);
    }

    //---------- Save Screenshot ----------//
    private void Save_Screenshot(Texture2D screenshot)
    {
        string screenShotPath = Application.persistentDataPath + "/" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss") + "_" + ScreenshotName;
        File.WriteAllBytes(screenShotPath, screenshot.EncodeToPNG());

        // Native Share
        StartCoroutine(DelayedShare_Image(screenShotPath));
    }

    //---------- Clear Saved Screenshots ----------//
    public void Clear_SavedScreenShots()
    {
        string path = Application.persistentDataPath;

        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.png");

        foreach (FileInfo f in info)
        {
            File.Delete(f.FullName);
        }
    }

    //---------- Delayed Share ----------//
    private IEnumerator DelayedShare_Image(string screenShotPath)
    {
        while (!File.Exists(screenShotPath))
        {
            yield return new WaitForSeconds(.05f);
        }

        // Share
        NativeShare_Image(screenShotPath);
    }

    //---------- Native Share ----------//
    private void NativeShare_Image(string screenShotPath)
    {
        string text = "";
        string subject = "";
        string url = "";
        string title = "Select sharing app";

#if UNITY_ANDROID

        subject = "Test subject.";
        text = "Test text";
#endif

#if UNITY_IOS
        subject = "Test subject.";
        text = "Test text";
#endif


        text = "I scored " + GameState.GetScore() + " at " + GameState.GetQuestionType() + " on Math Runner, can you beat my score?";
        subject = "My score on Math Runner app";

        // Share
        new NativeShare().AddFile(screenShotPath).SetSubject(subject).SetText(text).Share();
        //NativeShare.Share(text, screenShotPath, url, subject, "image/png", true, title);
    }
}