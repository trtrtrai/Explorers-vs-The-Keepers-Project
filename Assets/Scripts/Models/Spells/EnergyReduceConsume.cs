using System;
using System.Collections.Generic;
using Controllers;
using GUI;
using UnityEngine;

namespace Models.Spells
{
    public class EnergyReduceConsume : MonoBehaviour
    {
        [SerializeField] private int energyReduce;
        [SerializeField] private int counting;

        [SerializeField] private bool isSetup;

        [SerializeField] private Hand hand; //get event newCard to reduce energy UI after
        [SerializeField] private List<Card> cardAffect;
        [SerializeField] private EnergyManager applier;

        public void Setup(EnergyManager target, int energyReducePerConsume, int loop)
        {
            if (isSetup) return;

            applier = target;
            applier.CardUsingEvent += IsReduceCost;
            energyReduce = energyReducePerConsume;
            counting = loop;

            isSetup = true;
        }

        private int IsReduceCost(int cost)
        {
            var costResult = -1;
            //check if card contains in cardEffect

            if (counting > 0)
            {
                costResult = Mathf.Clamp(cost - energyReduce, 0, cost);
                counting--;
            }

            if (counting <= 0)
            {
                Destroy(this);
            }

            return costResult;
        }

        private void OnDestroy()
        {
            applier.CardUsingEvent -= IsReduceCost;
        }
    }
}