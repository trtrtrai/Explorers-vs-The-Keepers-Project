using System;
using System.Collections;
using Controllers;
using EventArgs;
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
    public class Mission2Bot : Agent
    {
        [SerializeField] private BehaviorParameters self;
        [SerializeField] private int botTeam;
        [SerializeField] private float spawnPenalty;
        [SerializeField] private float spellsPenalty;

        [SerializeField] private int allies;
        [SerializeField] private int enemies;

        public override void OnEpisodeBegin()
        {
            if (self.BehaviorType != BehaviorType.InferenceOnly && botTeam == 1) WorldManager.Instance.SoftReset();

            allies = 0;
            enemies = 0;

            //initial penalty [New1 2-2.5] [New2 1-1.5]
            spawnPenalty = Random.Range(1f, 1.5f);
            spellsPenalty = spawnPenalty;
            StartCoroutine(SpawnPenalty());
            StartCoroutine(SpellsPenalty());
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            /*
             * Observation index 1: Vector2(x, y) with x is number of card can use (0-29), y is type of card (Minions, Generals, Spells) => 2
             * Observation index 2: Vector2(x, y) like index 1, it is a information of the second card on hand => 2
             * Observation index 3: Vector2(x, y) like index 1, it is a information of the third card on hand => 2
             * Observation index 4: Vector2(x, y) like index 1, it is a information of the fourth card on hand => 2
             * Observation index 5: Vector2(x, y) like index 1, it is a information of the fifth card on hand => 2
             * Observation index 6: number of allies on war field
             * Observation index 7: number of enemies on war field
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
            
            sensor.AddObservation(allies);
            sensor.AddObservation(enemies);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            /*
             * DiscreteActions index 0: is used card? 0-1
             * DiscreteActions index 1: what card is used? 0-29
             * DiscreteActions index 2: what type of card? 0-2
             * DiscreteActions index 3: what roadIndex would use the card? 0-1
             */
            
            if (actions.DiscreteActions[0] == 0)
            {
                if (CardController.Instance.GetCardCanBeUsed(botTeam).Count == 0)
                {
                    AddReward(0.005f);
                }
                else
                {
                    AddReward(-0.005f);
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
                    AddReward(0.01f);
                    if (CardController.Instance.CardCanBeUsed(botTeam, card))
                    {
                        if ((card.CardType == CardType.Minions && spawnPenalty <= 0f)
                            || (card.CardType == CardType.Spells && spellsPenalty <= 0f)) 
                        {
                            UseCard(card, actions.DiscreteActions[3]); // added reward in this func
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
                        RefreshSpawnPenalty();

                        if (WorldManager.Instance.IsGoodWhenPlaceCharacterOn(roadIndex, botTeam))
                        {
                            AddReward(-0.015f); // +0.005 in total instead of +0.02
                        }
                        
                        CardController.Instance.CardConsuming(botTeam, card);
                        WorldManager.Instance.CreateCharacter(card.Character, roadIndex, botTeam, card.SpellsEffect);
                        AddReward(0.01f);
                    }
                    else
                    {
                        AddReward(-0.02f); // +0.01 in start of if statement => -0.01 total
                    }
                    break;
                case CardType.Generals:
                    break;
                case CardType.Spells:
                    // Angry spirit - single ally - needed number of ally character minimum was 1 - priority active on ally at TilePosition higher than average.
                    // Reward = 0.001 per buffed ally attacked
                    // Can be used fail? => minus
                    
                    // Protect - area ally - needed place where at least 1 ally to use - priority active on ally at TilePosition higher than average in area.
                    // Reward = 0.001 per ally affect
                    // Can be used fail? => minus
                    switch (card.ActiveType)
                    {
                        case CardActiveType.WarField:
                            break;
                        case CardActiveType.WarFieldEnemy:
                            break;
                        case CardActiveType.Road:
                            break;
                        case CardActiveType.Single:
                        {
                            // Just only check for attack buff Spells
                            var character = WorldManager.Instance.GetRandomAlly(botTeam);
                            if (character is null || allies == 0)
                            {
                                AddReward(-0.02f); // +0.01 in start of if statement => -0.01 total
                            }
                            else
                            {
                                Debug.Log(botTeam + " " + card.name);
                                RefreshSpellsPenalty();
                                character.GetComponent<CharacterAttacking>().OnAttack += OnCharacterAttack; // need to see time effect to stop listen
                                CardController.Instance.CardConsuming(botTeam, card);
                                SpellsExecute.Activate(character, card.SpellsEffect);
                            }

                            break;
                        }
                        case CardActiveType.Area:
                        {
                            // Just only check healing Spells
                            var character = WorldManager.Instance.GetRandomAlly(botTeam);
                            if (character is null || allies == 0)
                            {
                                AddReward(-0.02f); // +0.01 in start of if statement => -0.01 total
                            }
                            else
                            {
                                var characters = WorldManager.Instance.GetCharactersInArea(character.Position, card.Radius, WorldManager.GetEnemyLayer(botTeam == 0 ? "Team2" : "Team1"));

                                if (characters.Count == 0)
                                {
                                    AddReward(-0.02f); // +0.01 in start of if statement => -0.01 total
                                }
                                else
                                {
                                    Debug.Log(botTeam + " " + card.name);
                                    RefreshSpellsPenalty();
                                    CardController.Instance.CardConsuming(botTeam, card);
                                    foreach (var effector in characters)
                                    {
                                        SpellsExecute.Activate(effector, card.SpellsEffect);
                                        AddReward(0.001f);
                                    }
                                }
                            }
                            break;
                        }
                        case CardActiveType.SingleEnemy:
                            break;
                        case CardActiveType.AreaEnemy:
                            break;
                        case CardActiveType.World:
                            break;
                    }
                    break;
            }
        }

        private void OnCharacterAttack(object sender, CharacterAttackEventArgs args)
        {
            if (sender is GameObject obj && obj.TryGetComponent(out Character character) && args.DamageDeal > 0)
            {
                AddReward(0.001f);
            }
        }

        private void RefreshSpawnPenalty()
        {
            spawnPenalty = Random.Range(5f, 6f); // 3.75f/Energy - [New1 8-10] [New2 5-6]

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
            spellsPenalty = Random.Range(2f, 3f); // 3.75f/Energy [New1 3-4] [New2 2-3]

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
                    SetReward(1f);
                }
                else
                {
                    SetReward(-1f);
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
                    allies++;
                }
                else
                {
                    enemies++;
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
                    enemies--;
                }
                else
                {
                    allies--;
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