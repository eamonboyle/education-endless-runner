using MathRunner.Core;
using MathRunner.UI.Toolkit;
using UnityEngine;

/// <summary>
/// Bridge: difficulty label rendered on the Toolkit HUD.
/// </summary>
public class DifficultyIndicator : MonoBehaviour
{
    private void Update()
    {
        if (UIRouter.Instance?.Hud == null) return;
        if (!GameState.IsRunning())
        {
            UIRouter.Instance.Hud.SetDifficulty("");
            return;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        var level = DifficultyPresets.GetDifficulty();
        string label = level switch
        {
            DifficultyLevel.Easy => "EASY",
            DifficultyLevel.Hard => "HARD",
            _ => "NORMAL"
        };
        UIRouter.Instance.Hud.SetDifficulty(label);
#else
        UIRouter.Instance.Hud.SetDifficulty("");
#endif
    }
}
