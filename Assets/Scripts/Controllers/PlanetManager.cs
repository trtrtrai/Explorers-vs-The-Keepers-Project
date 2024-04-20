using System.Collections;
using System.Collections.Generic;
using Data;
using GUI;
using GUI.SelectCardDeck;
using Models;
using Models.Structs;
using Story;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class PlanetManager : MonoBehaviour
    {
        [SerializeField] private StoryController story;
        [SerializeField] private ScrollViewPlanetMap scrollView;
        [SerializeField] private string currentSelectedScene;
        [SerializeField] private Transform cardSelectSpace;
        [SerializeField] private int missionCurrentIndex;
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
            
            scrollView.ScrollToMission(DataManager.GetLastMissionAvailable());

            story.CheckTrigger(CutSceneTrigger.PlanetMap, null);
        }

        public Transform GetCardSelectSpace() => cardSelectSpace;

        public void StartMission(string missionName)
        {
            currentSelectedScene = missionName;

            story.CheckTrigger(CutSceneTrigger.CardSelection, null, missionCurrentIndex);
        }

        public void MissionSelected(int missionIndex) => missionCurrentIndex = missionIndex;
        
        public void BackToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void MoveToGameScene()
        {
            if (currentSelectedScene.Length == 0) return;
            
            StartCoroutine(LoadSceneAsync(currentSelectedScene));
        }

        public void ClosedCardSelection()
        {
            foreach (var cardDrag in CardInventory.GetCardDragSelected())
            {
                cardDrag.BackToOrigin();
            }
        }
        
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            var old = SceneManager.GetActiveScene().name;
            var asyncLoad = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);

            //Camera.main!.GetComponent<AudioListener>().enabled = false;
            Destroy(EventSystem.current.gameObject);

            AudioListener.pause = true;
            while (!asyncLoad.isDone) yield return null;
            
            asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (!asyncLoad.isDone) yield return null;

            var deckData = new GameObject();
            deckData.name = "DeckData";
            deckData.tag = "DataContainer";
            var dataContainer = deckData.AddComponent<DataContainer>();
            dataContainer.Datas = new List<object>
            {
                new Models.Structs.MissionData
                {
                    PlanetMap = 0,
                    MissionIndex = missionCurrentIndex
                },
                new DeckData
                {
                    CardList = CardInventory.GetCardSelected()
                }
            };

            var scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.MoveGameObjectToScene(deckData, scene);
            SceneManager.MoveGameObjectToScene(GameObject.FindGameObjectWithTag("AudioController"), scene);

            WorldManager.Instance.enabled = true;
            SceneManager.UnloadSceneAsync("Loading");
            SceneManager.UnloadSceneAsync(old);
            AudioListener.pause = false;
        }

        private void OnDisable()
        {
            Instance = null;
        }
    }
}