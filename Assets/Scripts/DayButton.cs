using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class DayButton : MonoBehaviour
    {
        public Text dayText;

        public Sprite normalSprite;
        public Sprite todaySprite;
        public Sprite selectSprite;
        public Sprite disableSprite;
        public Sprite completeSprite;
        public GameObject complete;

        private Image dayImage;
        private Button dayButton;
        private Day day;

        private Sprite sprite;



        private void Awake()
        {
            dayImage = GetComponent<Image>();
            dayButton = GetComponent<Button>();
        }



        public void Init(DateTime thisDay, DateTime today, Day? day = null)
        {
            complete.SetActive(false);
            if (day == null)
            {
                this.day.id = thisDay.Year * 10000 + thisDay.Month * 100 + thisDay.Day;
                this.day.isComplited = false;
            }
            else
            {
                this.day = day.Value;
            }

            if (thisDay > today)
            {
                dayImage.sprite = disableSprite;
                dayButton.interactable = false;
            }
            else if (thisDay < today)
            {
                dayImage.sprite = normalSprite;
                dayButton.interactable = true;
            }
            else
            {
                dayImage.sprite = todaySprite;
                dayButton.interactable = true;
            }

            if (this.day.isComplited && thisDay <= today)
            {
                dayImage.sprite = completeSprite;
                complete.SetActive(true);
            }

            sprite = dayImage.sprite;

            dayText.text = $"{thisDay.Day}";
            dayButton.onClick.AddListener(() =>
            {
                ScreenManager.Instance.dailyChallengeInterface.SelectDay(this.day);
                ScreenManager.Instance.dailyChallengeInterface.SelectButton(this);
            });
        }



        public void Select()
        {
            dayImage.sprite = selectSprite;
        }



        public void Unselect()
        {
            dayImage.sprite = sprite;
        }



        public Day GetDay()
        {
            return day;
        }
    }
}
