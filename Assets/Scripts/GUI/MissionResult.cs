using System.Collections;
using Controllers;
using Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace GUI
{
    public class MissionResult : MonoBehaviour
    {
        private void Start()
        {
            Time.timeScale = 0;
            Debug.Log(WorldManager.Instance.GameResult
                ? "Mission result get player won"
                : "Mission result get player lose");

            if (WorldManager.Instance.GameResult)
            {
                DataManager.UpdateDataStory();
            }
            // update data?
            // unlock new mission?
            // reward new cards?
        }

        public void BackToPlanet(SceneAsset sceneAsset)
        {
            StartCoroutine(BackToPlanetAsync(sceneAsset.name));
        }
        
        private IEnumerator BackToPlanetAsync(string sceneName)
        {
            var old = SceneManager.GetActiveScene().name;
            var asyncLoad = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);

            Camera.main!.GetComponent<AudioListener>().enabled = false;
            Destroy(EventSystem.current.gameObject);

            while (!asyncLoad.isDone) yield return null;
            
            asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            while (!asyncLoad.isDone) yield return null;

            Time.timeScale = 1f;
            
            SceneManager.UnloadSceneAsync("Loading");
            SceneManager.UnloadSceneAsync(old);
        }
    }
}