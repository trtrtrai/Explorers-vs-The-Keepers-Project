using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

namespace Story
{
    public class StoryController : MonoBehaviour
    {
        [SerializeField] private int missionIndex;
        [SerializeField] private List<CutSceneAttach> attaches;

        public bool CheckTrigger(CutSceneTrigger currentTrigger, Action callback)
        {
            var finder = attaches.Any(c => c.triggerSignal == currentTrigger);

            if (!finder) return false;

            var firstPlay = DataManager.GetPlanetData()[0].Missions[missionIndex].firstPlay;

            if (firstPlay)
            {
                var storyCanvas = Instantiate(Resources.Load<GameObject>("StoryCanvas"));
                var script = storyCanvas.GetComponent<StoryManager>();
                var cutSceneAttach = attaches.First(c => c.triggerSignal == currentTrigger);
                script.PlayCutScene(cutSceneAttach.cutSceneIndex, callback);

                return true;
            }

            return false;
        }
    }

    [Serializable]
    public struct CutSceneAttach
    {
        public int cutSceneIndex;
        public CutSceneTrigger triggerSignal;
    }

    public enum CutSceneTrigger
    {
        StartMission,
        EndMission,
        Others,
    }
}