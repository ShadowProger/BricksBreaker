using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class ScreenManager : SingletonMonoBehaviour<ScreenManager>
    {
        public GameInterface gameInterface;
        public MenuInterface menuInterface;
        public PauseInterface pauseInterface;
        public GameOverInterface gameOverInterface;
        public DailyChallengeInterface dailyChallengeInterface;
        public UnfortunatelyInterface unfortunatelyInterface;
        public LevelCompleteInterface levelCompleteInterface;
        public ShopInterface shopInterface;
        public ContinueInterface continueInterface;
        public BuySkinInterface buySkinInterface;
        public MoreStarsInterface moreStarsInterface;
        public NoVideoAvailableInterface noVideoAvailableInterface;
        public SettingsInterface settingsInterface;



        public void Init()
        {
            gameInterface.gameObject.SetActive(true);
            menuInterface.gameObject.SetActive(true);
            pauseInterface.gameObject.SetActive(true);
            gameOverInterface.gameObject.SetActive(true);
            dailyChallengeInterface.gameObject.SetActive(true);
            unfortunatelyInterface.gameObject.SetActive(true);
            levelCompleteInterface.gameObject.SetActive(true);
            shopInterface.gameObject.SetActive(true);
            continueInterface.gameObject.SetActive(true);
            buySkinInterface.gameObject.SetActive(true);
            moreStarsInterface.gameObject.SetActive(true);
            noVideoAvailableInterface.gameObject.SetActive(true);
            settingsInterface.gameObject.SetActive(true);

            gameInterface.Init();
            menuInterface.Init();
            pauseInterface.Init();
            gameOverInterface.Init();
            dailyChallengeInterface.Init();
            unfortunatelyInterface.Init();
            levelCompleteInterface.Init();
            shopInterface.Init();
            continueInterface.Init();
            buySkinInterface.Init();
            moreStarsInterface.Init();
            noVideoAvailableInterface.Init();
            settingsInterface.Init();

            gameInterface.gameObject.SetActive(false);
            pauseInterface.gameObject.SetActive(false);
            gameOverInterface.gameObject.SetActive(false);
            dailyChallengeInterface.gameObject.SetActive(false);
            unfortunatelyInterface.gameObject.SetActive(false);
            levelCompleteInterface.gameObject.SetActive(false);
            shopInterface.gameObject.SetActive(false);
            continueInterface.gameObject.SetActive(false);
            buySkinInterface.gameObject.SetActive(false);
            moreStarsInterface.gameObject.SetActive(false);
            noVideoAvailableInterface.gameObject.SetActive(false);
            settingsInterface.gameObject.SetActive(false);
        }
    }
}
