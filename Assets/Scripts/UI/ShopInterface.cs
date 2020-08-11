using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class ShopInterface : Window
    {
        public GameManager gameManager;
        public Text starText;
        public Transform content;

        public GameObject skinPanelPrefab;

        public int selectedSkinID = -1;

        private List<SkinPanel> skinPanels = new List<SkinPanel>();
        private int currentPickedSkinID;



        public override void Init()
        {
            gameManager.gameInfo.onStarsChange.AddListener(OnStarsChanged);
            CreateSkinPanels();
        }



        private void CreateSkinPanels()
        {
            foreach (Skin skin in gameManager.skins)
            {
                GameObject goSkinPanel = Instantiate(skinPanelPrefab, content);
                SkinPanel skinPanel = goSkinPanel.GetComponent<SkinPanel>();
                skinPanel.UpdatePanel(skin);
                if (skin.id == gameManager.CurrentSkinID)
                {
                    skinPanel.SetPicked(true);
                    currentPickedSkinID = skin.id;
                }
                skinPanels.Add(skinPanel);
            }
        }
        


        private void UpdateSkinPanels()
        {
            foreach (SkinPanel skinPanel in skinPanels)
            {
                skinPanel.UpdatePanel(gameManager.GetSkin(skinPanel.skin.id));
            }
        }



        private void OnStarsChanged(int starsCount)
        {
            starText.text = "" + starsCount;
        }



        public void OnBackButtonClick()
        {
            this.Close();
            ScreenManager.Instance.menuInterface.Open();
        }



        public override void Open()
        {
            base.Open();

            this.gameObject.SetActive(true);
        }



        public override void Close()
        {
            base.Close();

            gameObject.SetActive(false);
        }



        public void SelectPanel(SkinPanel skinPanel)
        {
            Debug.Log($"Selected panel id = {skinPanel.skin.id} ({gameManager.GetSkin(skinPanel.skin.id).sprite.name})");
            switch (skinPanel.skin.status)
            {
                // выбрали уже купленный скин
                case SkinStatus.Bought:
                    if (currentPickedSkinID == skinPanel.skin.id)
                    {
                        return;
                    }
                    GetSkinPanel(currentPickedSkinID).SetPicked(false);
                    currentPickedSkinID = skinPanel.skin.id;
                    gameManager.CurrentSkinID = skinPanel.skin.id;
                    skinPanel.SetPicked(true);
                    gameManager.SaveSkins();
                    break;

                // покупка скина
                case SkinStatus.Buy:
                    selectedSkinID = skinPanel.skin.id;
                    ScreenManager.Instance.buySkinInterface.Open();
                    break;

                // получить скин за просмотр видео
                case SkinStatus.Ads:
                    selectedSkinID = skinPanel.skin.id;
                    ScreenManager.Instance.buySkinInterface.Open();
                    break;
            }
        }



        private SkinPanel GetSkinPanel(int id)
        {
            foreach (SkinPanel skinPanel in skinPanels)
            {
                if (skinPanel.skin.id == id)
                {
                    return skinPanel;
                }
            }
            return null;
        }



        public void Buy(int id)
        {
            var skinPanel = GetSkinPanel(id);
            gameManager.gameInfo.Stars -= skinPanel.skin.price;
            skinPanel.skin.status = SkinStatus.Bought;
            gameManager.SaveSkins();
            gameManager.SaveGameInfo();
        }



        public void Pick(int id)
        {
            var skinPanel = GetSkinPanel(id);
            GetSkinPanel(currentPickedSkinID).SetPicked(false);
            currentPickedSkinID = skinPanel.skin.id;
            gameManager.CurrentSkinID = skinPanel.skin.id;
            skinPanel.SetPicked(true);
            gameManager.SaveSkins();
        }
    }
}
