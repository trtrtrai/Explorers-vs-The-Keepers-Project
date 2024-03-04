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
                DataManager.SettingData.IsFirstPlay = false;
                var storyCanvas = Instantiate(Resources.Load<GameObject>("StoryCanvas"));
                var script = storyCanvas.GetComponent<StoryManager>();
                script.PlayCutScene(0, Play);
            }
            else SceneManager.LoadScene("PlanetMap1");
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
    }
}