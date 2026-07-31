using System.Collections;
using UnityEngine;

/// <summary>
/// Ad manager stub. Unity Ads integration is currently disabled.
/// Re-enable by uncommenting the ad initialization and show calls
/// and adding the IUnityAdsListener interface.
/// </summary>
public class AdManager : MonoBehaviour
{
#pragma warning disable CS0414 // Reserved for when Unity Ads integration is re-enabled.
    [SerializeField] private string gameId = "3492402";
    [SerializeField] private bool testMode = false;
    [SerializeField] private string placementIdVideo = "rewardedVideo";
#pragma warning restore CS0414

    public void ShowVideoAdvert()
    {
        Debug.Log("AdManager: Ads are currently disabled.");
    }
}
