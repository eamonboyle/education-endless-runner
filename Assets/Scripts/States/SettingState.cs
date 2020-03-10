using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingState
{
    public static void SetSound(bool s)
    {
        PlayerPrefs.SetInt("sound", s ? 1 : 0);
    }

    public static bool GetSound()
    {
        int s = PlayerPrefs.GetInt("sound");

        if (s == 1)
        {
            return true;
        }

        return false;
    }

    internal static void ChangeSound()
    {
        SetSound(!GetSound());
    }

    public static void SetGraphics(string g)
    {
        PlayerPrefs.SetString("graphics", g);
    }

    public static string GetGraphics()
    {
        return PlayerPrefs.GetString("graphics");
    }

    public static void ResetPlayerPrefs()
    {
        int playCount = GameState.GetPlayCount();
        bool firstPlay = GameState.IsFirstLoad();
        PlayerPrefs.DeleteAll();
        GameState.SetCharacter("boy");
        GameState.SetQuestionType("addition");
        GameState.SetPlayCount(playCount);

        if (!firstPlay)
        {
            GameState.SetFirstLoad();
        }
    }
}
