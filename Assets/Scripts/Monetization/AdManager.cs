using System.Collections;
using UnityEngine;

/// <summary>
/// Ad manager stub. Unity Ads integration is currently disabled.
/// Re-enable by uncommenting the ad initialization and show calls
/// and adding the IUnityAdsListener interface.
/// </summary>
public class AdManager : MonoBehaviour
{
    [SerializeField] private string gameId = "3492402";
    [SerializeField] private bool testMode = false;
    [SerializeField] private string placementIdVideo = "rewardedVideo";

    public void ShowVideoAdvert()
    {
        Debug.Log("AdManager: Ads are currently disabled.");
    }
}
