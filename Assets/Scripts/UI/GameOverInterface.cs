using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class GameOverInterface : Window
    {
        public GameManager gameManager;



        public void OnContinueClick()
        {
            if (gameManager.gameInfo.Stars >= gameManager.starsForContinue)
            {
                gameManager.gameInfo.Stars -= gameManager.starsForContinue;
                Close();
                gameManager.GetField().ContinueAfterGameover();
            }
            else
            {
                ScreenManager.Instance.moreStarsInterface.Open(PlacementIDs.MoreStarsContinueId);
            }
        }



        public void OnHomeClick()
        {
            Close();

            UnityAdsController.Instance.ShowInterstitial();

            ScreenManager.Instance.gameInterface.Close();

            gameManager.QuitWithoutSave();
            gameManager.GetField().RemoveAllBlocks();

            ScreenManager.Instance.menuInterface.Open();
        }



        public void OnRetryClick()
        {
            Close();

            UnityAdsController.Instance.ShowInterstitial();

            int checkpoint = gameManager.gameInfo.checkpoint;
            gameManager.gameInfo.checkpoint = 1;
            gameManager.GetField().RemoveAllBlocks();
            gameManager.GetField().StartInfinityGame(checkpoint);
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
    }
}
