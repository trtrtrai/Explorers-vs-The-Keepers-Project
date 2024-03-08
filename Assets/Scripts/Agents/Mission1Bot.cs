using System.Collections;
using Controllers;
using EventArgs;
using ScriptableObjects;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Agents
{
    public class Mission1Bot : OpponentBot
    {
        [SerializeField] private BehaviorParameters self;
        [SerializeField] private int botTeam;
        [SerializeField] private float spawnPenalty;

        public override void OnEpisodeBegin()
        {
            if (self.BehaviorType != BehaviorType.InferenceOnly && botTeam == 1) WorldManager.Instance.SoftReset();
            
            //initial penalty
            spawnPenalty = Random.Range(3f, 5f);
            StartCoroutine(SpawnPenalty());
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            if (CardController.Instance is null)
            {
                enabled = false;
                return;
            }
            
            var listCardCanUse = CardController.Instance.GetCardCanBeUsed(botTeam);
            var loop = Mathf.Clamp(listCardCanUse.Count, 0, 5);
            for (int i = 0; i < loop; i++)
            {
                sensor.AddObservation((int)listCardCanUse[i]);
            }

            for (int i = 0; i < 5 - loop; i++)
            {
                sensor.AddObservation(-1);
            }
            
            sensor.AddObservation(spawnPenalty);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            /*var isDebug = false;
            if (actions.DiscreteActions[0] == 1 && actions.DiscreteActions[1] == 1)
            {
                isDebug = true;
                //Debug.Log($"Action {actions.DiscreteActions[0]} - {actions.DiscreteActions[1]}");
            }*/
            if (actions.DiscreteActions[0] == 0)
            {
                // dont use card
                if (spawnPenalty <= 0f) SetReward(-0.02f);
                else SetReward(0.015f);
                
                if (CardController.Instance.GetCardCanBeUsed(botTeam).Count == 0)
                {
                    SetReward(0.005f);
                }
                else
                {
                    SetReward(-0.01f);
                }
            }
            else
            {
                // use card
                var cardEnum = actions.DiscreteActions[1];

                if (!CardController.Instance.IsHandContains(botTeam, cardEnum, out CardInfo card))
                {
                    SetReward(-0.02f);
                }
                else
                {
                    SetReward(0.005f);
                    //if (isDebug) Debug.Log(card.Name);
                    if (CardController.Instance.CardCanBeUsed(botTeam, card))
                    {
                        if (spawnPenalty <= 0f)
                        {
                            CardController.Instance.CardConsuming(botTeam, card);
                            UseCard(card);
                            SetReward(0.01f);
                            RefreshSpawnPenalty();
                        }
                        else
                        {
                            SetReward(-0.01f);
                        }
                    }
                    else
                    {
                        SetReward(-0.01f);
                    }
                }
            }
        }

        private void UseCard(CardInfo card)
        {
            switch (card.CardType)
            {
                case CardType.Minions:
                    WorldManager.Instance.CreateCharacter(card.Character, 0, botTeam, card.SpellsEffect);
                    SetReward(0.01f);
                    break;
                case CardType.Generals:
                    
                    break;
                case CardType.Spells:
                    break;
            }
        }

        private void RefreshSpawnPenalty()
        {
            spawnPenalty = Random.Range(10f, 12f); // 3.75f/Energy

            StartCoroutine(SpawnPenalty());
        }

        private IEnumerator SpawnPenalty()
        {
            while (spawnPenalty > 0f)
            {
                spawnPenalty -= Time.deltaTime;
                
                yield return null;
            }
        }

        public override void OnHeadquarterDestroy(object sender, System.EventArgs args)
        {
            if (sender is int teamWon)
            {
                if (self.BehaviorType != BehaviorType.InferenceOnly && botTeam == 1) WorldManager.Instance.SoftReset();
                if (self.BehaviorType == BehaviorType.InferenceOnly) enabled = false;

                /*if (teamWon == botTeam)
                {
                    SetReward(1f);
                    if (self.BehaviorType != BehaviorType.InferenceOnly) EndEpisode();
                    else enabled = false;
                }
                else
                {
                    SetReward(-1f);
                    if (self.BehaviorType != BehaviorType.InferenceOnly) EndEpisode();
                    else enabled = false;
                }*/
            }
        }

        public override void OnCharacterSpawn(object sender, CharacterSpawnEventArgs args)
        {
            
        }

        protected override void OnDisable()
        {
            StopAllCoroutines();
            
            base.OnDisable();
        }
    }
}