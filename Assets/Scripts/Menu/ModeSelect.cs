using UnityEngine;
using MathRunner.Core;

public class ModeSelect : MonoBehaviour
{
    public GameObject homeButton;

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

    public void Choose(string mode)
    {
        GameState.SetQuestionType(mode);

        // Choosing a classic math mode clears time-attack unless already
        // explicitly set via ModeSelectExtras; campaign stays if active.
        if (PlayerPrefs.GetInt(MathRunner.Core.GameConstants.PREF_CAMPAIGN_ACTIVE, 0) == 0)
        {
            // leave time-attack flag as set by extras UI
        }

        if (GameState.IsFirstLoad())
        {
            GameManager.instance.LoadTutorial();
        }
        else
        {
            GameManager.instance.LoadMainMenu();
        }
    }

    /// <summary>Enables time-attack and returns to main menu flow.</summary>
    public void EnableTimeAttack()
    {
        TimeAttackMode.SetTimeAttack(true);
        PlayerPrefs.SetInt(MathRunner.Core.GameConstants.PREF_CAMPAIGN_ACTIVE, 0);
    }

    /// <summary>Starts campaign mode for the current unlocked level.</summary>
    public void EnableCampaign()
    {
        TimeAttackMode.SetTimeAttack(false);
        int level = CampaignManager.GetCurrentLevel();
        PlayerPrefs.SetInt(MathRunner.Core.GameConstants.PREF_CAMPAIGN_ACTIVE, 1);
        PlayerPrefs.SetInt(MathRunner.Core.GameConstants.PREF_CAMPAIGN_LEVEL, level);
        var config = CampaignManager.GetLevelConfig(level);
        GameState.SetQuestionType(config.MathMode.ToPlayerPrefsString());
    }
}