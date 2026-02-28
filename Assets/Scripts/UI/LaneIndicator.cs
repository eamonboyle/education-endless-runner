using UnityEngine;

/// <summary>
/// Attaches to the player GameObject and displays a subtle lane indicator
/// at the bottom of the screen via OnGUI.
/// </summary>
public class LaneIndicator : MonoBehaviour
{
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnGUI()
    {
        if (playerMovement == null) return;
        if (!GameState.IsRunning()) return;

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 22,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        style.normal.textColor = new Color(1f, 1f, 1f, 0.35f);

        float width = 200f;
        float height = 36f;
        Rect rect = new Rect(
            (Screen.width - width) / 2f,
            Screen.height - height - 20f,
            width,
            height
        );

        string label;
        switch (playerMovement.currentLane)
        {
            case PlayerMovement.Lane.Left:
                label = "\u25C4 LEFT";
                break;
            case PlayerMovement.Lane.Right:
                label = "RIGHT \u25BA";
                break;
            default:
                label = "CENTER";
                break;
        }

        GUI.Label(rect, label, style);
    }
}
