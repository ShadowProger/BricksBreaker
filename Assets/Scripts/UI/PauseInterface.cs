using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class PauseInterface : Window
    {
        public GameManager gameManager;
        public GameObject homeButton;



        public void OnHomeClick()
        {
            Close();

            gameManager.SaveGameInfo();
            gameManager.GetField().RemoveAllBlocks();

            UnityAdsController.Instance.ShowInterstitial();

            ScreenManager.Instance.gameInterface.Close();
            ScreenManager.Instance.menuInterface.Open();
        }



        public void OnQuitClick()
        {
            Close();

            gameManager.GetField().RemoveAllBlocks();

            UnityAdsController.Instance.ShowInterstitial();

            ScreenManager.Instance.gameInterface.Close();

            if (GameManager.gameMode == GameMode.Infinity)
            {
                gameManager.QuitWithoutSave();
                ScreenManager.Instance.menuInterface.Open();
            }
            else
            {
                gameManager.SaveGameInfo();
                ScreenManager.Instance.dailyChallengeInterface.Open();
            }
        }



        public void OnCloseClick()
        {
            Close();

            gameManager.GetField().SetPause(false);
        }



        public override void Open()
        {
            base.Open();

            if (GameManager.gameMode == GameMode.Infinity)
            {
                homeButton.SetActive(true);
            }
            else
            {
                homeButton.SetActive(false);
            }

            gameObject.SetActive(true);
        }



        public override void Close()
        {
            base.Close();

            gameObject.SetActive(false);
        }
    }
}
