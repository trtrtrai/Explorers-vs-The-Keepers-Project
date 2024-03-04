using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Data
{
    public static class DataManager
    {
        private static bool firstLoad = true;
        private static string planetPath = "Planet";
        private static List<PlanetData> planetDatas;
        
        public static GameSettingData SettingData;
        public static StoryData Story;
        
        public static void SaveGameData()
        {
            GameData gameData = new StoryData()
            {
                CutScenes = new List<CutScene>()
                {
                    new CutScene()
                    {
                        speeches = new List<Speech>()
                        {
                            new Speech()
                            {
                                talker = CharacterSpeech.Supporter,
                                talkerSpriteName = "supporter-default",
                                speechTexts = new List<string>()
                                {
                                    "Hello Explorer! Welcome to Campaign PW-01."
                                }
                            }
                        }
                    }
                }
            };
            var dataHandler = new FileDataHandler(Application.persistentDataPath, "storyLine.gd");
            dataHandler.Save(gameData);
        }

        public static void LoadGameData()
        {
            if (!firstLoad) return;

            firstLoad = false;
            var dataHandler = new FileDataHandler(Application.persistentDataPath, "setting.gd");
            SettingData = dataHandler.Load<GameSettingData>();

            dataHandler = new FileDataHandler(Application.persistentDataPath, "storyLine.gd");
            Story = dataHandler.Load<StoryData>();

            planetDatas = new List<PlanetData>();
            for (int i = 0; i < SettingData.PlanetAmount; i++)
            {
                var planetDir = Path.Combine(planetPath, $"planet{i + 1}.gd");
                dataHandler = new FileDataHandler(Application.persistentDataPath, planetDir);
                planetDatas.Add(dataHandler.Load<PlanetData>());
            }
        }

        public static List<PlanetData> GetPlanetData() => new (planetDatas);
    }
}