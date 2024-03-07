using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace GUI
{
    public class GamePause : MonoBehaviour
    {
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

            Time.timeScale = 1;
            
            SceneManager.UnloadSceneAsync("Loading");
            SceneManager.UnloadSceneAsync(old);
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