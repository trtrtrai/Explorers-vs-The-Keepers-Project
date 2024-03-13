using System;
using System.Linq;
using Data;
using Story;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class MainMenuManager : MonoBehaviour
    {
        private void Start()
        {
            DataManager.LoadGameData();
            /*var data = DataManager.GetPlanetData();
            
            foreach (var planet in data)
            {
                foreach (var planetMission in planet.Missions)
                {
                    Debug.Log(planetMission.status);
                }
            }*/
        }

        public void Play()
        {
            if (DataManager.SettingData.IsFirstPlay)
            {
                var storyCanvas = Instantiate(Resources.Load<GameObject>("StoryCanvas"));
                var script = storyCanvas.GetComponent<StoryManager>();
                script.PlayCutScene(0, LoadMapScene, () => {
                    DataManager.SettingData.IsFirstPlay = false;
                });
            }
            else
            {
                LoadMapScene();
            }
        }

        private void LoadMapScene()
        {
            DataManager.SettingData.IsFirstPlay = false;
            SceneManager.LoadScene("PlanetMap1");
        }

        public void Setting()
        {
            
        }

        public void About()
        {
            
        }

        public void Exit()
        {
            Application.Quit();
        }

        private void OnApplicationQuit()
        {
            DataManager.SaveGameData();
        }
    }
}