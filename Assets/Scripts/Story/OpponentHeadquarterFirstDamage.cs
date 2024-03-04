using System.Collections;
using Controllers;
using EventArgs;
using Models;
using Models.SpecialCharacter;
using UnityEngine;

namespace Story
{
    public class OpponentHeadquarterFirstDamage : StoryTriggerSetup
    {
        [SerializeField] private bool isFirstDamage;
        private void Start()
        {
            isFirstDamage = false;
            StartCoroutine(WaitWorldManager());
        }

        private IEnumerator WaitWorldManager()
        {
            while (WorldManager.Instance is null)
            {
                yield return null;
            }
            
            WorldManager.Instance.HeadquarterTakeDamageListener(HeadquarterStatsChange);
        }

        private void HeadquarterStatsChange(object sender, CharacterStatsChangeEventArgs args)
        {
            if (!isFirstDamage && sender is Headquarter && args.StatsType == StatsType.Health)
            {
                isFirstDamage = true;
                WorldManager.Instance.HeadquarterTakeDamageRemoveListener(HeadquarterStatsChange);
                story.CheckTrigger(CutSceneTrigger.Others, null);
            }
        }
    }
}