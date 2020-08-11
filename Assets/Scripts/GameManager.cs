using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

namespace Manybits
{
    public class GameManager : MonoBehaviour
    {
        public CameraManager cameraManager;
        public Field field;

        public ScreenManager screenManager;

        private Rect openRect;

        public GameInfo gameInfo;
        public static GameMode gameMode;

        private string path;
        private string fileNameProgress = "gameProgress.xml";
        private string fileNameChallengeProgress = "challengeProgress.xml";
        private string fileNameField = "field.xml";
        private string fileNameSkinsData = "skinsData";
        private string fileNameSkins = "skins.xml";

        private XmlNode fieldNode = null;

        public string Path { get => path; }
        public string FileNameProgress { get => fileNameProgress; }
        public string FileNameChallengeProgress { get => fileNameChallengeProgress; }
        public string FileNameField { get => fileNameField; }

        public int selectedDay;
        public System.DateTime today;
        List<Day> days = new List<Day>();

        public bool giveStars;
        public int starsForLevel = 25;
        public int starsForContinue = 30;

        public const float waitingTime = 5f;

        public List<Skin> skins = new List<Skin>();
        [SerializeField] SpriteAtlas skinAtlas;

        Sprite currentSkin;
        private int currentSkinID = -1;

        public bool gdprConsent;
        public bool consentWindowShown;

        public Sprite CurrentSprite
        {
            get => currentSkin;
            set => currentSkin = value;
        }

        public int CurrentSkinID
        {
            get => currentSkinID;
            set
            {
                currentSkinID = value < 0 ? 0 : value;
                currentSkin = GetSkin(currentSkinID).sprite;
            }
        }



        private void Awake()
        {
            Debug.Log($"[GameManager] Awake");

            #if UNITY_ANDROID
            path = Application.persistentDataPath;
            #endif

            #if UNITY_EDITOR
            path = Application.dataPath;
            #endif
        }



        void Start()
        {
            Debug.Log("[GameManager] Start");
        }


        public void Init()
        {
            gameInfo = new GameInfo();

            Rect cameraRect = cameraManager.GetRect();
            openRect = cameraRect;
            openRect.yMin += screenManager.gameInterface.GetBottomPanelHeight() / cameraManager.GetCanvasHeight() * cameraRect.height;
            openRect.yMax -= screenManager.gameInterface.GetTopPanelHeight() / cameraManager.GetCanvasHeight() * cameraRect.height;

            field.PreInit();

            LoadSkinsData();
            CurrentSkinID = -1;
            if (!LoadSkins())
            {
                CurrentSkinID = 0;
            }

            LoadGameInfo();

            screenManager.Init();

            LoadChallengeProgress();

            gameInfo.InvokeAll();

            //Debug.Log($"Skin = {currentSkin.name}");

            DateTime d = DateTime.Today;
            today = new DateTime(d.Year, d.Month, d.Day);
        }

 

        void Update()
        {
            // test
            cameraManager.GetRect().DebugDraw(Color.red);
            openRect.DebugDraw(Color.yellow);
            field.GetBorders().DebugDraw(Color.green);
            //
        }



        public void StartInfinityGame()
        {
            int fieldWidth = 7;
            int fieldHeight = 9;

            Generator generator = new Generator(this);
            generator.Load("generator");

            field.Init(fieldWidth, fieldHeight, openRect, generator);

            if (gameInfo.canContinue)
            {
                LoadField();
                field.ContinueInfinityGame(fieldNode);
            }
            else
            {
                int checkpoint = gameInfo.checkpoint;
                gameInfo.checkpoint = 1;
                field.StartInfinityGame(checkpoint);
            }
        }



        public void StartDailyChallenge(int day)
        {
            int fieldWidth = 9;
            int fieldHeight = 11;

            Generator generator = new Generator(this);

            field.Init(fieldWidth, fieldHeight, openRect, generator);

            field.StartDailyChallenge(day);
        }



