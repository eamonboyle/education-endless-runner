using UnityEngine;

/// <summary>
/// Displays a small colour-coded difficulty tier label in the bottom-left
/// corner during gameplay, based on the player's current score.
/// Uses OnGUI for rendering — no scene setup required.
/// </summary>
public class DifficultyIndicator : MonoBehaviour
{
    private struct Tier
    {
        public string Label;
        public Color Color;
        public int MinScore;

        public Tier(string label, Color color, int minScore)
        {
            Label = label;
            Color = color;
            MinScore = minScore;
        }
    }

    private static readonly Tier[] Tiers =
    {
        new Tier("Insane", Color.red,    600),
        new Tier("Hard",   new Color(1f, 0.5f, 0f), 300),
        new Tier("Medium", Color.yellow, 100),
        new Tier("Easy",   Color.green,  0)
    };

    private void OnGUI()
    {
        if (!GameState.IsRunning()) return;

        int score = GameState.GetScore();
        Tier current = Tiers[Tiers.Length - 1];
        for (int i = 0; i < Tiers.Length; i++)
        {
            if (score >= Tiers[i].MinScore)
            {
                current = Tiers[i];
                break;
            }
        }

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.Max(28, Mathf.RoundToInt(Screen.height * 0.028f)),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.LowerLeft
        };
        style.normal.textColor = current.Color;

        float padding = 24f;
        float height = style.fontSize + 12f;
        GUI.Label(
            new Rect(padding, Screen.height - height - padding, 220f, height),
            current.Label, style);
    }
}
