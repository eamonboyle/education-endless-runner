using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private float loadTime;
    private float minimumSplashTime = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = FindObjectOfType<CanvasGroup>();

        canvasGroup.alpha = 1;

        if (Time.time < minimumSplashTime)
        {
            loadTime = minimumSplashTime;
        }
        else
        {
            loadTime = Time.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // fade in
        if (Time.time < minimumSplashTime)
        {
            canvasGroup.alpha = 1 - Time.time;
        }

        // fade out
        if (Time.time > minimumSplashTime && loadTime != 0)
        {
            canvasGroup.alpha = Time.time - minimumSplashTime;

            if (canvasGroup.alpha >= 1)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
