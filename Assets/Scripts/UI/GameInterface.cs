using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits {
    public class GameInterface : Window
    {
        public GameManager gameManager;

        public GameObject topPanel;
        public GameObject bottomPanel;
        public GameObject fieldGo;

        public GameObject bestObject;
        public GameObject remainObject;

        public Button fastForwardButton;
        public Button abortTurnButton;
        public Button boosterLineButton;
        public Button boosterDamageButton;

        public Text starText;
        public Text bestText;
        public Text remainText;
        public Text scoreText;

        public RectTransform leftFrame;
        public RectTransform rightFrame;

        public RectTransform fieldFrame;
        public CameraManager cameraManager;
        public Canvas canvas;

        [SerializeField] GameObject touchAndDrag;
        public bool IsTouchAndDragActive
        {
            get => touchAndDrag.activeInHierarchy;
            set => touchAndDrag.SetActive(value);
        }


        public override void Init()
        {
            Debug.Log("[GameInterface] Init");

            boosterLineButton.onClick.AddListener(OnBoosterLineButtonClick);
            boosterDamageButton.onClick.AddListener(OnBoosterDamageButtonClick);

            gameManager.gameInfo.onStarsChange.AddListener(OnStarsChanged);
            gameManager.gameInfo.onBestChange.AddListener(OnBestChanged);
            gameManager.GetField().onScoreChange.AddListener(OnScoreChanged);
            gameManager.GetField().onLineNumberChange.AddListener(OnLineNumberChanged);
            gameManager.GetField().onRemainLinesChange.AddListener(OnRemainLinesChanged);

            float topPanelHeight = ((RectTransform)topPanel.transform).rect.height;
            float bottomPanelHeight = ((RectTransform)bottomPanel.transform).rect.height;
        }



        public void SetFrame(Rect fieldRect)
        {
            float screenWidth = cameraManager.ScreenWidth;
            float screenHeight = cameraManager.ScreenHeight;

            float canvasWidth = ((RectTransform)canvas.transform).rect.width;
            float canvasHeight = ((RectTransform)canvas.transform).rect.height;

            float scaleCoef = canvasWidth / screenWidth;

            float left = fieldRect.xMin * scaleCoef;
            float right = fieldRect.xMax * scaleCoef;
            float top = fieldRect.yMax * scaleCoef;
            float bottom = fieldRect.yMin * scaleCoef;

            fieldFrame.localPosition = new Vector2((right + left) / 2, (top + bottom) / 2);
            fieldFrame.sizeDelta = new Vector2(fieldRect.width * scaleCoef + 20, fieldRect.height * scaleCoef + 20);

            RectTransform topPanelRect = topPanel.GetComponent<RectTransform>();
            topPanelRect.sizeDelta = new Vector2(topPanelRect.sizeDelta.x, canvasHeight / 2 - top);
        }



        public float GetTopPanelHeight()
        {
            return ((RectTransform)topPanel.transform).rect.height;
        }



        public float GetBottomPanelHeight()
        {
            return ((RectTransform)bottomPanel.transform).rect.height;
        }



        public void OnPauseClick()
        {
            gameManager.GetField().SetPause(true);
            ScreenManager.Instance.pauseInterface.Open();
        }



        public override void Open()
        {
            base.Open();

            if (GameManager.gameMode == GameMode.Infinity)
            {
                bestText.text = $"Best: {gameManager.gameInfo.Best}";
                bestObject.SetActive(true);
                remainObject.SetActive(false);
            }
            else
            {
                bestObject.SetActive(false);
                remainObject.SetActive(true);
            }

            abortTurnButton.gameObject.SetActive(false);
            fastForwardButton.gameObject.SetActive(false);

            gameObject.SetActive(true);
            fieldGo.SetActive(true);
        }



        public override void Close()
        {
            base.Close();

            gameObject.SetActive(false);
            fieldGo.SetActive(false);
        }



        private void OnStarsChanged(int starsCount)
        {
            starText.text = $"{starsCount}";
        }



        private void OnBestChanged(int best)
        {
            bestText.text = $"Best: {best}";
        }



        private void OnScoreChanged(int score)
        {
            scoreText.text = $"{score}";
        }



        private void OnLineNumberChanged(int lineNumber)
        {
            scoreText.text = $"{lineNumber}";
        }



        private void OnRemainLinesChanged(int remain)
        {
            remainText.text = $"{remain}";
        }



        public void ShowAbortTurnButton(bool isShow)
        {
            abortTurnButton.gameObject.SetActive(isShow);
        }



        public void ShowFastForwardButton(bool isShow)
        {
            fastForwardButton.gameObject.SetActive(isShow);
        }



        public void SetBoostersEnable(bool enabled)
        {
            boosterLineButton.interactable = enabled;
            boosterDamageButton.interactable = enabled;
        }



        void OnBoosterLineButtonClick()
        {
            int boosterPrice = 10;
            if (gameManager.gameInfo.Stars >= boosterPrice)
            {
                gameManager.gameInfo.Stars -= boosterPrice;
                gameManager.field.BoosterLastLineExecute();
            }
            else
            {
                ScreenManager.Instance.moreStarsInterface.Open(PlacementIDs.MoreStarsBoosterId);
            }
        }



        void OnBoosterDamageButtonClick()
        {
            int boosterPrice = 20;
            if (gameManager.gameInfo.Stars >= boosterPrice)
            {
                gameManager.gameInfo.Stars -= boosterPrice;
                gameManager.field.BoosterDamageExecute();
            }
            else
            {
                ScreenManager.Instance.moreStarsInterface.Open(PlacementIDs.MoreStarsBoosterId);
            }
        }
    }
}