        public bool LoadGameInfo()
        {
            Debug.Log("[GameManager] LoadProgress");

            XmlDocument xml = new XmlDocument();
            string filePath = System.IO.Path.Combine(path, fileNameProgress);

            if (!File.Exists(filePath))
            {
                Debug.Log("[GameManager] File is not exist");
                return false;
            }

            try
            {
                string xmlString = "";

                xmlString = File.ReadAllText(filePath);

                xml.LoadXml(xmlString);

                XmlNode progressNode = xml.SelectSingleNode("progress");

                gameInfo.checkpoint = int.Parse(progressNode.Attributes["checkpoint"].Value);
                gameInfo.canContinue = bool.Parse(progressNode.Attributes["canContinue"].Value);
                gameInfo.Stars = int.Parse(progressNode.Attributes["stars"].Value);
                gameInfo.Best = int.Parse(progressNode.Attributes["best"].Value);

                Debug.Log("[GameManager] Load is successful");
                return true;
            }
            catch
            {
                Debug.Log("[GameManager] Load is unsuccessful");
                return false;
            }
        }



        public void SaveGameInfo()
        {
            Debug.Log("[GameManager] SaveProgress");

            XmlDocument xml = new XmlDocument();

            string filePath = System.IO.Path.Combine(path, fileNameProgress);

            XmlElement root = xml.CreateElement("progress");

            xml.AppendChild(root);

            root.SetAttribute("checkpoint", gameInfo.checkpoint.ToString());
            root.SetAttribute("canContinue", (gameInfo.canContinue).ToString());
            root.SetAttribute("stars", (gameInfo.Stars).ToString());
            root.SetAttribute("best", (gameInfo.Best).ToString());

            xml.Save(filePath);
        }



        public bool LoadChallengeProgress()
        {
            Debug.Log("[GameManager] LoadChallengeProgress");

            XmlDocument xml = new XmlDocument();
            string filePath = System.IO.Path.Combine(path, fileNameChallengeProgress);

            if (!File.Exists(filePath))
            {
                Debug.Log("[GameManager] File is not exist");
                return false;
            }

            try
            {
                string xmlString = "";

                xmlString = File.ReadAllText(filePath);

                xml.LoadXml(xmlString);

                XmlNode challengeNode = xml.SelectSingleNode("challenge");
                XmlNodeList dayNodes = challengeNode.SelectNodes("day");

                foreach (XmlNode dayNode in dayNodes)
                {
                    Day day = new Day();
                    day.id = int.Parse(dayNode.Attributes["id"].Value);
                    day.isComplited = bool.Parse(dayNode.Attributes["complited"].Value);
                    days.Add(day);
                }

                days.Sort();

                Debug.Log("[GameManager] Load is successful");
                return true;
            }
            catch
            {
                Debug.Log("[GameManager] Load is unsuccessful");
                return false;
            }
        }



        public void SaveChallengeProgress()
        {
            Debug.Log("[GameManager] SaveChallengeProgress");

            XmlDocument xml = new XmlDocument();

            string filePath = System.IO.Path.Combine(path, fileNameChallengeProgress);

            XmlElement root = xml.CreateElement("challenge");
            xml.AppendChild(root);

            days.Sort();

            for (int i = 0; i < days.Count; i++)
            {
                XmlElement dayNode = xml.CreateElement("day");

                dayNode.SetAttribute("id", days[i].id.ToString());
                dayNode.SetAttribute("complited", days[i].isComplited.ToString());

                root.AppendChild(dayNode);
            }

            xml.Save(filePath);
        }



        public bool LoadField()
        {
            XmlDocument xml = new XmlDocument();
            string filePath = System.IO.Path.Combine(path, fileNameField);

            if (!File.Exists(filePath))
            {
                return false;
            }

            string xmlString = "";

            xmlString = File.ReadAllText(filePath);

            xml.LoadXml(xmlString);

            fieldNode = xml.SelectSingleNode("field");

            return true;
        }



        public void SaveSkins()
        {
            Debug.Log("[GameManager] SaveSkins");

            XmlDocument xml = new XmlDocument();

            string filePath = System.IO.Path.Combine(path, fileNameSkins);

            XmlElement root = xml.CreateElement("skins");
            root.SetAttribute("currentID", currentSkinID.ToString());
            xml.AppendChild(root);

            for (int i = 0; i < skins.Count; i++)
            {
                XmlElement skinNode = xml.CreateElement("skin");

                skinNode.SetAttribute("id", skins[i].id.ToString());
                skinNode.SetAttribute("status", skins[i].status.ToString());

                root.AppendChild(skinNode);
            }

            xml.Save(filePath);
        }



