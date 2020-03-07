using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    private static bool gameRunning = false;
    private static bool gameOver = false;
    private static int score = 0;
    private static float characterSpeed = 40.0f;

    public static void Init()
    {
        gameRunning = false;
        gameOver = false;
        score = 0;
        characterSpeed = 40.0f;
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

    public static void ShowGameUI()
    {
        GameObject.Find("InGameUI").GetComponent<Canvas>().enabled = true;
        GameObject.Find("GameOverUI").GetComponent<Canvas>().enabled = false;
    }

    public static void ShowGameOverUI()
    {
        SetRunning(false);
        GameObject.Find("InGameUI").GetComponent<Canvas>().enabled = false;
        GameObject.Find("GameOverUI").GetComponent<Canvas>().enabled = true;
    }

    // add methods for settings in here, audio etc
}
