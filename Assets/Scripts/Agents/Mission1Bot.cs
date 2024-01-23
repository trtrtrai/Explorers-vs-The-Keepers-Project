using Controllers;
using ScriptableObjects;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Agents
{
    public class Mission1Bot : Agent
    {
        [SerializeField] private BehaviorParameters self;
        [SerializeField] private int botTeam;

        public override void OnEpisodeBegin()
        {
            if (self.BehaviorType != BehaviorType.InferenceOnly) WorldManager.Instance.SoftReset();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
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
                if (CardController.Instance.GetCardCanBeUsed(botTeam).Count == 0)
                {
                    SetReward(0.1f);
                }
                else
                {
                    SetReward(-0.5f / StepCount);
                }
            }
            else
            {
                // use card
                var cardEnum = actions.DiscreteActions[1];

                if (!CardController.Instance.IsHandContains(botTeam, cardEnum, out CardInfo card))
                {
                    SetReward(-0.3f);
                }
                else
                {
                    //if (isDebug) Debug.Log(card.Name);
                    if (CardController.Instance.CardCanBeUsed(botTeam, card))
                    {
                        SetReward(0.5f);
                        CardController.Instance.CardConsuming(botTeam, card);
                        UseCard(card);
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
                    SetReward(0.3f);
                    break;
                case CardType.Generals:
                    
                    break;
                case CardType.Spells:
                    break;
            }
        }

        public void OnHeadquarterDestroy(object sender, System.EventArgs args)
        {
            if (sender is int teamWon)
            {
                if (teamWon == botTeam)
                {
                    SetReward(1f);
                    EndEpisode();
                }
                else
                {
                    SetReward(-1f);
                    EndEpisode();
                }
            }
        }
    }
}