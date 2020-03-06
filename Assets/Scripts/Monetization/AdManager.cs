using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    private string gameId = "3492332";
    private bool testMode = true;
    //private string placementIdReward = "rewardedVideo";
    private string placementIdVideo = "video";

    //public void OnUnityAdsDidError(string message)
    //{
    //    Debug.Log("AD ERROR");
    //}

    //public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    //{
    //    // Define conditional logic for each ad completion status:
    //    if (showResult == ShowResult.Finished)
    //    {
    //        // Reward the user for watching the ad to completion.
    //        Debug.Log("FINISHED AD");
    //        //Advertisement.RemoveListener(this);
    //    }
    //    else if (showResult == ShowResult.Skipped)
    //    {
    //        // Do not reward the user for skipping the ad.
    //        Debug.Log("SKIPPED ADD");
    //        //Advertisement.RemoveListener(this);
    //    }
    //    else if (showResult == ShowResult.Failed)
    //    {
    //        Debug.LogWarning("The ad did not finish due to an error.");
    //    }
    //}

    //public void OnUnityAdsDidStart(string placementId)
    //{
    //    Debug.Log("STARTED");
    //}

    //public void OnUnityAdsReady(string placementId)
    //{
    //    Debug.Log("AD READY");
    //    if (placementId == placementIdVideo)
    //    {
    //        Advertisement.Show(placementIdVideo);
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("AD MANAGER LOADED");

        // check wether to show an add
        int gamesPlayed = PlayerPrefs.GetInt("gamesPlayed");

        if (gamesPlayed % 5 == 0)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                gameId = "3492333";
            }


            //Advertisement.AddListener(this);
            Advertisement.Initialize(gameId, testMode);
            StartCoroutine(Countdown(2));
        }

        PlayerPrefs.SetInt("gamesPlayed", ++gamesPlayed);
    }

    private IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(placementIdVideo))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Show(placementIdVideo);
    }

    private IEnumerator Countdown(int seconds)
    {
        int count = seconds;

        while (count > 0)
        {
            yield return new WaitForSeconds(1);

            count--;
        }

        StartCoroutine(ShowBannerWhenReady());
    }
}
