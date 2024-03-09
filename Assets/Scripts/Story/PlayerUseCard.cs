using System.Collections;
using Controllers;
using UnityEngine;

namespace Story
{
    public class PlayerUseCard : StoryTriggerSetup
    {
        [SerializeField] private int playerTeam;
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

            playerTeam = WorldManager.Instance.Team1Player ? 0 : 1;
            CardController.Instance.OnCardUsed += OnCardUse;
        }

        private void OnCardUse(int team, CardName cardName)
        {
            if (!isFirstTrigger && team == playerTeam && cardName == cardTrigger)
            {
                isFirstTrigger = true;
                
                story.CheckTrigger(CutSceneTrigger.Others, null);
            }
        }
    }
}