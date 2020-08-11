using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class SettingsInterface : Window
    {
        public GameManager gameManager;
        public Toggle adsToggle;
        public Toggle analytinsToggle;
        public Button playButton;



        public override void Init()
        {
            playButton.onClick.AddListener(OnPlayClick);
            adsToggle.isOn = GDPR.AdsConsent;
            analytinsToggle.isOn = GDPR.AnalyticsConsent;
        }



        public void OnPlayClick()
        {
            GDPR.AdsConsent = adsToggle.isOn;
            GDPR.AnalyticsConsent = analytinsToggle.isOn;
            Close();
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
