using Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static Extensions.LoadSceneExtension;

namespace GUI
{
    public class GamePause : MonoBehaviour
    {
        public void BackToPlanet(SceneAsset sceneAsset)
        {
            StartCoroutine(BackToPlanetAsync(sceneAsset.name, () =>
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