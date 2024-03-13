using System;
using System.Collections.Generic;
using System.IO;
using Controllers;
using ScriptableObjects;
using UnityEngine;

namespace Data
{
    public static class DataManager
    {
        private static bool firstLoad = true;
        private static string planetPath = "Planet";
        private static List<PlanetData> planetDatas;
        private static CardInventory cardInventory;
        
        public static GameSettingData SettingData;
        public static StoryData Story;
        public static StoryPlanetTrigger PlanetTrigger;
        public static CardSelectionTrigger CardSelectionTrigger;

        /// <summary>
        /// Load readonly data in Streaming Assets folder and create new in data folder.
        /// </summary>
        private static void NewGame()
        {
            var dataHandler = new FileDataHandler(Application.streamingAssetsPath, "setting.gd");
            SettingData = dataHandler.Load<GameSettingData>();

            cardInventory = new CardInventory
            {
                Cash = 0,
                Coin = 0,
                Cards = new List<CardName>()
            };

            dataHandler = new FileDataHandler(Application.streamingAssetsPath, "storyLine.gd");
            Story = dataHandler.Load<StoryData>();
            
            planetDatas = new List<PlanetData>();
            for (int i = 0; i < SettingData.PlanetAmount; i++)
            {
                var planetDir = Path.Combine(Application.streamingAssetsPath, planetPath);
                dataHandler = new FileDataHandler(planetDir, $"planet{i + 1}.gd");
                planetDatas.Add(dataHandler.Load<PlanetData>());
            }

            dataHandler = new FileDataHandler(Application.streamingAssetsPath, "planetTrigger.gd");
            PlanetTrigger = dataHandler.Load<StoryPlanetTrigger>();

            dataHandler = new FileDataHandler(Application.streamingAssetsPath, "cardSelectionTrigger.gd");
            CardSelectionTrigger = dataHandler.Load<CardSelectionTrigger>();
            
            SaveGameData();
        }

        public static void SaveSettingUp()
        {
            var dataHandler = new FileDataHandler(Application.persistentDataPath, "setting.gd");
            dataHandler.Save(SettingData);
        }
        
        public static void SaveGameData()
        {
            SaveSettingUp();
            
            var dataHandler = new FileDataHandler(Application.persistentDataPath, "cardInventory.gd");
            dataHandler.Save(cardInventory);
            
            for (int i = 0; i < planetDatas.Count; i++)
            {
                var planetDir = Path.Combine(Application.persistentDataPath, planetPath);
                dataHandler = new FileDataHandler(planetDir, $"planet{i + 1}.gd");
                dataHandler.Save(planetDatas[i]);
            }

            dataHandler = new FileDataHandler(Application.persistentDataPath, "planetTrigger.gd");
            dataHandler.Save(PlanetTrigger);

            dataHandler = new FileDataHandler(Application.persistentDataPath, "cardSelectionTrigger.gd");
            dataHandler.Save(CardSelectionTrigger);
        }

        public static void LoadGameData()
        {
            if (!firstLoad) return;

            firstLoad = false;
            var dataHandler = new FileDataHandler(Application.persistentDataPath, "setting.gd");
            SettingData = dataHandler.Load<GameSettingData>();

            if (SettingData is null)
            {
                NewGame();
            }
            else
            {
                dataHandler = new FileDataHandler(Application.persistentDataPath, "cardInventory.gd");
                cardInventory = dataHandler.Load<CardInventory>();

                dataHandler = new FileDataHandler(Application.streamingAssetsPath, "storyLine.gd");
                Story = dataHandler.Load<StoryData>();

                planetDatas = new List<PlanetData>();
                for (int i = 0; i < SettingData.PlanetAmount; i++)
                {
                    var planetDir = Path.Combine(Application.persistentDataPath, planetPath);
                    dataHandler = new FileDataHandler(planetDir, $"planet{i + 1}.gd");
                    planetDatas.Add(dataHandler.Load<PlanetData>());
                }

                dataHandler = new FileDataHandler(Application.persistentDataPath, "planetTrigger.gd");
                PlanetTrigger = dataHandler.Load<StoryPlanetTrigger>();

                dataHandler = new FileDataHandler(Application.persistentDataPath, "cardSelectionTrigger.gd");
                CardSelectionTrigger = dataHandler.Load<CardSelectionTrigger>();
            }
        }

        public static List<PlanetData> GetPlanetData() => new (planetDatas);

        public static List<CardName> GetCardInventory() => new(cardInventory.Cards);

        public static void UpdateInventory(Reward reward)
        {
            switch (reward.rewardItem)
            {
                case RewardItem.Card:
                    cardInventory.Cards.Add((CardName)Enum.GetValues(typeof(CardName)).GetValue(reward.value));
                    break;
                case RewardItem.Coin:
                    cardInventory.Coin += reward.value;
                    break;
                case RewardItem.Cash:
                    cardInventory.Cash += reward.value;
                    break;
            }
        }

        public static void UpdateDataStory()
        {
            var currentMissionIndex = planetDatas[0].Missions.FindIndex(p => p.firstPlay);

            if (currentMissionIndex == -1) return;

            planetDatas[0].Missions[currentMissionIndex].firstPlay = false;

            foreach (var reward in planetDatas[0].Missions[currentMissionIndex].rewards)
            {
                switch (reward.rewardItem)
                {
                    case RewardItem.Card:
                        cardInventory.Cards.Add((CardName)Enum.GetValues(typeof(CardName)).GetValue(reward.value));
                        break;
                    case RewardItem.Coin:
                        cardInventory.Coin += reward.value;
                        break;
                    case RewardItem.Cash:
                        cardInventory.Cash += reward.value;
                        break;
                }
            }

            if (currentMissionIndex < planetDatas[0].Missions.Count - 1)
            {
                // Update planet story and card selection story
                var nextMission = ++currentMissionIndex;
                planetDatas[0].Missions[nextMission].firstPlay = true;
                if (planetDatas[0].Missions[nextMission].status != MissionStatus.ComingSoon)
                {
                    planetDatas[0].Missions[nextMission].status = MissionStatus.Playable;
                }
                PlanetTrigger.Planet1[nextMission].triggerActive = true;
                CardSelectionTrigger.Planet1[nextMission].triggerActive = true;
            }
        }

        public static CardInfo GetCardInfo(string cardName)
        {
            return Resources.Load<CardInfo>($"ScriptableObjects/Cards/{cardName}Card");
        }
    }
}