using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace Manybits
{
    public class BuySkinInterface : Window
    {
        public GameManager gameManager;
        public Image animatedBallSkin;
        public GameObject goWatch;
        public GameObject goBuy;
        public Text starText;
        public Text ballNameText;

        private Skin skin;


        public void OnWatchClick()
        {
            if (UnityAdsController.Instance.IsVideoAcceptable()) 
            {
                UnityAdsController.Instance.ShowRewardedVideo(PlacementIDs.BuySkinId, OnRewarded);
            }
            else
            {
                ScreenManager.Instance.noVideoAvailableInterface.Open();
            }
        }

        void OnRewarded(PlacementIDs id, ShowResult showResult)
        {
            if (id == PlacementIDs.BuySkinId && showResult == ShowResult.Finished)
            {
                ScreenManager.Instance.shopInterface.Buy(skin.id);
                ScreenManager.Instance.shopInterface.Pick(skin.id);

                Close();
            }
        }

        public void OnBuyClick()
        {
            if (gameManager.gameInfo.Stars >= skin.price)
            {
                ScreenManager.Instance.shopInterface.Buy(skin.id);
                ScreenManager.Instance.shopInterface.Pick(skin.id);

                Close();
            }
            else
            {
                ScreenManager.Instance.moreStarsInterface.Open(PlacementIDs.MoreStarsBuyId);
            }
        }



        public void OnCloseClick()
        {
            Close();
        }



        public override void Open()
        {
            base.Open();

            int id = ScreenManager.Instance.shopInterface.selectedSkinID;
            skin = gameManager.GetSkin(id);
            animatedBallSkin.sprite = skin.sprite;
            ballNameText.text = skin.ballName;
            if (skin.status == SkinStatus.Buy)
            {
                goBuy.SetActive(true);
                goWatch.SetActive(false);
                starText.text = $"{skin.price}";
            }
            else if (skin.status == SkinStatus.Ads)
            {
                goBuy.SetActive(false);
                goWatch.SetActive(true);
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
