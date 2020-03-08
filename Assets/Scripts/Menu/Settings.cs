using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject soundImage;
    public Sprite soundOnImg;
    public Sprite soundOffImg;

    private bool sound;

    void Start()
    {
        DisplaySoundSetting();
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
}
