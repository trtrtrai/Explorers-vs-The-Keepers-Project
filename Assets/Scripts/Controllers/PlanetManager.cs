using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class PlanetManager : MonoBehaviour
    {
        [SerializeField] private Transform cardSelectSpace;
        
        public static PlanetManager Instance
        {
            get;
            private set;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public Transform GetCardSelectSpace() => cardSelectSpace;

        public void StartMission(SceneAsset mission)
        {
            StartCoroutine(LoadSceneAsync(mission.name));
        }
        
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            var old = SceneManager.GetActiveScene().name;
            var asyncLoad = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);

            Camera.main!.GetComponent<AudioListener>().enabled = false;
            Destroy(EventSystem.current.gameObject);

            while (!asyncLoad.isDone) yield return null;
            
            asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (!asyncLoad.isDone) yield return null;
            
            SceneManager.UnloadSceneAsync("Loading");
            SceneManager.UnloadSceneAsync(old);
        }
    }
}