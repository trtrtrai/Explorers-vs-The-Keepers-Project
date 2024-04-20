using Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using static Extensions.LoadSceneExtension;

namespace GUI
{
    public class GamePause : MonoBehaviour
    {
        public void BackToPlanet(string sceneName)
        {
            StartCoroutine(BackToPlanetAsync(sceneName, () =>
            {
                Destroy(EventSystem.current.gameObject);
                Destroy(GameObject.FindGameObjectWithTag("AudioController"));
            }));
        }

        public void Restart()
        {
            GameObject.FindGameObjectWithTag("SceneManagement").GetComponent<LoadSceneExtension>().MissionRestart();
        }

        private void OnEnable()
        {
            Time.timeScale = 0f;
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
        }
    }
}