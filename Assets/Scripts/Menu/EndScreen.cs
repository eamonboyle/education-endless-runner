using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour, IUnityAdsListener
{
    public GameObject continueButton;
    public GameObject countdownText;
    public GameObject questionText;
    public GameObject mainCamera;

    public string gameId = "3492402";

#if UNITY_IOS
    public string gameId = "3492403";
#endif

    public bool testMode = false;
    public string myPlacementId = "rewardedVideo";

    private Button myButton;

    private void Start()
    {
        myButton = continueButton.GetComponent<Button>();

        // Initialize the Ads listener and service:
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
    }

    private void Update()
    {
        if (GameState.GetAdCount() <= 0)
        {
            myButton.interactable = false;
        }
        else
        {
            myButton.interactable = Advertisement.IsReady(myPlacementId);
        }
    }

    private void OnDestroy()
    {
        Advertisement.RemoveListener(this);
    }

    public void GameOverUIButtonClick(string action)
    {
        switch (action)
        {
            case "restart":
                GameState.Init();
                GameManager.instance.LoadGame();
                break;

            case "continue":
                ShowContinueAd();
                break;

            case "share":
                GameManager.instance.Screenshot();
                break;

            case "quit":
                GameManager.instance.LoadMainMenu();
                break;

            default:
                break;
        }
    }

    private void ShowContinueAd()
    {
        Advertisement.Show(myPlacementId);
        GameState.DecreaseAdCount();
    }

    private void ContinueGame()
    {
        GameState.ShowGameUI();
        StartCoroutine(Countdown(4));
    }

    private IEnumerator Countdown(int seconds)
    {
        int count = seconds;
        countdownText.SetActive(true);

        while (count > 0)
        {
            if (count == 1)
            {
                countdownText.GetComponent<Text>().text = "GO!";
            }
            else
            {
                countdownText.GetComponent<Text>().text = (count - 1).ToString();
            }

            // TODO: Change this sound and try put an animation on the text
            mainCamera.GetComponent<AudioSource>().Play();

            yield return new WaitForSeconds(1);

            count--;
        }

        StartGame();
    }

    private void StartGame()
    {
        GameState.QuestionBoxShow(true);
        countdownText.SetActive(false);
        questionText.SetActive(true);
        gameObject.GetComponent<Canvas>().enabled = false;
        GameObject.Find("PlayerObject").GetComponent<Animator>().SetBool("isRunning", true);
        GameState.SetRunning(true);
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log error
        Debug.LogError("AD DIDN'T START");
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
            //Debug.Log("Ad Finished, reward with continue");
            //Advertisement.RemoveListener(this);
            ContinueGame();
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        //Debug.Log("Ad started");
    }

    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, show the ad:
        if (placementId == myPlacementId)
        {
            //Advertisement.Show(myPlacementId);
            myButton.interactable = true;
        }
    }
}