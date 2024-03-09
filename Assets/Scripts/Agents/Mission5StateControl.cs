using System.Collections;
using Controllers;
using EventArgs;
using Models;
using Models.SpecialCharacter;
using Models.Spells;
using ScriptableObjects;
using Story;
using UnityEngine;

namespace Agents
{
    public class Mission5StateControl : MonoBehaviour
    {
        [SerializeField] private StoryController story;
        
        [SerializeField] private OpponentBot mission5State1;
        [SerializeField] private OpponentBot mission5State2;

        [SerializeField] private CardInfo destroyer;

        private void Start()
        {
            StartCoroutine(WaitingToBotStart());
        }

        IEnumerator WaitingToBotStart()
        {
            while (!mission5State1.enabled)
            {
                yield return null;
            }
            
            WorldManager.Instance.HeadquarterTakeDamageListener(HeadquarterDetectDamage);
        }

        private void HeadquarterDetectDamage(object sender, CharacterStatsChangeEventArgs args)
        {
            if (sender is not Headquarter || args.StatsType != StatsType.Health) return;
            
            // Action destroy, active state 2
            foreach (var character in WorldManager.Instance.GetCharactersOnWorld())
            {
                SpellsExecute.Activate(character, destroyer.SpellsEffect);
            }
            
            mission5State1.enabled = false;

            story.CheckTrigger(CutSceneTrigger.Others, null);
            CardController.Instance.OnCardUsed += OnPlayerUseCard;
            
            mission5State2.enabled = true;
            
            WorldManager.Instance.HeadquarterTakeDamageRemoveListener(HeadquarterDetectDamage);
        }

        private void OnPlayerUseCard(int team, CardName card)
        {
            var playerTeam = WorldManager.Instance.Team1Player ? 0 : 1;

            if (playerTeam == team && card == CardName.ThermonuclearBomb)
            {
                WorldManager.Instance.OnEndGame(this);
            }
        }
    }
}