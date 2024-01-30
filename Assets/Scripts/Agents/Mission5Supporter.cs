using System;
using Controllers;
using Models.Spells;
using ScriptableObjects;
using UnityEngine;

namespace Agents
{
    // Active unlimited Energy Spells and more..
    public class Mission5Supporter : MonoBehaviour
    {
        [SerializeField] private CardInfo unlimitedEnergy;

        public void Active(EnergyManager target)
        {
            unlimitedEnergy = Resources.Load<CardInfo>($"ScriptableObjects/Cards/Mission5State2UnlimitedEnergyCard");
            
            if (unlimitedEnergy.SpellsEffect is EnergyBoostSpells energyBoostSpells)
            {
                energyBoostSpells.Target = target;
                SpellsExecute.Activate(null, unlimitedEnergy.SpellsEffect);
            }
        }
    }
}