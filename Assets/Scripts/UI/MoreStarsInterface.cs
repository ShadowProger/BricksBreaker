using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace Manybits
{
    public class MoreStarsInterface : Window
    {
        public GameManager gameManager;
        [SerializeField] Button closeButton;
        [SerializeField] Button cancelButton;
        [SerializeField] Button watchButton;
        PlacementIDs placementId = PlacementIDs.None;



        public override void Init()
        {
            closeButton.onClick.AddListener(OnCloseClick);
            cancelButton.onClick.AddListener(OnCancelClick);
            watchButton.onClick.AddListener(OnWatchClick);
        }



        public void OnCloseClick()
        {
            Close();
        }



        public void OnCancelClick()
        {
            Close();
        }



        public void OnWatchClick()
        {
            if (UnityAdsController.Instance.IsVideoAcceptable())
            {
                UnityAdsController.Instance.ShowRewardedVideo(placementId, OnRewarded);
            }
            else
            {
                ScreenManager.Instance.noVideoAvailableInterface.Open();
            }
        }



        public void Open(PlacementIDs id)
        {
            placementId = id;
            Open();
        }



        public override void Open()
        {
            base.Open();

            gameObject.SetActive(true);
        }



        public override void Close()
        {
            base.Close();

            gameObject.SetActive(false);
        }



        void OnRewarded(PlacementIDs id, ShowResult showResult)
        {
            if (showResult == ShowResult.Finished)
            {
                if (placementId == PlacementIDs.MoreStarsContinueId)
                {
                    gameManager.gameInfo.Stars += 10;
                    gameManager.SaveGameInfo();
                }
                else if (placementId == PlacementIDs.MoreStarsBoosterId)
                {
                    gameManager.gameInfo.Stars += 10;
                    gameManager.SaveGameInfo();
                }
                else if (placementId == PlacementIDs.MoreStarsBuyId)
                {
                    gameManager.gameInfo.Stars += 10;
                    gameManager.SaveGameInfo();
                }
                Close();
            }
        }
    }
}
