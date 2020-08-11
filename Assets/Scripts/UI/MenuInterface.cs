using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class MenuInterface : Window
    {
        public GameManager gameManager;
        public Text checkpointText;
        public Button shareButton;
        public Button privacyPolicyButton;
        public Button settingsButton;



        public override void Init()
        {
            shareButton.onClick.AddListener(ShareAndRate.Instance.OnAndroidTextSharingClick);
            privacyPolicyButton.onClick.AddListener(ShareAndRate.Instance.OpenPrivacyPolicy);
            settingsButton.onClick.AddListener(OnSettingsClick);

            if (gameManager.gameInfo.checkpoint > 1)
            {
                checkpointText.text = $"Level {gameManager.gameInfo.checkpoint} Checkpoint";
                checkpointText.gameObject.SetActive(true);
            }
            else
            {
                checkpointText.gameObject.SetActive(false);
            }
                
        }



        public void OnSettingsClick() 
        {
            ScreenManager.Instance.settingsInterface.Open();
        }



        public void OnPlayButtonClick()
        {
            Close();
            gameManager.StartInfinityGame();

            ScreenManager.Instance.gameInterface.Open();
        }



        public void OnDailyChallengeButtonClick()
        {
            Close();
            ScreenManager.Instance.dailyChallengeInterface.Open();
        }



        public void OnShopButtonClick()
        {
            Close();
            ScreenManager.Instance.shopInterface.Open();
        }



        public override void Open()
        {
            base.Open();

            if (gameManager.gameInfo.checkpoint > 1)
            {
                checkpointText.text = $"Level {gameManager.gameInfo.checkpoint} Checkpoint";
                checkpointText.gameObject.SetActive(true);
            }
            else
            {
                checkpointText.gameObject.SetActive(false);
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
