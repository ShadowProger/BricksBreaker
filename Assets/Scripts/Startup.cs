using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public class Startup : MonoBehaviour
    {
        public GameObject adsController;
        public GameObject shareAndRate;
        public GameObject GameAnalytics;
        public GameObject GameAnalyticsController;
        public GameManager gameManager;

        private void Awake()
        {
            Debug.Log($"[Startup] Awake");
        }

        void Start()
        {
            Debug.Log($"[Startup] Start");

            if (!GDPR.ConsentIsSelect)
            {
                GDPRWindow.Instance.Show(() => 
                {
                    Debug.Log($"[GDPRWindow] Show callback");
                    Init();
                });
            }
            else
            {
                Init();
            }
        }

        public void Init()
        {
            Debug.Log($"[Startup] Init");
            var root = GameObject.Find("[MANAGERS]").transform;
            Create(shareAndRate, root);
            Create(adsController, root);

            Debug.Log($"Analytics Init: consent = {GDPR.AnalyticsConsent}");
            if (GDPR.AnalyticsConsent)
            {
                Create(GameAnalytics, root);
                Create(GameAnalyticsController, root);
            }
            gameManager.Init();
        }

        void Create(GameObject prefab, Transform parent = null, bool DontDestroy = true)
        {
            var instance = Instantiate<GameObject>(prefab);
            if (DontDestroy) DontDestroyOnLoad(instance);
        }
    }
}
