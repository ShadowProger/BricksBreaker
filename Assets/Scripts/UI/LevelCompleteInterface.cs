using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace Manybits
{
    public class LevelCompleteInterface : Window
    {
        public GameManager gameManager;
        public Text scoreText;
        public Text starsText;
        public Button rewardsButton;

        private int starsToGive;



        public void OnCollectClick()
        {
            Close();

            gameManager.gameInfo.Stars += starsToGive;
            gameManager.SaveGameInfo();

            ScreenManager.Instance.gameInterface.Close();
            ScreenManager.Instance.dailyChallengeInterface.Open();
        }



        public void OnRewardsClick()
        {
            if (UnityAdsController.Instance.IsVideoAcceptable())
            {
                UnityAdsController.Instance.ShowRewardedVideo(PlacementIDs.X2RewardId, OnRewarded);
            }
            else
            {
                ScreenManager.Instance.noVideoAvailableInterface.Open();
            }
        }



        void OnRewarded(PlacementIDs id, ShowResult showResult)
        {
            if (id == PlacementIDs.X2RewardId && showResult == ShowResult.Finished)
            {
                Close();

                gameManager.gameInfo.Stars += starsToGive * 2;
                gameManager.SaveGameInfo();

                ScreenManager.Instance.gameInterface.Close();
                ScreenManager.Instance.dailyChallengeInterface.Open();
            }
        }



        public override void Open()
        {
            base.Open();

            scoreText.text = $"Score: <color=#D37A00>{gameManager.GetField().Score}</color>";
            starsToGive = gameManager.giveStars ? gameManager.starsForLevel : 0;
            starsText.text = $"+ {starsToGive.ToString()}";

            rewardsButton.interactable = gameManager.giveStars;

            gameObject.SetActive(true);
        }



        public override void Close()
        {
            base.Close();

            gameObject.SetActive(false);
        }
    }
}
