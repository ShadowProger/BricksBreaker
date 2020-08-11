using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class UnfortunatelyInterface : Window
    {
        public GameManager gameManager;



        public void OnRetryClick()
        {
            Close();

            gameManager.SaveGameInfo();

            UnityAdsController.Instance.ShowInterstitial();

            gameManager.GetField().RemoveAllBlocks();
            gameManager.GetField().StartDailyChallenge(gameManager.selectedDay);
        }



        public void OnQuitClick()
        {
            Close();

            gameManager.GetField().RemoveAllBlocks();

            UnityAdsController.Instance.ShowInterstitial();

            ScreenManager.Instance.gameInterface.Close();
            ScreenManager.Instance.dailyChallengeInterface.Open();
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
