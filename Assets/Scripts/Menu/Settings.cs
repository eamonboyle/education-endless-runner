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

        if (string.IsNullOrEmpty(currentGraphicSettings))
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
        if (index < 0) return;

        if (increase && index < graphicNames.Count - 1)
        {
            leftArrow.SetActive(true);
            index++;
            QualitySettings.SetQualityLevel(index);
            currentGraphicSettings = graphicNames[index];

            if (index == graphicNames.Count - 1)
                rightArrow.SetActive(false);
        }
        else if (!increase && index > 0)
        {
            rightArrow.SetActive(true);
            index--;
            QualitySettings.SetQualityLevel(index);
            currentGraphicSettings = graphicNames[index];

            if (index == 0)
                leftArrow.SetActive(false);
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
            AudioListener.volume = 1;
        }
        else
        {
            soundImage.GetComponent<Image>().sprite = soundOffImg;
            AudioListener.volume = 0;
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
