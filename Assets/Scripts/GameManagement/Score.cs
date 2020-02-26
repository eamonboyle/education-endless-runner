using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public GameObject scoreObject;
    private float timePassed = 0.0f;
    private int score = 1;

    // Start is called before the first frame update
    void Start()
    {
        scoreObject.GetComponent<TextMeshProUGUI>().text = "Score: " + score;
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= 1)
        {
            score += (int)timePassed;
            timePassed -= (int)timePassed;
        }

        scoreObject.GetComponent<TextMeshProUGUI>().text = "Score: " + score;
        PlayerPrefs.SetInt("score", score);
    }
}
