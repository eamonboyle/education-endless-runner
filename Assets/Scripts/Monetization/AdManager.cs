using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    private string gameId = "3492332";
    private bool testMode = true;
    private string placementIdVideo = "video";

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("AD MANAGER LOADED");

        // // check wether to show an add
        // int gamesPlayed = GameState.GetPlayCount();

        // if (gamesPlayed % 5 == 0)
        // {
        //     if (Application.platform == RuntimePlatform.IPhonePlayer)
        //     {
        //         gameId = "3492333";
        //     }


        //     //Advertisement.AddListener(this);
        //     Advertisement.Initialize(gameId, testMode);
        //     StartCoroutine(Countdown(2));
        // }

        // GameState.IncrementPlayCount();
    }

    private IEnumerator ShowVideoAdvert()
    {
        return null;
        // while (!Advertisement.IsReady(placementIdVideo))
        // {
        //     yield return new WaitForSeconds(0.5f);
        // }
        // Advertisement.Show(placementIdVideo);
    }

    private IEnumerator Countdown(int seconds)
    {
        return null;
        // int count = seconds;

        // while (count > 0)
        // {
        //     yield return new WaitForSeconds(1);

        //     count--;
        // }

        // StartCoroutine(ShowVideoAdvert());
    }
}
