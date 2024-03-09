using System.Collections;
using Controllers;
using EventArgs;
using Models;
using Models.SpecialCharacter;

namespace Story
{
    public class OpponentHeadquarterFirstDamage : StoryTriggerSetup
    {
        private void Start()
        {
            isFirstTrigger = false;
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
            if (!isFirstTrigger && sender is Headquarter && args.StatsType == StatsType.Health)
            {
                isFirstTrigger = true;
                WorldManager.Instance.HeadquarterTakeDamageRemoveListener(HeadquarterStatsChange);
                story.CheckTrigger(CutSceneTrigger.Others, null);
            }
        }
    }
}