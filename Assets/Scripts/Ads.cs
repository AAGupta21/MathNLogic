using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class Ads : MonoBehaviour
{
    private InterstitialAd interstitial;

    // Start is called before the first frame update
    void Awake()
    {
        string appId = "ca-app-pub-4637006073864033~8557462667";
        MobileAds.Initialize(appId);
    }

    private void Start()
    {
        string adUnitId = "ca-app-pub-4637006073864033/4267574303";
        this.interstitial = new InterstitialAd(adUnitId);
    }

    public void ShowAds()
    {
        if (interstitial.IsLoaded())
            interstitial.Show();
    }
}