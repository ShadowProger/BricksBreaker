using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace Manybits
{
    public class DailyChallengeInterface : Window
    {
        public GameManager gameManager;
        public Text monthText;
        public Text starText;
        public Button playButton;
        public Image playButtonImage;
        public Text playButtonText;
        public GameObject videoImage;
        public Button nextMonthButton;
        public Button prevMonthButton;
        public Calendar calendar;

        public Sprite buttonGreen;
        public Sprite buttonRed;
        public Sprite buttonBlue;
        public Sprite buttonGray;

        private string[] monthNames = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private DayButton selectedButton = null;
        private int month;
        private int year;
        private int disp;
        bool isPlayAfterVideo = false;

        

        public override void Init()
        {
            playButton.onClick.AddListener(OnPlayButtonClick);

            gameManager.gameInfo.onStarsChange.AddListener(OnStarsChanged);
        }



        public void OnPlayButtonClick()
        {
            if (isPlayAfterVideo)
            {
                if (UnityAdsController.Instance.IsVideoAcceptable())
                {
                    UnityAdsController.Instance.ShowRewardedVideo(PlacementIDs.PlayId, OnRewarded);
                }
                else
                {
                    ScreenManager.Instance.noVideoAvailableInterface.Open();
                }
            }
            else
            {
                this.Close();

                gameManager.giveStars = !gameManager.IsDayComplete(gameManager.selectedDay);

                gameManager.StartDailyChallenge(gameManager.selectedDay);
                ScreenManager.Instance.gameInterface.Open();
            }
        }



        void OnRewarded(PlacementIDs id, ShowResult showResult)
        {
            if (id == PlacementIDs.PlayId && showResult == ShowResult.Finished)
            {
                this.Close();

                gameManager.giveStars = !gameManager.IsDayComplete(gameManager.selectedDay);

                gameManager.StartDailyChallenge(gameManager.selectedDay);
                ScreenManager.Instance.gameInterface.Open();
            }
        }



        public void OnBackButtonClick()
        {
            this.Close();
            ScreenManager.Instance.menuInterface.Open();
        }



        public void OnPrevMonthButtonClick()
        {
            this.disp--;
            this.month--;
            if (this.month < 1)
            {
                this.month = 12;
                this.year--;
            }
            if (this.disp < -1)
            {
                this.prevMonthButton.interactable = false;
            }
            this.nextMonthButton.interactable = true;
            this.monthText.text = monthNames[this.month - 1];
            this.calendar.GoPrev();
            SelectDay(Day.empty);
            selectedButton = null;
        }



        public void OnNextMonthButtonClick()
        {
            this.disp++;
            this.month++;
            if (this.month > 12)
            {
                this.month = 1;
                this.year++;
            }
            if (this.disp > -1)
            {
                this.nextMonthButton.interactable = false;
            }
            this.prevMonthButton.interactable = true;
            this.monthText.text = monthNames[this.month - 1];
            this.calendar.GoNext();
            SelectDay(Day.empty);
            selectedButton = null;
        }



        public override void Open()
        {
            base.Open();

            this.month = gameManager.today.Month;
            this.year = gameManager.today.Year;
            this.disp = 0;
            this.nextMonthButton.interactable = false;
            this.prevMonthButton.interactable = true;
            DayButton todayButton = this.calendar.Init(gameManager.today);
            this.monthText.text = monthNames[this.month - 1];
            if (todayButton != null)
            {
                SelectDay(todayButton.GetDay());
                SelectButton(todayButton);
            }
            this.gameObject.SetActive(true);
        }



        public void SelectDay(Day day)
        {
            isPlayAfterVideo = false;
            int id = day.id;
            videoImage.SetActive(false);

            if (id == -1)
            {
                gameManager.selectedDay = -1;
                playButton.interactable = false;
                playButtonImage.sprite = buttonGray;
            }
            else
            {
                gameManager.selectedDay = id;
                playButton.interactable = true;

                if (day.id == gameManager.today.ToInt())
                {
                    playButtonImage.sprite = buttonGreen;
                }
                else
                {
                    playButtonImage.sprite = buttonBlue;
                    if (!day.isComplited)
                    {
                        isPlayAfterVideo = true;
                        videoImage.SetActive(true);
                    }
                }
            }

            if (day.isComplited)
            {
                playButtonText.text = "Replay";
                playButtonImage.sprite = buttonRed;
            }
            else
            {
                playButtonText.text = "Play";
            }
        }



        public void SelectButton(DayButton dayButton)
        {
            if (selectedButton != null)
            {
                selectedButton.Unselect();
            }
            selectedButton = dayButton;
            dayButton.Select();
        }



        public override void Close()
        {
            base.Close();

            gameObject.SetActive(false);
        }



        private void OnStarsChanged(int starsCount)
        {
            starText.text = "" + starsCount;
        }
    }
}