        public bool LoadSkins()
        {
            Debug.Log("[GameManager] LoadSkins");

            XmlDocument xml = new XmlDocument();
            string filePath = System.IO.Path.Combine(path, fileNameSkins);

            if (!File.Exists(filePath))
            {
                Debug.Log("[GameManager] File is not exist");
                return false;
            }

            try
            {
                string xmlString = "";

                xmlString = File.ReadAllText(filePath);

                xml.LoadXml(xmlString);

                XmlNode root = xml.SelectSingleNode("skins");
                CurrentSkinID = int.Parse(root.Attributes["currentID"].Value);
                XmlNodeList skinNodes = root.SelectNodes("skin");

                foreach (XmlNode skinNode in skinNodes)
                {
                    int skinID = int.Parse(skinNode.Attributes["id"].Value);
                    Skin skin = GetSkin(skinID);
                    string statusStr = skinNode.Attributes["status"].Value;

                    if (skin.status == SkinStatus.Bought)
                    {
                        continue;
                    }

                    switch (statusStr)
                    {
                        case "Bought":
                            skin.status = SkinStatus.Bought;
                            break;
                        case "Buy":
                            skin.status = SkinStatus.Buy;
                            break;
                        case "Ads":
                            skin.status = SkinStatus.Ads;
                            break;
                    }
                }

                Debug.Log("[GameManager] Load is successful");
                return true;
            }
            catch
            {
                Debug.Log("[GameManager] Load is unsuccessful");
                return false;
            }
        }
        


        public bool LoadSkinsData()
        {
            Debug.Log("[GameManager] LoadSkinsData");

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(Resources.Load(fileNameSkinsData).ToString());

            XmlNodeList dataList = xml.GetElementsByTagName("skin");
            foreach (XmlNode skinNode in dataList)
            {
                Skin skin = new Skin();
                skin.id = int.Parse(skinNode.Attributes["id"].Value);
                string spriteName = skinNode.Attributes["sprite"].Value;
                skin.sprite = skinAtlas.GetSprite(spriteName);
                skin.ballName = skinNode.Attributes["name"].Value;
                skin.price = int.Parse(skinNode.Attributes["price"].Value);
                if (skin.price < 0)
                {
                    skin.status = SkinStatus.Bought;
                }
                else if (skin.price == 0)
                {
                    skin.status = SkinStatus.Ads;
                }
                else
                {
                    skin.status = SkinStatus.Buy;
                }
                skins.Add(skin);
            }

            Debug.Log("[GameManager] Load is successful");
            return true;
        }



        public Skin GetSkin(int id)
        {
            foreach (Skin skin in skins)
            {
                if (skin.id == id)
                {
                    return skin;
                }
            }
            return null;
        }



        public Field GetField()
        {
            return field;
        }



        public void QuitWithoutSave()
        {
            gameInfo.canContinue = false;
            SaveGameInfo();
        }



        public Day? GetDay(int id)
        {
            Day? day = null;
            for (int i = 0; i < days.Count; i++)
            {
                if (days[i].id == id)
                {
                    day = days[i];
                    break;
                }
            }

            return day;
        }



        public bool IsDayComplete(int day)
        {
            return GetDay(day) != null;
        }



        public void AddCompliteDay(int id)
        {
            Debug.Log($"[GameManager] Save day {id}");
            int i;
            for (i = 0; i < days.Count; i++)
            {
                if (days[i].id == id)
                {
                    break;
                }
            }

            if (i < days.Count)
            {
                days[i] = new Day(id, true);
            }
            else
            {
                days.Add(new Day(id, true));
            }
        }
    }



    public class GameInfo
    {
        public int checkpoint = 1;
        public bool canContinue = false;
        private int stars = 0;
        private int best;

        public int Stars
        {
            get => stars;

            set
            {
                stars = value;
                if (onStarsChange != null)
                {
                    onStarsChange.Invoke(stars);
                }
            }
        }

        public int Best
        {
            get => best;

            set
            {
                best = value;
                if (onBestChange != null)
                {
                    onBestChange.Invoke(best);
                }
            }
        }

        public StarsChangeEvent onStarsChange { get; set; }
        public BestChangeEvent onBestChange { get; set; }

        public GameInfo()
        {
            onStarsChange = new StarsChangeEvent();
            onBestChange = new BestChangeEvent();
        }

        public void InvokeAll()
        {
            onBestChange.Invoke(best);
            onStarsChange.Invoke(stars);
        }

        public class StarsChangeEvent : UnityEvent<int> { }
        public class BestChangeEvent : UnityEvent<int> { }
    }



    // Режим игры
    public enum GameMode { Infinity, DailyChallenge }
}
