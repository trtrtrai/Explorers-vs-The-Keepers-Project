using System;
using System.Collections;
using Controllers;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Extensions
{
    public class LoadSceneExtension : MonoBehaviour
    {
        private void Awake()
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("SceneManagement");

            if (objs.Length > 1)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }
        
        public static void LoadMapScene()
        {
            DataManager.SettingData.IsFirstPlay = false;
            SceneManager.LoadScene("PlanetMap1");
        }

        public static IEnumerator BackToPlanetAsync(string sceneName, Action destroyObjCallback)
        {
            var asyncLoad = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
            
            destroyObjCallback?.Invoke();
            while (!asyncLoad.isDone) yield return null;

            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }

        public void MissionRestart()
        {
            StartCoroutine(MissionRestartAsync());
        }
        
        private IEnumerator MissionRestartAsync()
        {
            var cur = SceneManager.GetActiveScene().name;
            Time.timeScale = 1f;
            
            var asyncLoad = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
            while (!asyncLoad.isDone) yield return null;
            
            SceneManager.MoveGameObjectToScene(GameObject.FindGameObjectWithTag("AudioController"), SceneManager.GetSceneByName("Loading"));
            
            var u = SceneManager.UnloadSceneAsync(cur);
            while (!u.isDone) yield return null;
            
            var newScene = SceneManager.CreateScene("Empty");
            SceneManager.SetActiveScene(newScene);
            
            asyncLoad = SceneManager.LoadSceneAsync(cur, LoadSceneMode.Additive);
            while (!asyncLoad.isDone) yield return null;
            
            SceneManager.UnloadSceneAsync(newScene);
            SceneManager.MoveGameObjectToScene(GameObject.FindGameObjectWithTag("AudioController"), SceneManager.GetSceneByName(cur));
            WorldManager.Instance.enabled = true;
            SceneManager.UnloadSceneAsync("Loading");
        }
    }
}