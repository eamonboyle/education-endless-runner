using MathRunner.UI.Toolkit;
using UnityEngine;

/// <summary>
/// Bridge: lane label on Toolkit HUD (LEFT / CENTER / RIGHT).
/// </summary>
public class LaneIndicator : MonoBehaviour
{
    private void Update()
    {
        var hud = UIRouter.Instance?.Hud;
        if (hud == null) return;
        if (!GameState.IsRunning())
        {
            hud.SetLane("");
            return;
        }

        var player = GameObject.Find("PlayerObject");
        if (player == null) return;

        float x = player.transform.position.x;
        string lane = x < -1f ? "LEFT" : x > 1f ? "RIGHT" : "CENTER";
        hud.SetLane(lane);
    }
}
