using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class GDPRWindow : MonoBehaviour
    {
        static GDPRWindow instance;
        public Button playButton;
        public Button privacyButton;
        public Toggle confirmToggle;

        [HideInInspector] public bool gdprConsent;
        [SerializeField] string privacyUrl;

        static Action actionAfterClose;

        public static GDPRWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = Instantiate(Resources.Load<GameObject>("GDPRWindow"));
                    instance = obj.GetComponent<GDPRWindow>();
                    obj.SetActive(false);
                }

                return instance;
            }
        }



        private void Awake()
        {
            playButton.onClick.AddListener(OnPlayClick);
            privacyButton.onClick.AddListener(OnPrivacyClick);
            confirmToggle.onValueChanged.AddListener(OnConfirmValueChanged);
        }

        public void OnPlayClick()
        {
            Close();
            GDPR.AdsConsent = confirmToggle.isOn;
            GDPR.AnalyticsConsent = confirmToggle.isOn;
            GDPR.ConsentIsSelect = true;
            actionAfterClose?.Invoke();
            actionAfterClose = null;
        }

        public void OnPrivacyClick()
        {
            OpenPrivacyPolicy();
        }

        public void OnConfirmValueChanged(bool value)
        {
            playButton.interactable = value;
        }

        public void Show(Action callback)
        {
            actionAfterClose = callback;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            Destroy(gameObject);
        }

        public void OpenPrivacyPolicy()
        {
#if UNITY_ANDROID
            Application.OpenURL(privacyUrl);
#elif UNITY_IPHONE
			Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
#endif
        }

        private void OnDestroy()
        {
            actionAfterClose = null;
            instance = null;
        }
    }
}
