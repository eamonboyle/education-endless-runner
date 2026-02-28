using UnityEngine;

/// <summary>
/// Displays a flashing "NEW HIGH SCORE!" celebration when
/// <see cref="GameState.OnNewHighScore"/> fires.
/// </summary>
public class HighScoreCelebration : MonoBehaviour
{
    private bool showCelebration;
    private float celebrationTimer;
    private const float CelebrationDuration = 3f;

    private void OnEnable()
    {
        GameState.OnNewHighScore += OnNewHighScore;
    }

    private void OnDisable()
    {
        GameState.OnNewHighScore -= OnNewHighScore;
    }

    private void OnNewHighScore()
    {
        showCelebration = true;
        celebrationTimer = CelebrationDuration;
    }

    private void Update()
    {
        if (!showCelebration) return;

        celebrationTimer -= Time.deltaTime;
        if (celebrationTimer <= 0f)
        {
            showCelebration = false;
        }
    }

    private void OnGUI()
    {
        if (!showCelebration) return;

        bool useGold = Mathf.FloorToInt(Time.time * 4f) % 2 == 0;
        Color textColor = useGold ? new Color(1f, 0.84f, 0f) : Color.white;

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 52,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        style.normal.textColor = textColor;

        float width = 600f;
        float height = 80f;
        Rect rect = new Rect(
            (Screen.width - width) / 2f,
            Screen.height * 0.25f,
            width,
            height
        );

        GUI.Label(rect, "NEW HIGH SCORE!", style);
    }
}
