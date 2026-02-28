using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathRunner.Core;

public static class SettingState
{
    public static void SetSound(bool s)
    {
        PlayerPrefs.SetInt(GameConstants.PREF_SOUND, s ? 1 : 0);
    }

    public static bool GetSound()
    {
        if (GameState.IsFirstLoad())
        {
            return true;
        }

        return PlayerPrefs.GetInt(GameConstants.PREF_SOUND) != 0;
    }

    internal static void ChangeSound()
    {
        SetSound(!GetSound());
    }

    public static void SetGraphics(string g)
    {
        PlayerPrefs.SetString(GameConstants.PREF_GRAPHICS, g);
    }

    public static string GetGraphics()
    {
        return PlayerPrefs.GetString(GameConstants.PREF_GRAPHICS);
    }

    public static void ResetPlayerPrefs()
    {
        int playCount = GameState.GetPlayCount();
        bool firstPlay = GameState.IsFirstLoad();
        PlayerPrefs.DeleteAll();
        GameState.SetCharacter("boy");
        GameState.SetQuestionType("addition");
        GameState.SetPlayCount(playCount);
        GameState.ResetAdCount();

        if (!firstPlay)
        {
            GameState.SetFirstLoad();
        }
    }
}
