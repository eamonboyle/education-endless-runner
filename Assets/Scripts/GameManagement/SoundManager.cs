using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        bool playSound = SettingState.GetSound();

        if (!playSound)
        {
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.volume = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
