using System;
using System.Collections.Generic;
using System.IO;
using Controllers;
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
        
        public static void SaveGameData()
        {
            /*var dataHandler = new FileDataHandler(Application.persistentDataPath, "setting.gd");
            dataHandler.Save(SettingData);
            
            dataHandler = new FileDataHandler(Application.persistentDataPath, "storyLine.gd");
            dataHandler.Save(Story);
            
            for (int i = 0; i < planetDatas.Count; i++)
            {
                var planetDir = Path.Combine(planetPath, $"planet{i + 1}.gd");
                dataHandler = new FileDataHandler(Application.persistentDataPath, planetDir);
                dataHandler.Save(planetDatas[i]);
            }*/
        }

        public static void LoadGameData()
        {
            if (!firstLoad) return;

            firstLoad = false;
            var dataHandler = new FileDataHandler(Application.persistentDataPath, "setting.gd");
            SettingData = dataHandler.Load<GameSettingData>();
            
            dataHandler = new FileDataHandler(Application.persistentDataPath, "cardInventory.gd");
            cardInventory = dataHandler.Load<CardInventory>();

            dataHandler = new FileDataHandler(Application.persistentDataPath, "storyLine.gd");
            Story = dataHandler.Load<StoryData>();

            planetDatas = new List<PlanetData>();
            for (int i = 0; i < SettingData.PlanetAmount; i++)
            {
                var planetDir = Path.Combine(planetPath, $"planet{i + 1}.gd");
                dataHandler = new FileDataHandler(Application.persistentDataPath, planetDir);
                planetDatas.Add(dataHandler.Load<PlanetData>());
            }
            
            dataHandler = new FileDataHandler(Application.persistentDataPath, "planetTrigger.gd");
            PlanetTrigger = dataHandler.Load<StoryPlanetTrigger>();
            
            dataHandler = new FileDataHandler(Application.persistentDataPath, "cardSelectionTrigger.gd");
            CardSelectionTrigger = dataHandler.Load<CardSelectionTrigger>();
        }

        public static List<PlanetData> GetPlanetData() => new (planetDatas);

        public static List<CardName> GetCardInventory() => new(cardInventory.Cards);

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
    }
}