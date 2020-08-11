using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class Calendar : MonoBehaviour
    {
        public GameManager gameManager;
        public GameObject emptyDayButtonPrefab;
        public GameObject dayButtonPrefab;

        private List<GameObject> emptyDayButtons = new List<GameObject>();
        private List<DayButton> dayButtons = new List<DayButton>();
        private DateTime today;
        private int month;
        private int year;
        private DayButton todayButton = null;



        public DayButton Init(DateTime today)
        {
            this.today = today;
            this.month = today.Month;
            this.year = today.Year;

            Show(today.Year, today.Month);

            return todayButton;
        }



        public void GoNext()
        {
            this.month++;
            if (this.month > 12)
            {
                this.month = 1;
                this.year++;
            }
            Show(this.year, this.month);
        }



        public void GoPrev()
        {
            this.month--;
            if (this.month < 1)
            {
                this.month = 12;
                this.year--;
            }
            Show(this.year, this.month);
        }



        private void Show(int year, int month)
        {
            foreach (var item in emptyDayButtons)
            {
                Destroy(item);
            }
            emptyDayButtons.Clear();
            foreach (var item in dayButtons)
            {
                Destroy(item.gameObject);
            }
            dayButtons.Clear();

            DateTime firstDay = new DateTime(year, month, 1);
            int emptyDaysCount = (int)firstDay.DayOfWeek;

            for (int i = 0; i < emptyDaysCount; i++)
            {
                GameObject emptyDayButton = Instantiate(emptyDayButtonPrefab, transform);
                emptyDayButtons.Add(emptyDayButton);
            }

            int daysCount = DateTime.DaysInMonth(year, month);

            for (int i = 0; i < daysCount; i++)
            {
                DayButton dayButton = Instantiate(dayButtonPrefab).GetComponent<DayButton>();
                dayButtons.Add(dayButton);
                dayButton.transform.SetParent(transform, false);
                DateTime thisDay = new DateTime(year, month, i + 1);

                Day? day = gameManager.GetDay(thisDay.ToInt());

                if (day == null)
                {
                    dayButton.Init(thisDay, this.today);
                }
                else
                {
                    dayButton.Init(thisDay, this.today, day);
                }

                if (thisDay == this.today)
                {
                    todayButton = dayButton;
                }
            }
        }
    }
}
