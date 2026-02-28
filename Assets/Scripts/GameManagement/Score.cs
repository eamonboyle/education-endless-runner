using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public GameObject scoreObject;

    private float timePassed = 0.0f;
    private int score;
    private Text scoreText;

    void Start()
    {
        score = GameState.GetScore();

        if (scoreObject != null)
        {
            scoreText = scoreObject.GetComponent<Text>();
            if (scoreText != null)
                scoreText.text = score.ToString();
        }
    }

    void Update()
    {
        if (!GameState.IsRunning())
        {
            return;
        }

        timePassed += Time.deltaTime;
        if (timePassed >= 1f)
        {
            int timePoints = 1;

            var powerUp = PowerUpSystem.Instance;
            if (powerUp != null && powerUp.HasActivePowerUp(PowerUpType.DoublePoints))
            {
                timePoints *= 2;
            }

            score += timePoints;
            timePassed -= 1f;
        }

        if (scoreText != null)
            scoreText.text = score.ToString();

        GameState.SetScore(score);
    }
}
