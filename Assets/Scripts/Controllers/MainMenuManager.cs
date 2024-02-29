using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class MainMenuManager : MonoBehaviour
    {
        public void Play()
        {
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
    }
}