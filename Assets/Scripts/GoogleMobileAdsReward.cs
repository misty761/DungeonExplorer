using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class GoogleMobileAdsReward : MonoBehaviour
{
    public bool isRewarded;
    public bool bCloseAD;
    public bool isAdFailedToLoad;
    public bool isAdFailedToShow;

    public RewardedAd rewardedAd;

    // Start is called before the first frame update
    void Start()
    {
        MyLoadAD();
    }

    public void MyLoadAD()
    {
        string adUnitId;
#if UNITY_ANDROID
                    adUnitId = "ca-app-pub-3940256099942544/5224354917";  // test ID

#elif UNITY_IPHONE
                    adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
                    adUnitId = "unexpected_platform";
#endif

        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);

        isRewarded = false;
        bCloseAD = false;
        isAdFailedToLoad = false;
        isAdFailedToShow = false;
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        isAdFailedToLoad = true;

        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        // MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        isAdFailedToShow = true;

        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);


    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        bCloseAD = true;
        if (!isRewarded)
        {
            MyLoadAD();
        }
        //MonoBehaviour.print("HandleRewardedAdClosed event received");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        isRewarded = true;

        //MonoBehaviour.print("HandleRewardedAdRewarded event received");
    }

    public void UserChoseToWatchAd()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
    }
}
