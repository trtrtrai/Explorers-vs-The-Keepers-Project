using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using EventArgs;
using GUI;
using Models;
using Models.Spells;
using ScriptableObjects;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agents
{
    /// <summary>
    /// State 1 of Mission5Bot. Card do not use: The Rock, Axit Rain, Go Home
    /// While headquarter take the first attack, other script will activate Axit Rain remove all characters on War Field.
    /// </summary>
    public class Mission5State1Bot : Agent
    {
        [SerializeField] private BehaviorParameters self;
        [SerializeField] private int botTeam;
        [SerializeField] private float spawnPenalty;
        [SerializeField] private float spellsPenalty;

        [SerializeField] private int alliesRoad0;
        [SerializeField] private int alliesRoad1;
        [SerializeField] private int enemiesRoad0;
        [SerializeField] private int enemiesRoad1;
        [SerializeField] private bool theGeneralCanSummon;
        [SerializeField] private int roadIndexRecommend;

        public override void OnEpisodeBegin()
        {
            if (self.BehaviorType != BehaviorType.InferenceOnly && botTeam == 1) WorldManager.Instance.SoftReset();

            alliesRoad0 = 0;
            alliesRoad1 = 0;
            enemiesRoad0 = 0;
            enemiesRoad1 = 0;
            theGeneralCanSummon = false;
            roadIndexRecommend = -1;

            spellsPenalty = 0f;
            spawnPenalty = 0f;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            /*
             * Observation index 1: Vector2(x, y) with x is number of card can use (0-29), y is type of card (Minions, Generals, Spells) => 2
             * Observation index 2: Vector2(x, y) like index 1, it is a information of the second card on hand => 2
             * Observation index 3: Vector2(x, y) like index 1, it is a information of the third card on hand => 2
             * Observation index 4: Vector2(x, y) like index 1, it is a information of the fourth card on hand => 2
             * Observation index 5: Vector2(x, y) like index 1, it is a information of the fifth card on hand => 2
             * Observation index 6: Vector2(x, y) x is number of allies, y is number of enemies on road index 0 => 2
             * Observation index 7: Vector2(x, y) x is number of allies, y is number of enemies on road index 1 => 2
             * Observation index 8: boolean variable if Generals can summon => 1
             * Observation index 9: road index recommend for place on war field => 1
             * Observation index 10: time sleep Minions spawn => 1
             * Observation index 11: time sleep Spells active => 1
             */
            
            var listCardCanUse = CardController.Instance.GetCardCanBeUsed(botTeam);
            var loop = Mathf.Clamp(listCardCanUse.Count, 0, 5);
            for (int i = 0; i < loop; i++)
            {
                var vectorObserve = new Vector2Int((int)listCardCanUse[i],
                    CardController.Instance.GetCardType(listCardCanUse[i], botTeam));
                sensor.AddObservation(vectorObserve);
            }

            for (int i = 0; i < 5 - loop; i++)
            {
                sensor.AddObservation(-1 * Vector2Int.one);
            }
            
            sensor.AddObservation(new Vector2Int(alliesRoad0, enemiesRoad0));
            sensor.AddObservation(new Vector2Int(alliesRoad1, enemiesRoad1));
            theGeneralCanSummon = CardController.Instance.CheckCanSummon("TheForest", botTeam);
            sensor.AddObservation(theGeneralCanSummon);

            if (enemiesRoad1 - alliesRoad1 >= 2)
            {
                roadIndexRecommend = 1;
            }
            else if (enemiesRoad0 - alliesRoad0 >= 2)
            {
                roadIndexRecommend = 0;
            }
            else
            {
                roadIndexRecommend = -1;
            }

            if (alliesRoad0 == 0) roadIndexRecommend = 0;
            if (alliesRoad1 == 0) roadIndexRecommend = 1;
            
            sensor.AddObservation(roadIndexRecommend);
            
            sensor.AddObservation(spawnPenalty);
            sensor.AddObservation(spellsPenalty);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            /*
             * DiscreteActions index 0: is used card? 0-1
             * DiscreteActions index 1: what card is used? 0-29
             * DiscreteActions index 2: what type of card? 0-2
             * DiscreteActions index 3: what roadIndex would use the card? 0-1
             */
            if (actions.DiscreteActions[0] == 1 && actions.DiscreteActions[1] == 7 && actions.DiscreteActions[2] == 2)
            {
                Debug.Log("TheForest trigger " + actions.DiscreteActions[3] + " roadIndex, Required: " + theGeneralCanSummon);
            }
            if (actions.DiscreteActions[0] == 0)
            {
                if (CardController.Instance.GetCardCanBeUsed(botTeam).Count == 0)
                {
                    AddReward(0.005f);
                }
                else if (spawnPenalty > 0f && spellsPenalty > 0f)
                {
                    AddReward(0.003f);
                }
                else
                {
                    if (alliesRoad0 == 0 || alliesRoad1 == 0)
                    {
                        AddReward(-0.015f);
                    }
                    
                    AddReward(-0.015f);
                }
            }
            else
            {
                var cardEnum = actions.DiscreteActions[1];
                var cardType = actions.DiscreteActions[2];

                if (CardController.Instance.GetCardType((CardName)cardEnum, botTeam) != cardType)
                {
                    AddReward(-0.01f);
                }
                
                if (!CardController.Instance.IsHandContains(botTeam, cardEnum, out CardInfo card))
                {
                    AddReward(-0.01f);
                }
                else
                {
                    AddReward(0.015f);
                    if (CardController.Instance.CardCanBeUsed(botTeam, card))
                    {
                        if ((card.CardType == CardType.Minions && spawnPenalty <= 0f)
                            || (card.CardType == CardType.Spells && spellsPenalty <= 0f)
                            || card.CardType == CardType.Generals) //Generals does not check
                        {
                            if (cardEnum is (int)CardName.TheRock or (int)CardName.AxitRain or (int)CardName.GoHome)
                            {
                                AddReward(-0.01f); // 0.0 in total
                            }
                            else UseCard(card, actions.DiscreteActions[3]); // added reward in this func
                        }
                        else
                        {
                            AddReward(-0.011f); // -0.001 in total
                        }
                    }
                    else
                    {
                        AddReward(-0.02f); // +0.01 in start of else => -0.01 total
                    }
                }
            }
        }

        private void UseCard(CardInfo card, int roadIndex)
        {
            switch (card.CardType)
            {
                case CardType.Minions:
                    if (roadIndex < WorldManager.Instance.RoadAmount)
                    {
                        if (roadIndexRecommend != -1)
                        {
                            if (roadIndex != roadIndexRecommend)
                            {
                                AddReward(-0.03f);
                            }
                            else
                            {
                                AddReward(0.005f);
                            }
                        }
                        else AddReward(0.01f);
                        
                        RefreshSpawnPenalty();
                        CardController.Instance.CardConsuming(botTeam, card);
                        WorldManager.Instance.CreateCharacter(card.Character, roadIndex, botTeam, card.SpellsEffect);
                        var reward = 0.004f * card.Cost; // Standard Minions with 2 Energy => 0.8f ~= 1f reward points
                        AddReward(reward);
                    }
                    else
                    {
                        AddReward(-0.02f); // -0.01 total
                    }
                    break;
                case CardType.Generals:
                    if (roadIndex < WorldManager.Instance.RoadAmount && theGeneralCanSummon)
                    {
                        Debug.Log(card.Name + " in coming!");
                        CardController.Instance.CardConsuming(botTeam, card);
                        WorldManager.Instance.CreateCharacter(card.Character, roadIndex, botTeam, card.SpellsEffect);
                        AddReward(3f);
                    }
                    else
                    {
                        AddReward(-0.01f); // +0.0 total
                    }
                    break;
                case CardType.Spells:
                    // Mind Connect - road - same with Minions spawn
                    // Reward extra reward because its use less Energy to summon Wolf (0.02)
                    // Can be used fail? => minus
                    
                    // Poison Swamp - area - at least 1 enemy to use
                    // Reward if enemy walk in and take damage (0.003)
                    // Can be used fail? => minus
                    switch (card.ActiveType)
                    {
                        case CardActiveType.Road:
                        {
                            if (card.SpellsEffect is SummonSpells summonSpells)
                            {
                                if (roadIndex < WorldManager.Instance.RoadAmount)
                                {
                                    if (roadIndexRecommend != -1)
                                    {
                                        if (roadIndex != roadIndexRecommend)
                                        {
                                            AddReward(-0.03f);
                                        }
                                        else
                                        {
                                            AddReward(0.005f);
                                        }
                                    }
                                    else AddReward(0.01f);
                                    
                                    Debug.Log(botTeam + " " + card.name);
                                    RefreshSpellsPenalty();
                                    summonSpells.RoadIndex = roadIndex;
                                    summonSpells.Team = botTeam;
                                    CardController.Instance.CardConsuming(botTeam, card);
                                    SpellsExecute.Activate(null, card.SpellsEffect);
                                    AddReward(0.015f);
                                }
                                else
                                {
                                    AddReward(-0.02f);
                                }
                            }
                            break;
                        }
                        case CardActiveType.Area:
                        {
                            var character = WorldManager.Instance.GetRandomEnemy(botTeam);
                            if (character is null || alliesRoad0 + alliesRoad1 == 0)
                            {
                                AddReward(-0.02f); // +0.01 in start of if statement => -0.01 total
                            }
                            else
                            {
                                WorldManager.Instance.GetCharactersInArea(character.Position, card.Radius, WorldManager.GetEnemyLayer(botTeam == 0 ? "Team1" : "Team2"), out List<Droppable> area);

                                if (card.SpellsEffect is EnvironmentSpells environmentSpells)
                                {
                                    Debug.Log(botTeam + " " + card.name);
                                    RefreshSpellsPenalty();
                                    environmentSpells.ListSettingUp = area.ConvertAll(d => d.GetComponent<TileData>());
                                    CardController.Instance.CardConsuming(botTeam, card);
                                    StartCoroutine(CountingRewardOnPoisonSwamp(area));
                                    SpellsExecute.Activate(null, card.SpellsEffect);
                                }
                            }
                            break;
                        }
                        default:
                        {
                            AddReward(-0.02f);
                            break;
                        }
                    }
                    break;
            }
        }

        private IEnumerator CountingRewardOnPoisonSwamp(List<Droppable> area)
        {
            while (!area.All(d => d.TryGetComponent(out PoisonSwamp _)))
            {
                yield return null;
            }
            
            while (!area.All(d => d.GetComponent<PoisonSwamp>().IsSetup))
            {
                yield return null;
            }

            area.ForEach(d =>
            {
                var p = d.GetComponent<PoisonSwamp>();
                
                p.OnTriggered += PoisonSwampTriggered;
                p.OnDestroyed += PoisonSwampDestroyed;
            });
        }

        private void PoisonSwampTriggered(object sender, EnvironmentTriggeredEventArgs args)
        {
            if (sender is PoisonSwamp)
            {
                if (args.Amount > 0)
                {
                    AddReward(0.003f);
                    Debug.Log("Poison Swamp reward 0.003");
                }
            }
        }

        private void PoisonSwampDestroyed(object sender, EnvironmentDestroyEventArgs args)
        {
            if (sender is PoisonSwamp poisonSwamp)
            {
                poisonSwamp.OnTriggered -= PoisonSwampTriggered;
                poisonSwamp.OnDestroyed -= PoisonSwampDestroyed;
            }
        }
        
        private void RefreshSpawnPenalty()
        {
            spawnPenalty = Random.Range(2f, 3f); // 3.75f/Energy

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
        
        private void RefreshSpellsPenalty()
        {
            spellsPenalty = Random.Range(2f, 3f); // 3.75f/Energy

            StartCoroutine(SpellsPenalty());
        }

        private IEnumerator SpellsPenalty()
        {
            while (spellsPenalty > 0f)
            {
                spellsPenalty -= Time.deltaTime;
                
                yield return null;
            }
        }

        public void OnHeadquarterDestroy(object sender, System.EventArgs args)
        {
            if (sender is int teamWon)
            {
                /*if (self.BehaviorType != BehaviorType.InferenceOnly)
                {
                    if (botTeam == 1) WorldManager.Instance.SoftReset();
                    
                    allies = 0;
                    enemies = 0;
                }
                if (self.BehaviorType == BehaviorType.InferenceOnly) enabled = false;*/

                if (teamWon == botTeam)
                {
                    SetReward(!theGeneralCanSummon ? 5f : 10f); // won while General can not summon => reduce reward
                }
                else
                {
                    SetReward(-10f);
                }

                if (self.BehaviorType != BehaviorType.InferenceOnly) EndEpisode();
                else enabled = false;
            }
        }

        public void OnCharacterSpawn(object sender, CharacterSpawnEventArgs args)
        {
            if (sender is WorldManager)
            {
                if (args.Team == botTeam)
                {
                    if (args.RoadIndex == 0)
                    {
                        alliesRoad0++;
                    }
                    else
                    {
                        alliesRoad1++;
                    }
                }
                else
                {
                    if (args.RoadIndex == 0)
                    {
                        enemiesRoad0++;
                    }
                    else
                    {
                        enemiesRoad1++;
                    }
                }

                args.Character.OnCharacterDeath += OnCharacterDeath;
            }
        }

        private void OnCharacterDeath(object sender, CharacterDeathEventArgs args)
        {
            if (sender is Character character)
            {
                character.OnCharacterDeath -= OnCharacterDeath;

                if (WorldManager.GetEnemyLayer(botTeam == 0 ? "Team1" : "Team2") == character.gameObject.layer)
                {
                    if (args.RoadIndex == 0)
                    {
                        enemiesRoad0--;
                    }
                    else
                    {
                        enemiesRoad1--;
                    }
                }
                else
                {
                    if (args.RoadIndex == 0)
                    {
                        alliesRoad0--;
                    }
                    else
                    {
                        alliesRoad1--;
                    }
                }
            }
        }

        protected override void OnDisable()
        {
            StopAllCoroutines();
            
            base.OnDisable();
        }
    }
}