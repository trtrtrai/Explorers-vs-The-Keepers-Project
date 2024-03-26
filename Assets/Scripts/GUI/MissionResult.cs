using System.Collections;
using Controllers;
using Data;
using Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static Extensions.LoadSceneExtension;

namespace GUI
{
    public class MissionResult : MonoBehaviour
    {
        [SerializeField] private MissionReward reward;
        [SerializeField] private Transform failed;
        
        private void Start()
        {
            Time.timeScale = 0;
            Debug.Log(WorldManager.Instance.GameResult
                ? $"Mission {WorldManager.Instance.MissionData.MissionIndex + 1} result get player won"
                : $"Mission {WorldManager.Instance.MissionData.MissionIndex + 1} result get player lose");

            if (WorldManager.Instance.GameResult)
            {
                // update data
                // unlock new mission
                // reward items
                DataManager.UpdateDataStory();
                reward.gameObject.SetActive(true);
                reward.ShowReward(WorldManager.Instance.GameResult);
            }
            else
            {
                failed.gameObject.SetActive(true);
            }
        }

        public void BackToPlanet(SceneAsset sceneAsset)
        {
            StartCoroutine(BackToPlanetAsync(sceneAsset.name, () => {
                Destroy(EventSystem.current.gameObject);
                Destroy(GameObject.FindGameObjectWithTag("AudioController"));
            }));
        }
        
        public void Restart()
        {
            GameObject.FindGameObjectWithTag("SceneManagement").GetComponent<LoadSceneExtension>().MissionRestart();
        }
    }
}