using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using GUI;
using GUI.SelectCardDeck;
using Models;
using Models.Structs;
using Story;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class PlanetManager : MonoBehaviour
    {
        [SerializeField] private StoryController story;
        [SerializeField] private SceneAsset currentSelectedScene;
        [SerializeField] private Transform cardSelectSpace;
        public CardInventoryUI CardInventory;
        
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

        private void Start()
        {
            //Debug.Log("Planet Start");
            var missions = DataManager.GetPlanetData()[0].Missions;
            var missionsUI = GetComponentsInChildren<MissionInfoUI>();

            for (int i = 0; i < missionsUI.Length; i++)
            {
                var missionData = missions[i];
                missionsUI[i].Setup(missionData);
            }

            story.CheckTrigger(CutSceneTrigger.PlanetMap, null);
        }

        public Transform GetCardSelectSpace() => cardSelectSpace;

        public void StartMission(SceneAsset mission)
        {
            currentSelectedScene = mission;

            story.CheckTrigger(CutSceneTrigger.CardSelection, null);
        }
        
        public void BackToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void MoveToGameScene()
        {
            if (currentSelectedScene is null) return;
            
            StartCoroutine(LoadSceneAsync(currentSelectedScene.name));
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

            var deckData = new GameObject();
            deckData.name = "DeckData";
            deckData.tag = "DataContainer";
            var dataContainer = deckData.AddComponent<DataContainer>();
            dataContainer.Datas = new List<object>
            {
                new DeckData
                {
                    CardList = CardInventory.GetCardSelected()
                }
            };
            SceneManager.MoveGameObjectToScene(deckData, SceneManager.GetSceneByName(sceneName));

            WorldManager.Instance.enabled = true;
            SceneManager.UnloadSceneAsync("Loading");
            SceneManager.UnloadSceneAsync(old);
        }
    }
}