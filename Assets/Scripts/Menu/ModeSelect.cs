using UnityEngine;
using MathRunner.Core;

public class ModeSelect : MonoBehaviour
{
    public GameObject homeButton;
    public PlayStylePanel playStylePanel;

    private void Start()
    {
        if (GameState.IsFirstLoad())
        {
            homeButton.SetActive(false);
        }
        else
        {
            homeButton.SetActive(true);
        }
    }

    /// <summary>
    /// Wired to the four question-type buttons in the ModeChoice scene.
    /// </summary>
    public void Choose(string mode)
    {
        GameState.SetQuestionType(mode);

        // First-timers go straight to the tutorial on Classic defaults; asking a
        // new player about Time Attack or Campaign before they have run once is
        // noise they cannot make sense of yet.
        if (GameState.IsFirstLoad())
        {
            TimeAttackMode.SetTimeAttack(false);
            PlayerPrefs.SetInt(MathRunner.Core.GameConstants.PREF_CAMPAIGN_ACTIVE, 0);
            GameManager.instance.LoadTutorial();
            return;
        }

        if (playStylePanel != null)
        {
            playStylePanel.Show(mode);
            return;
        }

        GameManager.instance.LoadMainMenu();
    }
}