using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    private static bool gameRunning;
    private static bool gameOver;
    private static int score;
    private static float characterSpeed;

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

    public static void SaveScore()
    {
        PlayerPrefs.SetInt("score", score);
    }

    public static int GetScoreFromSave()
    {
        return PlayerPrefs.GetInt("score");
    }

    public static int GetScore()
    {
        return score;
    }

    public static bool IsRunning()
    {
        return gameRunning;
    }

    public static bool IsGameOver()
    {
        return gameOver;
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

    // add methods for settings in here, audio etc
}
