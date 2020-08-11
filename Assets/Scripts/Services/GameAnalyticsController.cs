using GameAnalyticsSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class GameAnalyticsController : SingletonMonoBehaviour<GameAnalyticsController>
    {
        void Awake()
        {
            Debug.Log("[GameAnalyticsController] Initialize");
            GameAnalytics.Initialize();
        }

        public void RewardedFinishEvent(string placement)
        {
            GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, "UnityAds", placement);
        }

        public void ShowInterstitialEvent()
        {
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, "UnityAds", "StandardPlacement");
        }

        public void CustomEvent(string eventName, float eventValue)
        {
            GameAnalytics.NewDesignEvent(eventName, eventValue);
        }
    }
}
