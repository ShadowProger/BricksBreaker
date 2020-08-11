using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace Manybits
{
    public class UnityAdsController : SingletonMonoBehaviour<UnityAdsController>, IUnityAdsListener
    {
        public string gameId = "3538726";
        public string interstitialId = "video";
        public string rewardedId = "rewardedVideo";
        public bool testMode;

        public int Interstitialfrequency;
        private int InterstitialCounter;

        UnityAction<PlacementIDs, ShowResult> lastCallback = null;
        PlacementIDs lastId;

        void Start()
        {
            Advertisement.AddListener(this);
            Advertisement.Initialize(gameId, testMode);
            var consent = GDPR.AdsConsent;

            Debug.Log($"Unity Ads Init: consent = {consent}");

            MetaData gdprMetaData = new MetaData("gdpr");
            gdprMetaData.Set("consent", consent.ToString().ToLower());
            Advertisement.SetMetaData(gdprMetaData);
        }

        public bool IsVideoAcceptable()
        {
            return Advertisement.isInitialized && Advertisement.IsReady(rewardedId);
        }

        public bool ShowInterstitial()
        {
            Debug.Log($"Show Ad: Interstitial");

            bool result = false;
            InterstitialCounter--;
            if (Advertisement.IsReady(interstitialId) && InterstitialCounter <= 0)
            {
                Advertisement.Show(interstitialId);
                InterstitialCounter = Interstitialfrequency;
                result = true;
            }

            return result;
        }

        public bool ShowRewardedVideo(PlacementIDs id,  UnityAction<PlacementIDs, ShowResult> callback = null)
        {
            Debug.Log($"Show Ad: RewardedVideo");

            bool result = false;
            if (Advertisement.IsReady(rewardedId))
            {
                Advertisement.Show(rewardedId);
                lastId = id;
                lastCallback = callback;
                result = true;
            }

            return result;
        }

        public void OnUnityAdsDidError(string message)
        {
            
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            if (placementId == rewardedId)
            {
                InvokeCallback(lastId, showResult);
            }
        }

        public void OnUnityAdsDidStart(string placementId)
        {
            
        }

        public void OnUnityAdsReady(string placementId)
        {
            
        }

        void InvokeCallback(PlacementIDs id, ShowResult showResult)
        {
            lastCallback?.Invoke(lastId, showResult);
            lastCallback = null;
            lastId = PlacementIDs.None;
        }
    }
}
