using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public GameObject scoreObject;

    private float timePassed = 0.0f;
    private int score;

    void Start()
    {
        score = GameState.GetScore();
        scoreObject.GetComponent<Text>().text = score.ToString();
    }

    void Update()
    {
        if (!GameState.IsRunning())
        {
            return;
        }

        timePassed += Time.deltaTime;
        if (timePassed >= 1)
        {
            score += (int)timePassed;
            timePassed -= (int)timePassed;
        }

        scoreObject.GetComponent<Text>().text = score.ToString();
        GameState.SetScore(score);
    }
}
