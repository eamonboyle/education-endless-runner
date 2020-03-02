using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    private string gameId = "3492332";
    private bool testMode = true;
    private string placementIdReward = "rewardedVideo";
    private string placementIdVideo = "video";

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("AD ERROR");
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
            Debug.Log("FINISHED AD");
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
            Debug.Log("SKIPPED ADD");
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("STARTED");
    }

    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("AD READY");
        if (placementId == placementIdVideo)
        {
            Advertisement.Show(placementIdVideo);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            gameId = "3492333";
        }

        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
