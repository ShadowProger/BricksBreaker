using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class SkinPanel : MonoBehaviour
    {
        public GameObject bought;
        public GameObject picked;
        public GameObject buy;
        public GameObject ads;

        public GameObject goBallSkin;
        public GameObject goAnimatedballSkin;

        public Color boughtColor;
        public Color pickedColor;
        public Color buyColor;
        public Color adsColor;

        public Image frame;
        public Image ballSkin;
        public Image animatedBallSkin;
        public Text price;

        private Button skinButton;
        public Skin skin;



        private void Awake()
        {
            skinButton = GetComponent<Button>();
        }



        public void UpdatePanel(Skin skin)
        {
            bought.SetActive(skin.status == SkinStatus.Bought);
            buy.SetActive(skin.status == SkinStatus.Buy);
            ads.SetActive(skin.status == SkinStatus.Ads);
            picked.SetActive(false);
            ballSkin.sprite = skin.sprite;
            animatedBallSkin.sprite = skin.sprite;
            price.text = $"{skin.price}";
            this.skin = skin;
            SetFrameColor(skin.status);

            skinButton.onClick.AddListener(() =>
            {
                ScreenManager.Instance.shopInterface.SelectPanel(this);
            });
        }



        public void SetPicked(bool isPicked)
        {
            picked.SetActive(isPicked);
            goBallSkin.SetActive(!isPicked);
            goAnimatedballSkin.SetActive(isPicked);
            if (isPicked)
            {
                bought.SetActive(false);
                buy.SetActive(false);
                ads.SetActive(false);
                frame.color = pickedColor;
            }
            else
            {
                bought.SetActive(skin.status == SkinStatus.Bought);
                buy.SetActive(skin.status == SkinStatus.Buy);
                ads.SetActive(skin.status == SkinStatus.Ads);
                SetFrameColor(skin.status);
            }
        }



        private void SetFrameColor(SkinStatus status)
        {
            switch (status)
            {
                case SkinStatus.Bought:
                    frame.color = boughtColor;
                    break;
                case SkinStatus.Buy:
                    frame.color = buyColor;
                    break;
                case SkinStatus.Ads:
                    frame.color = adsColor;
                    break;
            }
        }
    }
}
