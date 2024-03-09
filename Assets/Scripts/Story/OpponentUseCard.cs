using System.Collections;
using Controllers;
using UnityEngine;

namespace Story
{
    public class OpponentUseCard : StoryTriggerSetup
    {
        [SerializeField] private int aiBotTeam;
        [SerializeField] private CardName cardTrigger;
        
        private void Start()
        {
            isFirstTrigger = false;
            StartCoroutine(WaitWorldManager());
        }

        private IEnumerator WaitWorldManager()
        {
            while (WorldManager.Instance is null || CardController.Instance is null)
            {
                yield return null;
            }

            aiBotTeam = WorldManager.Instance.Team1Player ? 1 : 0;
            CardController.Instance.OnCardUsed += OnCardUse;
        }

        private void OnCardUse(int team, CardName cardName)
        {
            if (!isFirstTrigger && team == aiBotTeam && cardName == cardTrigger)
            {
                isFirstTrigger = true;
                
                story.CheckTrigger(CutSceneTrigger.Others, null);
            }
        }
    }
}