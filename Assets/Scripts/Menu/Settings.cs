using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject soundImage;
    public GameObject resetOverlay;
    public GameObject graphicTextObj;
    public GameObject leftArrow;
    public GameObject rightArrow;

    public Sprite soundOnImg;
    public Sprite soundOffImg;

    private bool sound;
    private List<string> graphicNames = new List<string>();
    private string currentGraphicSettings;

    void Start()
    {
        DisplaySoundSetting();
        GetGraphicSettings();
    }

    private void GetGraphicSettings()
    {
        currentGraphicSettings = SettingState.GetGraphics();

        if (currentGraphicSettings == string.Empty || currentGraphicSettings == null) 
        {
            currentGraphicSettings = "Medium";
        }

        string[] gNames = QualitySettings.names;

        foreach (string graphicName in gNames)
        {
            graphicNames.Add(graphicName);
        }

        graphicTextObj.GetComponent<Text>().text = currentGraphicSettings;

        // make sure can't go out of bounds with arrows
        // hide if on first / last
    }

    public void ChangeGraphics(bool increase)
    {
        int index = graphicNames.IndexOf(currentGraphicSettings);

        if (increase)
        {
            // make sure left arrow is showing
            leftArrow.SetActive(true);

            // increase the graphics
            QualitySettings.SetQualityLevel(index + 1);
            currentGraphicSettings = graphicNames[index + 1];

            // if on the highest
            if (index + 1 == graphicNames.Count - 1)
            {
                // hide the arrow
                rightArrow.SetActive(false);
            }
        }
        else
        {
            // make sure right arrow is now active
            rightArrow.SetActive(true);

            // decrease the graphics
            QualitySettings.SetQualityLevel(index - 1);
            currentGraphicSettings = graphicNames[index - 1];

            // if on the lowest
            if (index - 1 == 0)
            { 
                // hide the arrow
                leftArrow.SetActive(false);
            }
        }

        graphicTextObj.GetComponent<Text>().text = currentGraphicSettings;
        SettingState.SetGraphics(currentGraphicSettings);
    }

    private void DisplaySoundSetting()
    {
        sound = SettingState.GetSound();

        if (sound)
        {
            soundImage.GetComponent<Image>().sprite = soundOnImg;
        }
        else
        {
            soundImage.GetComponent<Image>().sprite = soundOffImg;
        }
    }

    public void ClickSound()
    {
        SettingState.SetSound(!sound);
        DisplaySoundSetting();
    }

    public void ResetUI(bool show)
    {
        resetOverlay.GetComponent<Canvas>().enabled = show;
    }

    public void ResetPlayerPrefs()
    {
        SettingState.ResetPlayerPrefs();
        ResetUI(false);
    }
}
