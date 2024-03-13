using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using GUI;
using UnityEngine;

namespace Story
{
    public class StoryController : MonoBehaviour
    {
        [SerializeField] private int missionIndex;
        [SerializeField] private List<CutSceneAttach> attaches;

        [SerializeField] private CollectionTab collection;

        public bool CheckTrigger(CutSceneTrigger currentTrigger, Action callback, int cardSelectionMission = -1)
        {
            if (currentTrigger == CutSceneTrigger.PlanetMap)
            {
                var planetCutSceneIndex = DataManager.PlanetTrigger.Planet1.FindIndex(p => p.triggerActive);

                if (planetCutSceneIndex == -1) return false;
                
                // Always true in here
                var storyCanvas = Instantiate(Resources.Load<GameObject>("StoryCanvas"));
                var script = storyCanvas.GetComponent<StoryManager>();

                void UpdateStoryProcessCallback()
                {
                    if (planetCutSceneIndex == 0)
                    {
                        var card1 = new Reward { rewardItem = RewardItem.Card, value = 0 };
                        DataManager.UpdateInventory(card1);
                        var card2 = new Reward { rewardItem = RewardItem.Card, value = 2 };
                        DataManager.UpdateInventory(card2);
                        collection.gameObject.SetActive(true);
                        collection.ShowReward(new List<Reward>
                        {
                            card1, card2
                        });
                    }
                    DataManager.PlanetTrigger.Planet1[planetCutSceneIndex].triggerActive = false;
                }

                script.PlayCutScene(DataManager.PlanetTrigger.Planet1[planetCutSceneIndex].storyIndex, callback, UpdateStoryProcessCallback);

                return true;
            }
            
            if (currentTrigger == CutSceneTrigger.CardSelection)
            {
                var planetCutSceneIndex = DataManager.CardSelectionTrigger.Planet1.FindIndex(p => p.triggerActive);

                if (planetCutSceneIndex == -1 || planetCutSceneIndex != cardSelectionMission) return false;
                
                // Always true in here
                var storyCanvas = Instantiate(Resources.Load<GameObject>("StoryCanvas"));
                var script = storyCanvas.GetComponent<StoryManager>();

                void UpdateStoryProcessCallback()
                {
                    DataManager.CardSelectionTrigger.Planet1[planetCutSceneIndex].triggerActive = false;
                }

                script.PlayCutScene(DataManager.CardSelectionTrigger.Planet1[planetCutSceneIndex].storyIndex, callback, UpdateStoryProcessCallback);

                return true;
            }
            
            var finder = attaches.Any(c => c.triggerSignal == currentTrigger);

            if (!finder) return false;

            var firstPlay = DataManager.GetPlanetData()[0].Missions[missionIndex].firstPlay;

            if (firstPlay)
            {
                var storyCanvas = Instantiate(Resources.Load<GameObject>("StoryCanvas"));
                var script = storyCanvas.GetComponent<StoryManager>();
                var cutSceneAttach = attaches.First(c => c.triggerSignal == currentTrigger);
                script.PlayCutScene(cutSceneAttach.cutSceneIndex, callback, null);

                return true;
            }

            return false;
        }

        public void OnApplicationQuit()
        {
            DataManager.SaveGameData();
        }
    }

    [Serializable]
    public class CutSceneAttach
    {
        public int cutSceneIndex;
        public CutSceneTrigger triggerSignal;
    }

    public enum CutSceneTrigger
    {
        PlanetMap,
        CardSelection,
        StartMission,
        EndMission,
        Others,
    }
}