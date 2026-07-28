using MathRunner.UI.Toolkit;
using UnityEngine;

/// <summary>
/// Bridge: shows Toolkit celebration when <see cref="GameState.OnNewHighScore"/> fires.
/// </summary>
public class HighScoreCelebration : MonoBehaviour
{
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
        UIRouter.Instance?.ShowHighScoreCelebration();
    }
}
