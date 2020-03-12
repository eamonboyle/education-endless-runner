using Assets.Scripts.GameManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GameState
{
    private static bool gameRunning = false;
    private static bool gameOver = false;
    private static bool questionExists = false;
    private static int score = 0;
    private static float characterSpeed = 40.0f;

    public static void Init()
    {
        gameRunning = false;
        gameOver = false;
        score = 0;
        characterSpeed = 40.0f;
        SetQuestionExists(false);
    }

    public static void StartGame()
    {
        gameRunning = true;
        gameOver = false;
        score = 0;
    }

    public static void GameOver()
    {
        gameRunning = false;
        gameOver = true;
    }

    public static void SetScore(int s)
    {
        score = s;
    }

    public static int GetScore()
    {
        return score;
    }

    public static void SaveScore()
    {
        PlayerPrefs.SetInt("score", score);
    }

    public static int GetScoreFromSave()
    {
        return PlayerPrefs.GetInt("score");
    }

    public static void SetRunning(bool running)
    {
        gameRunning = running;
    }

    public static bool IsRunning()
    {
        return gameRunning;
    }

    public static void SetGameOver(bool over)
    {
        gameOver = true;
    }

    public static bool IsGameOver()
    {
        return gameOver;
    }
    
    public static void SetCharacterSpeed(float speed)
    {
        characterSpeed = speed;
    }

    public static float GetCharacterSpeed()
    {
        return characterSpeed;
    }

    public static void SetCharacter(string character)
    {
        PlayerPrefs.SetString("character", character);
    }

    public static string GetCharacter()
    {
        return PlayerPrefs.GetString("character");
    }

    public static int GetPlayCount()
    {
        return PlayerPrefs.GetInt("gamesPlayed");
    }

    public static void SetPlayCount(int playCount)
    {
        PlayerPrefs.SetInt("gamesPlayed", playCount);
    }

    public static void IncrementPlayCount()
    {
        int count = PlayerPrefs.GetInt("gamesPlayed");
        PlayerPrefs.SetInt("gamesPlayed", ++count);
    }

    public static void SetQuestionType(string questionType)
    {
        PlayerPrefs.SetString("mode", questionType);
    }

    public static string GetQuestionType()
    {
        return PlayerPrefs.GetString("mode");
    }

    public static void SetQuestionExists(bool exists)
    {
        questionExists = exists;
    }

    public static bool GetQuestionExists()
    {
        return questionExists;
    }

    public static void SetHighScore()
    {
        int s = GetHighScore();

        if (score > s)
        {
            // TODO, DO AN EFFECT LIKE CONFETTI HERE?
            PlayerPrefs.SetInt("highScore_" + GetQuestionType(), score);
        }
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt("highScore_" + GetQuestionType());
    }

    public static void SetFirstLoad()
    {
        PlayerPrefs.SetInt("firstLoad", 1);
    }

    public static bool IsFirstLoad()
    {
        if (PlayerPrefs.GetInt("firstLoad") == 0)
        {
            // enter default settings
            SettingState.SetSound(true);
            SettingState.SetGraphics("Medium");

            return true;
        }

        return false;
    }

    public static void ShowGameUI()
    {
        GameObject.Find("InGameUI").GetComponent<Canvas>().enabled = true;
        GameObject.Find("GameOverUI").GetComponent<Canvas>().enabled = false;
        GameObject.Find("PauseUI").GetComponent<Canvas>().enabled = false;
    }

    public static void ShowPauseUI()
    {
        SetRunning(false);
        QuestionBoxShow(false);
        GameObject.Find("PlayerObject").GetComponent<Animator>().SetBool("isRunning", false);
        GameObject.Find("InGameUI").GetComponent<Canvas>().enabled = false;
        GameObject.Find("GameOverUI").GetComponent<Canvas>().enabled = false;
        GameObject.Find("PauseUI").GetComponent<Canvas>().enabled = true;
        GameObject.Find("QuestionText").SetActive(false);
    }

    public static void QuestionBoxShow(bool show)
    {
        if (!show)
        {
            foreach (GameObject box in GameObject.FindGameObjectsWithTag("QuestionBox"))
            {
                box.GetComponent<MeshRenderer>().enabled = false;
                box.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else
        {
            foreach (GameObject box in GameObject.FindGameObjectsWithTag("QuestionBox"))
            {
                box.GetComponent<MeshRenderer>().enabled = true;
                box.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    public static void ShowGameOverUI()
    {
        QuestionBoxShow(false);
        SetRunning(false);
        SetQuestionExists(false);
        SetHighScore();
        GameObject.Find("HighScoreAmount").GetComponent<Text>().text = GetHighScore().ToString();
        GameObject.Find("CurrentScoreAmount").GetComponent<Text>().text = GetScore().ToString();
        GameObject.Find("InGameUI").GetComponent<Canvas>().enabled = false;
        GameObject.Find("PauseUI").GetComponent<Canvas>().enabled = false;
        GameObject.Find("GameOverUI").GetComponent<Canvas>().enabled = true;
        GameObject.Find("QuestionText").SetActive(false);
    }

    public static void ShowTutorialGameOver()
    {
        SetRunning(false);
        GameObject.Find("TutorialUI").GetComponent<Canvas>().enabled = false;
        GameObject.Find("TutorialGameOverUI").GetComponent<Canvas>().enabled = true;
        GameObject.Find("QuestionText").SetActive(false);
    }

    public static void ShowTutorialCompleteUI()
    {
        SetRunning(false);
        GameObject.Find("TutorialUI").GetComponent<Canvas>().enabled = false;
        GameObject.Find("TutorialGameOverUI").GetComponent<Canvas>().enabled = false;
        GameObject.Find("TutorialCompleteUI").GetComponent<Canvas>().enabled = true;
        GameObject.Find("QuestionText").SetActive(false);
        GameObject.Find("PlayerObject").GetComponent<Animator>().SetBool("isRunning", false);
        GameObject.Find("PlayerObject").GetComponent<Animator>().SetBool("dancing", true);
        SetFirstLoad();
    }

    // add methods for settings in here, audio etc
}
