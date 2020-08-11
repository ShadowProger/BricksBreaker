using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace Manybits
{
    public class ContinueInterface : Window
    {
        public GameManager gameManager;
        [SerializeField] Button watchButton;
        [SerializeField] Text timeText;
        [SerializeField] Image lineImage;
        [SerializeField] int waitTime;
        float time;
        ContinueWindowState state;



        public override void Init()
        {
            watchButton.onClick.AddListener(OnWatchClick);
        }



        private void Update()
        {
            if (state == ContinueWindowState.Idle)
            {
                time -= Time.deltaTime;
                timeText.text = $"{Mathf.RoundToInt(time)}";
                float fillAmount = time / waitTime;
                lineImage.fillAmount = fillAmount;
            }
            if (time <= 0f)
            {
                Close();
                if (GameManager.gameMode == GameMode.Infinity)
                {
                    ScreenManager.Instance.gameOverInterface.Open();
                }
                else
                {
                    ScreenManager.Instance.unfortunatelyInterface.Open();
                }
            }
        }



        public void OnWatchClick()
        {
            if (UnityAdsController.Instance.IsVideoAcceptable())
            {
                state = ContinueWindowState.Wait;
                UnityAdsController.Instance.ShowRewardedVideo(PlacementIDs.ContinueId, OnRewarded);
            }
            else
            {
                ScreenManager.Instance.noVideoAvailableInterface.Open();
            }
        }

        void OnRewarded(PlacementIDs id, ShowResult showResult)
        {
            if (id == PlacementIDs.ContinueId && showResult == ShowResult.Finished)
            {
                Close();
                gameManager.GetField().ContinueAfterGameover();
            }
            else
            {
                state = ContinueWindowState.Idle;
            }
        }


        public override void Open()
        {
            base.Open();

            lineImage.fillAmount = 1f;
            timeText.text = $"{waitTime}";
            state = ContinueWindowState.Idle;
            time = waitTime;

            gameObject.SetActive(true);
        }



        public override void Close()
        {
            base.Close();

            gameObject.SetActive(false);
        }

        enum ContinueWindowState { Idle, Wait }
    }
}
